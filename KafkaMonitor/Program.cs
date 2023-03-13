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
            // ע�� Kafka �ͻ���
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

            // ��ȡ�����ߺ�������ʵ��
            //var producer = app.Services.GetRequiredService<IProducer<byte[], byte[]>>();
            //var consumer = app.Services.GetRequiredService<IConsumer<byte[], byte[]>>();
           

            // ����Ҫʹ�������ߺ������ߵ�����ʹ�ù��캯��ע��ķ�ʽ��ȡʵ��
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