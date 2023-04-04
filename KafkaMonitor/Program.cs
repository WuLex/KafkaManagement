using Confluent.Kafka;
using Confluent.Kafka.DependencyInjection;
using KafkaMonitor.Services;
using KafkaMonitor.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static Confluent.Kafka.ConfigPropertyNames;

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


            #region 2.0.0注册方式
            //builder.Services.AddKafkaClient(new ProducerConfig
            //{
            //    BootstrapServers = "localhost:9092",
            //    //EnableIdempotence = true
            //});

            //builder.Services.AddKafkaClient(new ConsumerConfig
            //{
            //    BootstrapServers = "somewhere.else:9092",
            //    //GroupId = "group1"
            //}); 
            #endregion

            #region MyRegion 

            builder.Services.AddSingleton(new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = _configuration["Kafka:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                MaxPollIntervalMs = 300000,
                MaxPartitionFetchBytes = 1024 * 1024,
                FetchWaitMaxMs = 5000
            });

            builder.Services.AddSingleton(new ProducerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"]
            });

            builder.Services.AddSingleton(c => new ConsumerBuilder<string, string>(
                c.GetRequiredService<ConsumerConfig>())
                .SetValueDeserializer(new StringDeserializer())
                .Build());

            builder.Services.AddSingleton(c => new ProducerBuilder<string, string>(
                c.GetRequiredService<ProducerConfig>())
                .SetValueSerializer(new StringSerializer())
            .Build());

            //--------------------------------------------------------------------------------
            builder.Services.AddSingleton<IProducer<Byte[], Byte[]>>(new MoProducer());
            builder.Services.AddSingleton<IConsumer<Byte[], Byte[]>>(new MoConsumer());

            //builder.Services.AddSingleton<IKafkaService, KafkaService>();
            //builder.Services.AddScoped<IKafkaService, KafkaService>();
            builder.Services.AddScoped<IKafkaService>((sp) =>
            {
                var producer = sp.GetRequiredService<IProducer<byte[], byte[]>>();
                var consumer = sp.GetRequiredService<IConsumer<byte[], byte[]>>();
                return new KafkaService(_configuration, producer, consumer);
            });
            #endregion
            //--------------------------------------------------------------------------------
            //var producerConfig = new ProducerConfig
            //{
            //    BootstrapServers = "localhost:9092",
            //    ClientId = "AspNetCoreKafkaMonitoring",
            //    MessageTimeoutMs = 5000
            //};

            //var consumerConfig = new ConsumerConfig
            //{
            //    BootstrapServers = "localhost:9092",
            //    GroupId = "AspNetCoreKafkaMonitoring",
            //    AutoOffsetReset = AutoOffsetReset.Earliest
            //};
            //--------------------------------------------------------------------------------

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