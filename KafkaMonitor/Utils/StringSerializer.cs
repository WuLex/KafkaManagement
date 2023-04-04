using Confluent.Kafka;
using System.Text;
using System.Text.Json;

namespace KafkaMonitor.Utils
{
    public class StringSerializer : ISerializer<string>
    {
        public byte[] Serialize(string data, SerializationContext context)
        {
            if (data == null)
            {
                return null;
            }

            var json = JsonSerializer.Serialize(data);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
