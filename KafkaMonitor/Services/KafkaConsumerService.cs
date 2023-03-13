using Confluent.Kafka;

namespace KafkaMonitor.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<IConsumer<string, string>>();
            consumer.Subscribe(_configuration.GetSection("Kafka:Consumer:Topics").Get<string[]>());

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(stoppingToken);

                if (result != null)
                {
                    _logger.LogInformation($"Received message: {result.Value}");
                    // 处理Kafka消息
                }
            }

            consumer.Close();
        }
    }
}