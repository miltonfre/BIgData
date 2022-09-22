using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Spark.Sql;
using Microsoft.Spark.Sql.Streaming;

using static Microsoft.Spark.Sql.Functions;

namespace mySparkStreamingApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            runSparkKafka(args);
            //wordCount(args);
        }
        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, collection) =>
        {
            //collection.AddHostedService<KafkaConsumerHostService>();
            // collection.AddHostedService<KafkaProducerHostService>();
        });

        private static void runSparkKafka(string[] args)
        {
            //The Spark Session is the entry point to programming Spark with the Dataset and DataFrame API.
            //allows you to access Spark and DataFrame functionality throughout your program.
            SparkSession spark = SparkSession
             .Builder()
             .AppName("Kafka_streaming")
             .GetOrCreate();
            System.Console.WriteLine("SparkSession CREATED");
            //used to read streaming data in as a DataFrame
            //host and port information to tell your Spark app where to expect its streaming data.
            DataFrame df = spark
                .ReadStream()
                .Format("kafka")
                .Option("kafka.bootstrap.servers", "localhost:9092")
                .Option("subscribe", "twits")
                .Option("startingOffsets", "earliest")
                .Option("failOnDataLoss", "false")
                .Load();
            System.Console.WriteLine("df LOADED");
            //   var query = df.WriteStream()
            //.Format("console")
            //.Start();

            df.SelectExpr("CAST(value AS STRING)");
            System.Console.WriteLine("CASTING VALUE");

            DataFrame words = df
                .Select(Explode(Split(df["value"], " "))
                    .Alias("word"));
            DataFrame wordCounts = words.GroupBy("word").Count();
            System.Console.WriteLine("WORDS COUNTED");

            //wordCounts.Show();
            StreamingQuery query = wordCounts
                .WriteStream()
                .OutputMode("complete")
                .Format("console")
                .Start();
            System.Console.WriteLine("COMPLETED");

            query.AwaitTermination(3000); 
            //df.SelectExpr("CAST(key AS STRING)", "CAST(value AS STRING)");
            // df.Show();

            // Stop Spark session
            spark.Stop();
            //.As(String, String);

            //This UDF processes each string it receives from the netcat terminal to produce an array that includes the original string (contained in str), followed by the original string concatenated with the length of the original string.
            //Func<Column, Column> udfArray =
            //                        Udf<string, string[]>((str) => new string[] { str, $"{str} {str.Length}" });

            //DataFrame arrayDF = lines.Select(Explode(udfArray(lines["value"])));

            //StreamingQuery query = arrayDF
            //                        .WriteStream()
            //                        .Format("console")
            //                        .Start();
            //query.AwaitTermination();
        }

        private static void wordCount(string[] args)
        {
            Console.WriteLine("Hello World!");

            // Create Spark session
            SparkSession spark =
                SparkSession
                    .Builder()
                    .AppName("word_count_sample")
                    .GetOrCreate();

            // Create initial DataFrame
            string filePath = args[0];
            DataFrame dataFrame = spark.Read().Text(filePath);

            //Count words
            DataFrame words =
                dataFrame
                    .Select(Split(Col("value"), " ").Alias("words"))
                    .Select(Explode(Col("words")).Alias("word"))
                    .GroupBy("word")
                    .Count()
                    .OrderBy(Col("count").Desc());

            // Display results
            words.Show();

            // Stop Spark session
            spark.Stop();
        }

        private void runSpark(string[] args)
        {
            // Default to running on localhost:9999
            string hostname = "localhost";
            int port = 9092;
            //The Spark Session is the entry point to programming Spark with the Dataset and DataFrame API.
            //allows you to access Spark and DataFrame functionality throughout your program.
            SparkSession spark = SparkSession
             .Builder()
             .AppName("Streaming example with a UDF")
             .GetOrCreate();

            //used to read streaming data in as a DataFrame
            //host and port information to tell your Spark app where to expect its streaming data.
            DataFrame lines = spark
                .ReadStream()
                .Format("socket")
                .Option("host", hostname)
                .Option("port", port)
                .Load();


            //This UDF processes each string it receives from the netcat terminal to produce an array that includes the original string (contained in str), followed by the original string concatenated with the length of the original string.
            Func<Column, Column> udfArray =
                                    Udf<string, string[]>((str) => new string[] { str, $"{str} {str.Length}" });

            DataFrame arrayDF = lines.Select(Explode(udfArray(lines["value"])));

            StreamingQuery query = arrayDF
                                    .WriteStream()
                                    .Format("console")
                                    .Start();
            query.AwaitTermination();

            //Console.WriteLine("Hello World!");

            //// Create Spark session
            //SparkSession spark =
            //    SparkSession
            //        .Builder()
            //        .AppName("word_count_sample")
            //        .GetOrCreate();

            //// Create initial DataFrame
            //string filePath = args[0];
            //DataFrame dataFrame = spark.Read().Text(filePath);

            ////Count words
            //DataFrame words =
            //    dataFrame
            //        .Select(Split(Col("value"), " ").Alias("words"))
            //        .Select(Explode(Col("words")).Alias("word"))
            //        .GroupBy("word")
            //        .Count()
            //        .OrderBy(Col("count").Desc());

            //// Display results
            //words.Show();

            //// Stop Spark session
            //spark.Stop();
        }
    }
}
