using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mySparkStreamingApp2.Kafka
{
    public class KafkaProducerHostService1 : IHostedService
    {
        public readonly ILogger<KafkaProducerHostService> _logger;
        private IProducer<Null, string> _producer;

        public KafkaProducerHostService1(ILogger<KafkaProducerHostService> logger)
        {
            _logger = logger;
            var config = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092"
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 10; ++i)
            {
                var value = $"hello world {i}";
                _logger.LogInformation(value);
                await _producer.ProduceAsync("demo", new Message<Null, string>()
                {
                    Value = value
                }, cancellationToken);
            }
            _producer.Flush(TimeSpan.FromSeconds(10));
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _producer?.Dispose();
            return Task.CompletedTask;
        }
    }
}
