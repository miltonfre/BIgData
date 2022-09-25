# BIgData
Video: [https://github.com/miltonfre/BIgData](https://web.microsoftstream.com/video/ab71593f-ed0b-4738-ac8c-764d43b4a223)
This is the final project for CS523 course BIG DATA TECHNOLOGIES, 
Initial requiriments:
 - Spark Streaming Project
 - Spark and HBase/Hive Integration
 - Additional small research project
 - Video recording of project demo

Technologies used in this project:
SO: windows 10.
Kafka
.Net Core 3.1

1. Installing spark on windows: https://learn.microsoft.com/es-es/dotnet/spark/tutorials/get-started?tabs=windows
2. installing kafka on windows


  - KafkaStreaming:
  this projector manages the conections with twitter and send the data to kafka producer 
  
  - mySparkStreamingApp2:
    This project has the code for consumer and implements all spark functionality.
    
  - mySparkStreamingApp2ML.ConsoleApp:
    Its a project used to investigate and do some tests during the development stage, I decided keep the project but its not for the course project.
    
  - mySparkStreamingApp2ML.ConsoleApp
    The implementation for the Machine Learning code, here is for train, test, predict and model code. (sentiment analysis from twits)
    
  - SentimentRazor:
    Web application to execute the Machine learning and analyse phrases independenlty
    ![image](https://user-images.githubusercontent.com/5255854/192121196-43905172-226d-486a-995b-96014ba3429c.png)
    
    
    --run zookeeper
cd C:\bin\kafka_2.12-3.2.1
C:\bin\kafka_2.12-3.2.1\bin\windows\zookeeper-server-start.bat config/zookeeper.properties


--run kafka
cd C:\bin\kafka_2.12-3.2.1
C:\bin\kafka_2.12-3.2.1\bin\windows\kafka-server-start.bat config/server.properties

--Check topics on kafka
C:\bin\kafka_2.12-3.2.1\bin\windows\kafka-topics.bat --describe --topic twits --bootstrap-server localhost:9092

--read the evenst
C:\bin\kafka_2.12-3.2.1\bin\windows\kafka-console-consumer.bat --topic twits --from-beginning --bootstrap-server localhost:9092

-- kafka create topic
bin/kafka-topics.sh --create --topic topictest --bootstrap-server localhost:9092


    Run spark:
    cd D:\Personal\Master Maharishi\Classes\BigData\FInalProject\mySparkStreamingApp2
spark-submit --packages org.apache.spark:spark-sql-kafka-0-10_2.12:3.0.1 --class org.apache.spark.deploy.dotnet.DotnetRunner --master local bin\Debug\netcoreapp3.1\microsoft-spark-3-0_2.12-2.1.1.jar dotnet bin\Debug\netcoreapp3.1\mySparkStreamingApp2.dll

    
