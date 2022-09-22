using KafkaStreaming.Kafka;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.Core.Streaming;
using Tweetinvi.Models;
using Tweetinvi.Streaming.V2;

namespace KafkaStreaming.Twitter
{
    public class TwitterConnector
    {
        public KafkaProducerHostService _kafkaProducer;

        public readonly ILogger<TwitterConnector> _logger;
        private readonly TwitterClient _userClient;
        private readonly ITweetStream _stream;
        private readonly ISampleStreamV2 _stream2;
        private readonly string API_KEY = "RmqvfOOUU1l69XvI4y1o55VcS";
        private readonly string API_KEY_SECRET = "vsRIQLbEbUwOunOs3hzO3kk6MYfZt8PmtRjlQpDxGtzBbMMfm7";
        private readonly string BEARER_TOKEN = "AAAAAAAAAAAAAAAAAAAAADj0hAEAAAAAzfC5yHEqCer4DzjLhSNT5zAYnd4%3DxlyEAoJmzvWKQZbPCWaICH9E9NBPpsQ1OWLKqTBjMbW9IXafOH ";
       // private readonly string ACCESS_TOKEN = "376817957-IP4vTcUlko52RUDfI2t7dDWyBnfytvbXbjIt2Lae";
       /// private readonly string ACCESS_TOKEN_SECRET = "xv1X4s3rFfN8o40iZ6JaDsFqoAQXg0rvVoz5ZSxC0bD9q";
        public TwitterConnector()
        {
            //var userCredentials = new TwitterCredentials(API_KEY, API_KEY_SECRET, ACCESS_TOKEN, ACCESS_TOKEN_SECRET);
            var userCredentials = new TwitterCredentials(API_KEY, API_KEY_SECRET, BEARER_TOKEN);
            _userClient = new TwitterClient(userCredentials);
            //_stream = _userClient.Streams.CreateTweetStream();
            _stream2 = _userClient.StreamsV2.CreateSampleStream();
            _kafkaProducer = new KafkaProducerHostService();
        }

        public  void GetTwitters()
        {
            int i = 0;
           _stream2.TweetReceived += (sender, args) =>
            {
                if (args.Tweet.Lang =="es")
                {
                    System.Console.WriteLine(args.Tweet.Text);
                    _kafkaProducer.ProduceMessage(args.Tweet.Text);
                    i++;
                    if (i>=5)
                    {
                        _stream2.StopStream();
                        _kafkaProducer.DisposeProducer();
                    }
                }                
            };
         }

        public async void StartAsync()
        {
            await _stream2.StartAsync();
        }
    }
}
