using Confluent.Kafka;

namespace KafkaMonitor.Models
{
    public class KafkaStatusViewModel
    {
        public List<ConsumerGroup> ConsumerGroups { get; set; }
        public List<TopicMetadata> Topics { get; set; }
        public List<ConsumeResult<string, string>> Messages { get; set; }
    }

    public class ConsumerGroup
    {
        public string GroupId { get; set; }
    }

    public class TopicMetadata
    {
        public string Topic { get; set; }
        public List<PartitionMetadata> Partitions { get; set; }
    }

    public class PartitionMetadata
    {
        public int Partition { get; set; }
        public int LowWatermarkOffset { get; set; }
        public int HighWatermarkOffset { get; set; }
        public int CommittedOffset { get; set; }
        public int Lag { get; set; }
    }
}
