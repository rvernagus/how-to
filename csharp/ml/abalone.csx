#r "nuget: Microsoft.ML, 1.4.0"
#r "nuget:Microsoft.Data.Analysis,0.2.0"
using Microsoft.Data.Analysis;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;
using System.Linq;
using System.Net;

public class AbaloneData
{
    public string Sex { get; set; }

    public float Length { get; set; }

    public float Diameter { get; set; }

    public float Height { get; set; }

    public float WholeWeight { get; set; }

    public float ShuckedWeight { get; set; }

    public float VisceraWeight { get; set; }

    public float ShellWeight { get; set; }

    [ColumnName("Label")]
    public Single Rings { get; set; }
}

public class AbalonePrediction
{
    public Single Label { get; set; }

    public Single Score { get; set; }

    public override string ToString() =>
        $"Label: {Label}, Score: {Score}";
}

if (!File.Exists("abalone.data"))
{
    using var client = new WebClient();
    client.DownloadFile("https://archive.ics.uci.edu/ml/machine-learning-databases/abalone/abalone.data", "abalone.data");
}

var columnNames = new[] { "sex", "length", "diameter", "height", "whole_weight", "shucked_weight", "viscera_weight", "shell_weight", "rings" };
var df = DataFrame.LoadCsv("abalone.data", header: false, columnNames: columnNames);

// Count nulls
df.Columns
    .Select(c => (columnName: c.Name, nullCount: c.NullCount))
    .ToList()
    .ForEach(x => Console.WriteLine(x));


// How is the target distributed?
df.GroupBy("rings")
    .Count("rings", "sex")
    .Rows
    .Select(count => $"rings={count[0], 2}\tn={count[1]}")
    .ToList()
    .ForEach(s => Console.WriteLine(s));

var data = df.Rows
    .Select(row =>
        new AbaloneData
        {
            Sex = (string)row[0],
            Length = (float)row[1],
            Diameter = (float)row[2],
            Height = (float)row[3],
            WholeWeight = (float)row[4],
            ShuckedWeight = (float)row[5],
            VisceraWeight = (float)row[6],
            ShellWeight = (float)row[7],
            Rings = (float)row[8]
        }
    );

var context = new MLContext();

var allData = context.Data.LoadFromEnumerable<AbaloneData>(data);

allData = context.Data.ShuffleRows(allData);

var splitData = context.Data.TrainTestSplit(allData, testFraction: 0.2);
var (trainData, testData) = (splitData.TrainSet, splitData.TestSet);

var featureColumns = new[]
{
    nameof(AbaloneData.Sex), nameof(AbaloneData.Length), nameof(AbaloneData.Diameter), nameof(AbaloneData.Height),
    nameof(AbaloneData.WholeWeight), nameof(AbaloneData.ShuckedWeight), nameof(AbaloneData.VisceraWeight),
    nameof(AbaloneData.ShellWeight)
};

var transformer = context
    .Transforms.Categorical.OneHotEncoding(nameof(AbaloneData.Sex))
    .Append(context.Transforms.Concatenate("Features", featureColumns))
    .Append(context.Transforms.NormalizeLpNorm("FeaturesNorm", "Features"))
    .Fit(allData);

var estimator = context.Regression.Trainers.LbfgsPoissonRegression(featureColumnName: "FeaturesNorm");

var transformedTrainData = transformer.Transform(trainData);
var cvResults = context.Regression.CrossValidate(transformedTrainData, estimator, numberOfFolds: 5);
var cvResult = cvResults
    .OrderByDescending(x => x.Metrics.RSquared)
    .First();

var transformedTestData = transformer.Transform(testData);
var predictions = cvResult.Model.Transform(transformedTestData);
var metrics = context.Regression.Evaluate(predictions);

Console.WriteLine("Test Results");
Console.WriteLine($"Mean Absolute Error: {metrics.MeanAbsoluteError}");
Console.WriteLine($"Mean Squared Error: {metrics.MeanSquaredError}");
Console.WriteLine($"Root Mean Squared Error: {metrics.RootMeanSquaredError}");
Console.WriteLine($"R-squared: {metrics.RSquared}");
Console.WriteLine("--------------------------------");
Console.WriteLine();

context.Data
    .CreateEnumerable<AbalonePrediction>(predictions, reuseRowObject: false)
    .Take(10)
    .ToList()
    .ForEach(Console.WriteLine);
