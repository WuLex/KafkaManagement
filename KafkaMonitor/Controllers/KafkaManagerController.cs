using Confluent.Kafka;
using KafkaMonitor.Dtos;
using KafkaMonitor.Utils;
using Microsoft.AspNetCore.Mvc;

namespace KafkaMonitor.Controllers
{

    public class KafkaManagerController : Controller
    {
        private readonly KafkaService _kafkaService;
        private readonly IConfiguration _config;
        private readonly IConsumer<string, string> _consumer;
        private readonly IProducer<string, string> _producer;

        public KafkaManagerController(IConfiguration config, KafkaService kafkaService, IConsumer<string, string> consumer, IProducer<string, string> producer)
        {
            _config = config;
            _kafkaService = kafkaService;
            _consumer = consumer;
            _producer = producer;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("KafkaManager/topics")]
        [HttpGet()]
        public ActionResult<IEnumerable<string>> GetTopics()
        {
            var bootstrapServers = _config.GetValue<string>("Kafka:BootstrapServers");
            var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build();
            var topics = adminClient.GetMetadata(TimeSpan.FromSeconds(5)).Topics.Select(t => t.Topic).ToList();
            return Ok(topics);
        }


        [Route("KafkaManager/{topic}/partitions")]
        [HttpGet("KafkaManager/{topic}/partitionsS")]
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

        [Route("KafkaManager/{topic}/messages")]
        [HttpGet("KafkaManager/{topic}/messages")]
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

        [Route("KafkaManager/{topic}/messages")]
        [HttpPost("KafkaManager/{topic}/messages")]
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

        [Route("KafkaManager/messages")]
        [HttpGet("KafkaManager/messages")]
        public IActionResult GetMessages()
        {
            //var config = new ConsumerConfig
            //{
            //    BootstrapServers = _config["Kafka:BootstrapServers"],
            //    GroupId = _config["Kafka:GroupId"],
            //    AutoOffsetReset = AutoOffsetReset.Earliest,
            //    EnableAutoCommit = false
            //};

            //var consumerBuilder =new ConsumerBuilder<Ignore, string>(config)
            //    .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            //    .Build();
            try
            {
                var messages = _kafkaService.GetMessages();
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("KafkaManager/send")]
        [HttpPost("KafkaManager/send")]
        public async Task<IActionResult> SendMessageAsync([FromBody] KafkaMessageDto messageDto)
        {
            try
            {
                var message = new Message<string, string>
                {
                    Key = messageDto.Topic,
                    Value = messageDto.Message
                };
                await _kafkaService.SendMessage(message);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
    }
}