#r "nuget: Microsoft.ML, 1.4.0"
#r "nuget:Microsoft.Data.Analysis,0.2.0"
using Microsoft.Data.Analysis;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;
using System.Linq;
using System.Net;

 public class AdultData
{
    public float Age { get; set; }

    public string WorkClass { get; set; }

    public float Fnlwgt { get; set; }

    public string Education { get; set; }

    public float EducationNum { get; set; }

    public string MaritalStatus { get; set; }

    public string Occupation { get; set; }

    public string Relationship { get; set; }

    public string Race { get; set; }

    public string Sex { get; set; }

    public float CapitalGain { get; set; }

    public float CapitalLoss { get; set; }

    public float HoursPerWeek { get; set; }

    public string NativeCountry { get; set; }

    [ColumnName("Label")]
    public string Target { get; set; }
}

if (!File.Exists("adult.data"))
{
    using var client = new WebClient();
    client.DownloadFile("https://archive.ics.uci.edu/ml/machine-learning-databases/adult/adult.data", "adult.data");
}

if (!File.Exists("adult.test"))
{
    using var client = new WebClient();
    client.DownloadFile("https://archive.ics.uci.edu/ml/machine-learning-databases/adult/adult.test", "adult.test");
}

var columnNames = new[]
{
    "age", "workclass", "fnlwgt", "education", "education_num", "marital_status", "occupation", "relationship",
    "race", "sex", "capital_gain", "capital_loss", "hours_per_week", "native_country", "target"
};

var dfTrain = DataFrame.LoadCsv("adult.data", header: false, columnNames: columnNames);
var dfTest = DataFrame.LoadCsv("adult.test", header: true, columnNames: columnNames);

dfTrain = dfTrain.DropNulls(options: DropNullOptions.All);
dfTest = dfTest.DropNulls(options: DropNullOptions.All);

// List columns and the type they were inferred to be
dfTrain.Columns
    .Select(column => (name: column.Name, type: column.DataType))
    .ToList()
    .ForEach(ctype => Console.WriteLine($"Column: {ctype.name}, Type: {ctype.type}"));

// Count of target values
dfTrain["target"]
    .GroupColumnValues<string>()
    .Select((kv, _) => $"{kv.Key, 6} n={kv.Value.Count}")
    .ToList()
    .ForEach(s => Console.WriteLine(s));

Func<DataFrameRow, AdultData> mapper = row => new AdultData
{
    Age = (float)row[0],
    WorkClass = (string)row[1],
    Fnlwgt = (float)row[2],
    Education = (string)row[3],
    EducationNum = (float)row[4],
    MaritalStatus = (string)row[5],
    Occupation = (string)row[6],
    Relationship = (string)row[7],
    Race = (string)row[8],
    Sex = (string)row[9],
    CapitalGain = (float)row[10],
    CapitalLoss = (float)row[11],
    HoursPerWeek = (float)row[12],
    NativeCountry = (string)row[13],
    Target = (string)row[14]
};

var trainRows = dfTrain.Rows.Select(mapper);
var testRows = dfTest.Rows.Select(mapper);

var context = new MLContext();

var trainData = context.Data.LoadFromEnumerable(trainRows);
trainData = context.Data.ShuffleRows(trainData);

var testData = context.Data.LoadFromEnumerable(testRows);
testData = context.Data.ShuffleRows(testData);

var featureColumns = new[]
{
    nameof(AdultData.Age), nameof(AdultData.WorkClass), nameof(AdultData.Fnlwgt), nameof(AdultData.Education),
    nameof(AdultData.EducationNum), nameof(AdultData.MaritalStatus), nameof(AdultData.Occupation), nameof(AdultData.Relationship),
    nameof(AdultData.Race), nameof(AdultData.Sex), nameof(AdultData.CapitalGain), nameof(AdultData.CapitalLoss),
    nameof(AdultData.HoursPerWeek), nameof(AdultData.NativeCountry)
};

var categoricalColumns = new[]
{
    nameof(AdultData.WorkClass), nameof(AdultData.Education), nameof(AdultData.MaritalStatus), nameof(AdultData.Occupation),
    nameof(AdultData.Relationship), nameof(AdultData.Race), nameof(AdultData.Sex), nameof(AdultData.NativeCountry)
};

var labelLookup = new Dictionary<string, bool>
{
    [" <=50K"] = false,
    [" <=50K."] = false,
    [" >50K"] = true,
    [" >50K."] = true
};

var chain = new EstimatorChain<Microsoft.ML.Transforms.OneHotEncodingTransformer>();
var transformer = categoricalColumns
    .Aggregate(chain, (pl, col) => pl.Append(context.Transforms.Categorical.OneHotEncoding(col)))
    .Append(context.Transforms.Conversion.MapValue("Label", labelLookup, "Label"))
    .Append(context.Transforms.Concatenate("Features", featureColumns))
    .Append(context.Transforms.NormalizeBinning("FeaturesNorm", "Features"))
    .Fit(trainData);

var estimator = context.BinaryClassification.Trainers.SdcaLogisticRegression(featureColumnName: "FeaturesNorm");

var transformedTrainData = transformer.Transform(trainData);
var cvResults = context.BinaryClassification.CrossValidate(transformedTrainData, estimator, numberOfFolds: 3);
var cvResult = cvResults
    .OrderByDescending(x => x.Metrics.Accuracy)
    .First();

var transformedTestData = transformer.Transform(testData);
var predictions = cvResult.Model.Transform(transformedTestData);
var metrics = context.BinaryClassification.Evaluate(predictions);

Console.WriteLine($"Accuracy: {metrics.Accuracy}");
Console.WriteLine($"Area Under Roc Curve: {metrics.AreaUnderRocCurve}");
Console.WriteLine($"F1 Score: {metrics.F1Score}");
Console.WriteLine("--------------------------------");
Console.WriteLine();
