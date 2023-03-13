using Confluent.Kafka;
using Confluent.Kafka.DependencyInjection;
using KafkaMonitor.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace KafkaMonitor.Controllers
{
    public class KafkaController : Controller
    {
        private readonly KafkaService _kafkaService;
        private readonly IConsumer<string, string> _consumer;
        //private readonly KafkaClient<string, string> _kafkaClient;

        //public KafkaController(KafkaClient<string, string> kafkaClient)
        //{
        //    _kafkaClient = kafkaClient;
        //}

        public KafkaController(IConsumer<string, string> consumer, KafkaService kafkaService)
        {
            _consumer = consumer;
            _kafkaService = kafkaService;
        }

        public IActionResult Index()
        {
            //var messages = _consumer.Consume(10).ToList();
            var status = _kafkaService.GetKafkaStatus();
            ViewBag.Status = status;
            return View();
        }

        //public IActionResult IndexTwo()
        //{
        //    var consumerGroups = _kafkaClient.AdminClient.ListConsumerGroups().ToList();

        //    var topics = _kafkaClient.AdminClient.GetMetadata().Topics;

        //    var messages = new List<Message<string, string>>();

        //    foreach (var topic in topics)
        //    {
        //        var partitions = _kafkaClient.Consumer.GetPartitions(topic.Topic);

        //        foreach (var partition in partitions)
        //        {
        //            var watermarkOffsets = _kafkaClient.Consumer.QueryWatermarkOffsets(topic.Topic, partition);

        //            var committedOffsets = _kafkaClient.Consumer.Committed(new TopicPartition(topic.Topic, partition));

        //            var position = _kafkaClient.Consumer.Position(new List<TopicPartition> { new TopicPartition(topic.Topic, partition) });

        //            var low = watermarkOffsets.Low;

        //            var high = watermarkOffsets.High;

        //            var committed = committedOffsets.Offset.Value;

        //            var lag = high - committed;

        //            var messagesInPartition = _kafkaClient.Consumer.Consume(lag, TimeSpan.FromSeconds(1));

        //            messages.AddRange(messagesInPartition);
        //        }
        //    }

        //    var model = new KafkaStatusViewModel
        //    {
        //        ConsumerGroups = consumerGroups,
        //        Topics = topics,
        //        Messages = messages
        //    };

        //    return View(model);
        //}
        public IActionResult Status()
        {
            var status = _kafkaService.GetKafkaStatus();
            ViewBag.Status = status;
            return View();
        }

        //使用Kafka服务类来接收消息并将它们发送到视图
        public IActionResult ConsumeMessage()
        {
            var messages = new List<string>();

            _kafkaService.Consume(message =>
            {
                messages.Add(message);
                // Notify clients that new messages are available
                // (you can use SignalR or other real-time messaging technologies for this)
            });

           return View(messages);
        }


    }
}
