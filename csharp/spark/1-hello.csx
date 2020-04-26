#! "netcoreapp3.0"
#r "nuget: Microsoft.Spark, 0.6.0"
using Microsoft.Spark.Sql;

var spark = SparkSession
    .Builder()
    .AppName("hello")
    .GetOrCreate();
