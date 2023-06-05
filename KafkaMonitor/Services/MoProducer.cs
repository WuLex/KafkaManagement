using Confluent.Kafka;

namespace KafkaMonitor.Services
{
    public class MoProducer : IProducer<byte[], byte[]>
    {
        public Handle Handle => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public void AbortTransaction(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void AbortTransaction()
        {
            throw new NotImplementedException();
        }

        public int AddBrokers(string brokers)
        {
            throw new NotImplementedException();
        }

        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int Flush(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void Flush(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void InitTransactions(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public int Poll(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void Produce(string topic, Message<byte[], byte[]> message, Action<DeliveryReport<byte[], byte[]>> deliveryHandler = null)
        {
            throw new NotImplementedException();
        }

        public void Produce(TopicPartition topicPartition, Message<byte[], byte[]> message, Action<DeliveryReport<byte[], byte[]>> deliveryHandler = null)
        {
            throw new NotImplementedException();
        }

        public Task<DeliveryResult<byte[], byte[]>> ProduceAsync(string topic, Message<byte[], byte[]> message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<DeliveryResult<byte[], byte[]>> ProduceAsync(TopicPartition topicPartition, Message<byte[], byte[]> message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void SendOffsetsToTransaction(IEnumerable<TopicPartitionOffset> offsets, IConsumerGroupMetadata groupMetadata, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void SetSaslCredentials(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
