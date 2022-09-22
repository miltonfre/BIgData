using KafkaStreaming.Twitter;
using System;

namespace KafkaStreaming
{
    class Program
    {
        static void Main(string[] args)
        {
            TwitterConnector twits = new TwitterConnector();
            // CreateHostBuilder(args).Build().Run();
            twits.GetTwitters();
            twits.StartAsync();
            Console.ReadLine();
        }
    }
}
