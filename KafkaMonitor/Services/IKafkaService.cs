namespace KafkaMonitor.Services
{
    public interface IKafkaService
    {
        public string GetKafkaStatus();
        public void Consume(Action<string> handleMessage);
    }
}
