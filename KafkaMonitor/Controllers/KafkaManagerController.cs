using Confluent.Kafka;
using KafkaMonitor.Utils;
using Microsoft.AspNetCore.Mvc;

namespace KafkaMonitor.Controllers
{
    public class KafkaManagerController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IConsumer<string, string> _consumer;
        private readonly IProducer<string, string> _producer;

        public KafkaManagerController(IConfiguration config, IConsumer<string, string> consumer, IProducer<string, string> producer)
        {
            _config = config;
            _consumer = consumer;
            _producer = producer;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("topics")]
        public ActionResult<IEnumerable<string>> GetTopics()
        {
            var bootstrapServers = _config.GetValue<string>("Kafka:BootstrapServers");
            var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build();
            var topics = adminClient.GetMetadata(TimeSpan.FromSeconds(5)).Topics.Select(t => t.Topic).ToList();
            return Ok(topics);
        }

        [HttpGet("topics/{topic}/partitions")]
        public ActionResult<IEnumerable<int>> GetPartitions(string topic)
        {
            var bootstrapServers = _config.GetValue<string>("Kafka:BootstrapServers");
            var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build();
            var metadata = adminClient.GetMetadata(topic, TimeSpan.FromSeconds(5));
            var partitions = metadata.Topics
                .Single(t => t.Topic == topic)
                .Partitions
                .Select(p => p.PartitionId)
                .ToList();
            return Ok(partitions);
        }

        [HttpGet("topics/{topic}/messages")]
        public ActionResult<IEnumerable<string>> GetMessages(string topic, int partition, long offset, int count)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _config.GetValue<string>("Kafka:BootstrapServers"),
                GroupId = "kafka-monitoring-tool"
            };
            var consumerBuilder = new ConsumerBuilder<Ignore, string>(consumerConfig)
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                .SetValueDeserializer(new StringDeserializer());
            using var consumer = consumerBuilder.Build();

            var partitionOffset = new TopicPartitionOffset(new TopicPartition(topic, new Partition(partition)), new Offset(offset));
            consumer.Assign(new List<TopicPartitionOffset> { partitionOffset });

            var messages = new List<string>();
            while (messages.Count < count)
            {
                var consumeResult = consumer.Consume();
                if (consumeResult.IsPartitionEOF)
                {
                    break;
                }
                messages.Add(consumeResult.Message.Value);
            }

            return Ok(messages);
        }

        [HttpPost("topics/{topic}/messages")]
        public async Task<ActionResult> PostMessage(string topic, [FromBody] string message)
        {
            var producerConfig = new ProducerConfig { BootstrapServers = _config.GetValue<string>("Kafka:BootstrapServers") };
            var producerBuilder = new ProducerBuilder<Null, string>(producerConfig)
                .SetValueSerializer(new StringSerializer());
            using var producer = producerBuilder.Build();

            var deliveryResult = await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
            Console.WriteLine($"Delivered message '{deliveryResult.Value}' to topic {deliveryResult.Topic}, partition {deliveryResult.Partition}, offset {deliveryResult.Offset}");

            return Ok();
        }
    }
}