using Confluent.Kafka;
using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Kafka.Public;
using Kafka.Public.Loggers;
using System.Text;

namespace mySparkStreamingApp2.Kafka
{
    public class KafkaConsumerHostService : IHostedService
    {

        public readonly ILogger<KafkaConsumerHostService> _logger;
        private readonly ClusterClient _cluster;
        public KafkaConsumerHostService(ILogger<KafkaConsumerHostService> logger)
        {
            _logger = logger;
            _cluster = new ClusterClient(new Configuration
            {
                Seeds = "localhost:9092"
            }, new ConsoleLogger());
        }



        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cluster.ConsumeFromLatest("demo");
            _cluster.MessageReceived += record =>
            {
                _logger.LogInformation($"Received: {Encoding.UTF8.GetString(record.Value as byte [])}");
            };
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cluster?.Dispose();
            return Task.CompletedTask;
        }
    }
}



