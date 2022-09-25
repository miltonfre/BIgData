using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Spark.Sql;
using Microsoft.Spark.Sql.Streaming;

using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;
using MySparkStreamingApp2ML.Model;

using static Microsoft.Spark.Sql.Functions;
using Tweetinvi.Core.Extensions;

namespace mySparkStreamingApp2
{
    class Program
    {
        static void Main(string[] args)
        {
             // runSparkKafka(args);
           runSparkKafkaStreamML(args);
            //runSparkKafkaStream(args);
            //wordCount(args);
        }


        private static void runSparkKafkaStreamML(string[] args)
        {
            SparkSession spark = SparkSession
                                .Builder()
                                .AppName(".NET for Apache Spark Sentiment Analysis")
                                .GetOrCreate();

            DataFrame df = spark
                .ReadStream()
                .Format("kafka")
                .Option("kafka.bootstrap.servers", "localhost:9092")
                .Option("subscribe", "twits")
                .Option("startingOffsets", "earliest")
                .Option("failOnDataLoss", "false")
                .Load().SelectExpr("CAST(value AS STRING)");

            System.Console.WriteLine("PrintSchema-----------------------------------------");
            df.CreateOrReplaceTempView("twits");
            df.PrintSchema();
            var TwitOnString = df.SelectExpr("CAST(value AS STRING)");
            

            TwitOnString.CreateOrReplaceTempView("twits");
            System.Console.WriteLine("PrintSchema-----------------------------------------");
            TwitOnString.PrintSchema();


           // TwitOnString.Show();
            spark.Udf()
                        .Register<string, bool>("MLudf", Predict);

            // Use Spark SQL to call ML.NET UDF
            // Display results of sentiment analysis on reviews
            df.CreateOrReplaceTempView("twits");
            DataFrame sqlDf = spark.Sql("SELECT value, MLudf(value) FROM twits");
            // sqlDf.Show();

            // Print out first 20 rows of data
            // Prevent data getting cut off by setting truncate = 0
            var query = sqlDf
                .WriteStream()
                .OutputMode("Append")
                .Format("console")
               .Start();
            query.AwaitTermination();

            spark.Stop();
        }

        private static void runSparkKafka(string[] args)
        {
            SparkSession spark = SparkSession
            .Builder()
            .AppName("Kafka_Dataset")
            .GetOrCreate();
            System.Console.WriteLine("SparkSession CREATED");

            DataFrame df = spark
                .Read()
                .Format("kafka")
                .Option("kafka.bootstrap.servers", "localhost:9092")
                .Option("subscribe", "twits")
                .Option("startingOffsets", "earliest")
                .Option("failOnDataLoss", "false")
                .Load();

            var TwitOnString = df.SelectExpr("CAST(value AS STRING)");
            var twits_tab = TwitOnString
                               .WithColumn("word", Explode(Split(Col("value"), " ")))
                               .GroupBy("word")
                               .Count()
                               .Sort()
                               .Filter(Col("word").Contains("#"));

            twits_tab.Show();
            System.Console.WriteLine("COMPLETED");
            spark.Stop();

        }
        private static void runSparkKafkaStream(string[] args)
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

            var TwitOnString = df.SelectExpr("CAST(value AS STRING)");

            var twits_tab = TwitOnString
                               .WithColumn("word", Explode(Split(Col("value"), " ")))
                               .GroupBy("word");
            
            var query = TwitOnString
                .WriteStream()
                .OutputMode("Append")
                .Format("console")
               .Start();
            query.AwaitTermination();

            spark.Stop();
        }

        /// <summary>
        /// example method to count words
        /// 
        /// </summary>
        /// <param name="args"></param>
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


        private static bool Predict(string text)
        {
            var input = new ModelInput { SentimentText = text };
            return Convert.ToBoolean(Convert.ToInt16(ConsumeModel.Predict(input).Prediction)) ;
        }
    }
}
