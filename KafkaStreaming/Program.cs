using KafkaStreaming.Twitter;
using System;

namespace KafkaStreaming
{
    class Program
    {
        static void Main(string[] args)
        {
            TwitterConnector twits = new TwitterConnector();
            twits.GetTwitters();
            twits.StartAsync();
            Console.ReadLine();
        }
    }
}
