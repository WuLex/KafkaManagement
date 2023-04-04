namespace KafkaMonitor.Models
{
    public class Metric
    {
        public Metric(string name, double value, string unit, string description)
        {
            Name = name;
            Value = value;
            Unit = unit;
            Description = description;
        }

        public string Name { get; }
        public double Value { get; }
        public string Unit { get; }
        public string Description { get; }

        public override string ToString()
        {
            return $"{Name}: {Value} {Unit} ({Description})";
        }
    }
}