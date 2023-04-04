using Confluent.Kafka;
using KafkaMonitor.Services;
using System;
using System.Text;

public class KafkaService : IDisposable,IKafkaService
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
    private readonly CancellationTokenSource _cancellationTokenSource;


    private readonly IProducer<byte[], byte[]> _producer;
    private readonly IConsumer<byte[], byte[]> _consumer;
    private readonly IConsumer<string, string> _consumerTwo;

    public KafkaService()
    { 
    }

    public KafkaService(IConfiguration configuration,IProducer<byte[], byte[]> producer, IConsumer<byte[], byte[]> consumer)
    {
        //_bootstrapServers = configuration["KafkaConfig:BootstrapServers"];
        //_groupId = configuration["KafkaConfig:GroupId"];
        //_clientId = configuration["KafkaConfig:ClientId"];
        //_securityProtocol = configuration["KafkaConfig:SecurityProtocol"];
        //_saslMechanism = configuration["KafkaConfig:SaslMechanism"];
        //_saslUsername = configuration["KafkaConfig:SaslUsername"];
        //_saslPassword = configuration["KafkaConfig:SaslPassword"];
        //_sslCaLocation = configuration["KafkaConfig:SslCaLocation"];
        //_autoOffsetReset = configuration["KafkaConfig:AutoOffsetReset"];
   
        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["KafkaConfig:BootstrapServers"],
            GroupId = configuration["KafkaConfig:GroupId"],
            ClientId = configuration["KafkaConfig:ClientId"],
            SecurityProtocol = Enum.Parse<SecurityProtocol>(configuration["KafkaConfig:SecurityProtocol"]),
            SaslMechanism = Enum.Parse<SaslMechanism>(configuration["KafkaConfig:SaslMechanism"]),
            SaslUsername = configuration["KafkaConfig:SaslUsername"],
            SaslPassword = configuration["KafkaConfig:SaslPassword"],
            SslCaLocation = configuration["KafkaConfig:SslCaLocation"],
            AutoOffsetReset = Enum.Parse<AutoOffsetReset>(configuration["KafkaConfig:AutoOffsetReset"])
        };

        _consumerTwo = new ConsumerBuilder<string, string>(config)
            //.SetValueDeserializer(new StringDeserializer(Encoding.UTF8))
            .Build();

        _cancellationTokenSource = new CancellationTokenSource();
        var topics = new List<string> { "my-topic" };
        _consumerTwo.Subscribe(topics);

        _producer = producer;
        _consumer = consumer;
    }
  
    public string GetKafkaStatus()
    {
        try
        {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _bootstrapServers }).Build())
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(20));
                return $"Kafka is up and running. Found {metadata.Brokers.Count} broker(s) and {metadata.Topics.Count} topic(s).";
            }
        }
        catch (Exception ex)
        {
            return $"Error connecting to Kafka: {ex.Message}";
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
   
    public void Dispose()
    {
        _consumer.Close();
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}