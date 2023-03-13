using Confluent.Kafka;
using Confluent.Kafka.DependencyInjection;
 

namespace KafkaMonitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Kafka
            // 注册 Kafka 客户端
            //builder.Services.AddKafkaClient(options =>
            //{
            //    options.ProducerConfig.BootstrapServers = "localhost:9092";
            //    options.ConsumerConfig.BootstrapServers = "localhost:9092";
            //});
            var _configuration = builder.Configuration;

            //builder.Services.AddKafkaClient(options =>
            //{
            //    options.ConsumerConfig.BootstrapServers = _configuration["Kafka:BootstrapServers"];
            //    options.ProducerConfig.BootstrapServers = _configuration["Kafka:BootstrapServers"];
            //});
            builder.Services.AddKafkaClient(new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
                //EnableIdempotence = true
            });

            builder.Services.AddKafkaClient(new ConsumerConfig
            {
                BootstrapServers = "somewhere.else:9092",
                //GroupId = "group1"
            });
            //--------------------------------------------------------------------------------
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
                ClientId = "AspNetCoreKafkaMonitoring",
                MessageTimeoutMs = 5000
            };

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "AspNetCoreKafkaMonitoring",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

           
            #endregion
             
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // 获取生产者和消费者实例
            //var producer = app.Services.GetRequiredService<IProducer<byte[], byte[]>>();
            //var consumer = app.Services.GetRequiredService<IConsumer<byte[], byte[]>>();
           

            // 在需要使用生产者和消费者的类中使用构造函数注入的方式获取实例
            //builder.Services.AddTransient<KafkaService>(sp => new KafkaService(_configuration, producer, consumer));


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}