using Confluent.Kafka;
using System.Text;
using System.Text.Json;

namespace KafkaMonitor.Utils
{
    public class StringDeserializer : IDeserializer<string>
    {
        public string Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull)
            {
                return null;
            }

            var json = Encoding.UTF8.GetString(data.ToArray());
            return JsonSerializer.Deserialize<string>(json);
        }
    }
}
