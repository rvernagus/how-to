import os
import findspark
from pyspark.sql import SparkSession

os.environ["JAVA_HOME"] = "/usr/lib/jvm/java-8-openjdk-amd64"
os.environ["SPARK_HOME"] = "/home/vsonline/workspace/install/spark-2.4.4-bin-hadoop2.7"

findspark.init()

spark = SparkSession.builder.master("local[*]").getOrCreate()
sc = spark.sparkContext
