using Confluent.Kafka;
using KafkaMonitor.Utils;

namespace KafkaMonitor.Extensions
{
    public static class KafkaDependencyInjection
    {
        public static void AddKafka(this IServiceCollection services, IConfiguration configuration)
        {
            var bootstrapServers = configuration.GetValue<string>("Kafka:BootstrapServers");

            // Configure producer
            services.AddSingleton<IProducer<string, string>>(_ =>
            {
                var producerConfig = new ProducerConfig { BootstrapServers = bootstrapServers };
                var producerBuilder = new ProducerBuilder<string, string>(producerConfig)
                    .SetValueSerializer(new StringSerializer());
                return producerBuilder.Build();
            });

            // Configure consumer
            services.AddSingleton<IConsumer<string, string>>(_ =>
            {
                var consumerConfig = new ConsumerConfig
                {
                    BootstrapServers = bootstrapServers,
                    GroupId = "kafka-monitoring-tool"
                };
                var consumerBuilder = new ConsumerBuilder<string, string>(consumerConfig)
                    .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                    .SetValueDeserializer(new StringDeserializer());
                return consumerBuilder.Build();
            });

            // Configure Kafka service
            services.AddSingleton<KafkaService>();
        }
    }
}