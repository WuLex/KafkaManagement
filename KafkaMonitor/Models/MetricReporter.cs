using System.Diagnostics;

namespace KafkaMonitor.Models
{
    public class MetricReporter
    {
        private readonly Metric sendRate;
        private Timer timer;
        private CancellationToken cancellationToken;

        public MetricReporter(Metric sendRate)
        {
            this.sendRate = sendRate;
        }

        public void Start(TimeSpan interval, CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            timer = new Timer(_ => Report(), null, TimeSpan.Zero, interval);
        }

        public void Stop()
        {
            timer.Dispose();
            Report();
        }

        private void Report()
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var value = sendRate.Value;
            Debug.WriteLine($"Send rate: {value} messages/second");
        }
    }
}