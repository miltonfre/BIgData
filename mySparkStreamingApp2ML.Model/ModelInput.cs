// This file was auto-generated by ML.NET Model Builder. 

using Microsoft.ML.Data;

namespace MySparkStreamingApp2ML.Model
{
    public class ModelInput
    {
        [ColumnName("Sentiment"), LoadColumn(0)]
        public string Sentiment { get; set; }


        [ColumnName("SentimentText"), LoadColumn(1)]
        public string SentimentText { get; set; }


        [ColumnName("LoggedIn"), LoadColumn(2)]
        public string LoggedIn { get; set; }


    }
}
