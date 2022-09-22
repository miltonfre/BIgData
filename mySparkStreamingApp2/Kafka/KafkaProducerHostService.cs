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
    public class KafkaProducerHostService 
    {
        public readonly ILogger<KafkaProducerHostService> _logger;
        private IProducer<Null, string> _producer;

        public KafkaProducerHostService()
        {
            //_logger = LoggerFactory.Create();
            var config = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092"
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async void ProduceMessage(string msg)
        {
            //_logger.LogInformation(msg);
            int a = 0;
            await _producer.ProduceAsync("twits", new Message<Null, string>()
            {
                Value = msg
            });
        }
        public void FlushProducer()
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
        }
        public  void DisposeProducer()
        {
            _producer?.Dispose();
        }
        
    }
}
