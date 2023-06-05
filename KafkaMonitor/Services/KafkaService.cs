using Confluent.Kafka;
using KafkaMonitor.Models;
using KafkaMonitor.Services;
using KafkaMonitor.Utils;
using System;
using System.Text;

public class KafkaService : IDisposable
{
    private readonly string _bootstrapServers;
    private readonly string _groupId;
    private readonly string _clientId;
    private readonly string _securityProtocol;
    private readonly string _saslMechanism;
    private readonly string _saslUsername;
    private readonly string _saslPassword;
    private readonly string _sslCaLocation;
    private readonly string _autoOffsetReset;

    private readonly IConfiguration _config;
    private readonly ILogger<KafkaService> _logger;
    private readonly IProducer<string, string> _producer;
    private readonly IConsumer<string, string> _consumer;
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    //private readonly IProducer<byte[], byte[]> _producer;
    //private readonly IConsumer<byte[], byte[]> _consumer;
    private readonly IConsumer<string, string> _consumerTwo;
 
    public KafkaService(IConfiguration configuration,IProducer<string, string> producer, IConsumer<string, string> consumer)
    {
        _bootstrapServers = configuration["KafkaConfig:BootstrapServers"];
        //_groupId = configuration["KafkaConfig:GroupId"];
        //_clientId = configuration["KafkaConfig:ClientId"];
        //_securityProtocol = configuration["KafkaConfig:SecurityProtocol"];
        //_saslMechanism = configuration["KafkaConfig:SaslMechanism"];
        //_saslUsername = configuration["KafkaConfig:SaslUsername"];
        //_saslPassword = configuration["KafkaConfig:SaslPassword"];
        //_sslCaLocation = configuration["KafkaConfig:SslCaLocation"];
        //_autoOffsetReset = configuration["KafkaConfig:AutoOffsetReset"];
        _config = configuration;
        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["KafkaConfig:BootstrapServers"],
            GroupId = configuration["KafkaConfig:GroupId"],
            //ClientId = configuration["KafkaConfig:ClientId"],
            //SecurityProtocol = Enum.Parse<SecurityProtocol>(configuration["KafkaConfig:SecurityProtocol"]),
            //SaslMechanism = Enum.Parse<SaslMechanism>(configuration["KafkaConfig:SaslMechanism"]),
            //SaslUsername = configuration["KafkaConfig:SaslUsername"],
            //SaslPassword = configuration["KafkaConfig:SaslPassword"],
            //SslCaLocation = configuration["KafkaConfig:SslCaLocation"],
            //AutoOffsetReset = Enum.Parse<AutoOffsetReset>(configuration["KafkaConfig:AutoOffsetReset"])
        };

        _consumerTwo = new ConsumerBuilder<string, string>(config)
            .SetValueDeserializer(new StringDeserializer())
            .Build();
        var topics = new List<string> { "mytopic" };
        _consumerTwo.Subscribe(topics);


        _cancellationTokenSource = new CancellationTokenSource();




        _producer = producer;
        _consumer = consumer;
    }
  
    public KafkaStatusViewModel GetKafkaStatus()
    {

        KafkaStatusViewModel kafkaStatusViewModel = new KafkaStatusViewModel();

        try
        {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _bootstrapServers }).Build())
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(20));
                kafkaStatusViewModel.Topics = metadata.Topics;
                kafkaStatusViewModel.Brokers = metadata.Brokers;

                return kafkaStatusViewModel;
                //return $"Kafka is up and running. Found {metadata.Brokers.Count} broker(s) and {metadata.Topics.Count} topic(s).";
            }
        }
        catch (Exception ex)
        {
            //return $"Error connecting to Kafka: {ex.Message}";

            return kafkaStatusViewModel;
        }
    }


    public void Consume(Action<string> handleMessage)
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                var consumeResult =  _consumerTwo.Consume(_cancellationTokenSource.Token);
                handleMessage(consumeResult.Value);
            }
            catch (OperationCanceledException)
            {
                // Graceful shutdown
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }
    }
    public async Task<IEnumerable<dynamic>> GetMessages()
    {
        var topics = _config.GetSection("Kafka:Consumer:Topics").Get<List<string>>();

        var result = new List<dynamic>();
        var consumerConfig = new ConsumerConfig
        {
            //BootstrapServers = _bootstrapServers,
            //GroupId = "my-group",

            BootstrapServers = _bootstrapServers,
            GroupId = "csharp-consumer",
            EnableAutoOffsetStore = false,
            EnableAutoCommit = true,
            StatisticsIntervalMs = 5000,
            SessionTimeoutMs = 6000,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnablePartitionEof = true,
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky

            ////EnableAutoOffsetStore = false,
            ////EnableAutoCommit = true,
            //StatisticsIntervalMs = 5000,
            //SessionTimeoutMs = 6000,
            ////AutoOffsetReset = AutoOffsetReset.Earliest,
            ////EnablePartitionEof = true,
            ////PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
        };
        

        try
        {
            using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                consumer.Subscribe(topics);

                while (true)
                {
                    var consumeResult = consumer.Consume();
                    if (consumeResult == null)
                    {
                        continue;
                    }

                    result.Add(new
                    {
                        topic = consumeResult.Topic,
                        partition = consumeResult.Partition.Value,
                        offset = consumeResult.Offset.Value,
                        message = consumeResult.Message?.Value
                    });

                    if (consumeResult.IsPartitionEOF)
                    {
                        return result;
                    }
                }

                consumer.Close();
            }

        }
        catch (Exception ex)
        {
           Console.WriteLine(ex);
        }

        return result;
    }
    //public async Task<IEnumerable<dynamic>> GetMessages(IConsumer<Ignore, string> consumer)
    //{
    //    var topics = _config.GetSection("Kafka:Topics").Get<List<string>>();

    //    var result = new List<dynamic>();

    //    using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _bootstrapServers }).Build())
    //    {
    //        var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(20));
    //        var topicMetadataList = metadata.Topics;

    //        foreach (var topicMetadata in topicMetadataList)
    //        {
    //            foreach (var partition in topicMetadata.Partitions)
    //            {
    //                var topicPartition = new TopicPartition(topicMetadata.Topic, new Partition(partition.PartitionId));
    //                var watermarks = consumer.QueryWatermarkOffsets(topicPartition, TimeSpan.FromSeconds(5));
    //                var offset = new TopicPartitionOffset(topicPartition, watermarks.High - 1);

    //                consumer.Assign(topicPartition);
    //                consumer.Seek(offset);

    //                while (true)
    //                {
    //                    var consumerResult = consumer.Consume(CancellationToken.None);

    //                    if (consumerResult == null)
    //                    {
    //                        break;
    //                    }

    //                    result.Add(new
    //                    {
    //                        topic = topicMetadata.Topic,
    //                        partition = partition.PartitionId,
    //                        offset = consumerResult.Offset.Value,
    //                        message = consumerResult.Value
    //                    });
    //                }
    //            }
    //        }
    //    }

    //    return result;
    //}

    public async Task<bool> SendMessage(Message<string, string> message)
    {
        //var topic = _config.GetValue<string>("Kafka:DefaultTopic");
        var topic =string.IsNullOrEmpty(message.Key)? _config.GetValue<string>("Kafka:DefaultTopic"): message.Key.Trim();
        try
        {
            var deliveryResult = await _producer.ProduceAsync(topic, new Message<string, string> { Value = message.Value });
            //_logger.LogInformation($"Sent message to topic {deliveryResult.Topic}, partition {deliveryResult.Partition}, offset {deliveryResult.Offset}");
            return true;
        }
        catch (ProduceException<string, string> e)
        {
            _logger.LogError($"Failed to send message: {e.Error.Reason}");
            return false;
        }
    }

    private async Task ConsumeMessages(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_config.GetSection("Kafka:Topics").Get<List<string>>());

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var consumerResult = _consumer.Consume(TimeSpan.FromSeconds(1));
                if (consumerResult != null)
                {
                    _logger.LogInformation($"Received message from topic {consumerResult.Topic}, partition {consumerResult.Partition}, offset {consumerResult.Offset}: {consumerResult.Value}");
                }
            }
            catch (ConsumeException e)
            {
                _logger.LogError($"Error consuming message: {e.Error.Reason}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Unexpected error: {e.Message}");
            }
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _consumer.Close();
        _consumer.Dispose();
        _producer.Flush();
        _producer.Dispose();
        _cancellationTokenSource.Dispose();
    }
}