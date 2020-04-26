#r "nuget: Microsoft.ML, 1.4.0"
#r "nuget:Microsoft.Data.Analysis,0.2.0"
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;
using System.Net;
using Microsoft.ML.Transforms;

class InputData
{
    [ColumnName("PixelValues")]
    [VectorType(64)]
    public float[] PixelValues;

    [LoadColumn(64)]
    public float Number;
}

class OutPutData
{
    [ColumnName("Score")]
    public float[] Score;
}

class SampleMNISTData
{
    internal static readonly InputData MNIST1 = new InputData()
    {
        PixelValues = new float[] { 0, 0, 0, 0, 14, 13, 1, 0, 0, 0, 0, 5, 16, 16, 2, 0, 0, 0, 0, 14, 16, 12, 0, 0, 0, 1, 10, 16, 16, 12, 0, 0, 0, 3, 12, 14, 16, 9, 0, 0, 0, 0, 0, 5, 16, 15, 0, 0, 0, 0, 0, 4, 16, 14, 0, 0, 0, 0, 0, 1, 13, 16, 1, 0 }
    }; //num 1


    internal static readonly InputData MNIST2 = new InputData()
    {
        PixelValues = new float[] { 0, 0, 1, 8, 15, 10, 0, 0, 0, 3, 13, 15, 14, 14, 0, 0, 0, 5, 10, 0, 10, 12, 0, 0, 0, 0, 3, 5, 15, 10, 2, 0, 0, 0, 16, 16, 16, 16, 12, 0, 0, 1, 8, 12, 14, 8, 3, 0, 0, 0, 0, 10, 13, 0, 0, 0, 0, 0, 0, 11, 9, 0, 0, 0 }
    };//num 7

    internal static readonly InputData MNIST3 = new InputData()
    {
        PixelValues = new float[] { 0, 0, 6, 14, 4, 0, 0, 0, 0, 0, 11, 16, 10, 0, 0, 0, 0, 0, 8, 14, 16, 2, 0, 0, 0, 0, 1, 12, 12, 11, 0, 0, 0, 0, 0, 0, 0, 11, 3, 0, 0, 0, 0, 0, 0, 5, 11, 0, 0, 0, 1, 4, 4, 7, 16, 2, 0, 0, 7, 16, 16, 13, 11, 1 }
    };// num9

    
}

if (!File.Exists("optdigits-train.csv"))
{
    using var client = new WebClient();
    client.DownloadFile("http://archive.ics.uci.edu/ml/machine-learning-databases/optdigits/optdigits.tra", "optdigits-train.csv");
}

if (!File.Exists("optdigits-test.csv"))
{
    using var client = new WebClient();
    client.DownloadFile("http://archive.ics.uci.edu/ml/machine-learning-databases/optdigits/optdigits.tes", "optdigits-test.csv");
}

var mlContext = new MLContext();

// STEP 1: Common data loading configuration
var trainData = mlContext.Data.LoadFromTextFile(
    path: "optdigits-train.csv",
    columns : new[] 
    {
        new TextLoader.Column(nameof(InputData.PixelValues), DataKind.Single, 0, 63),
        new TextLoader.Column("Number", DataKind.Single, 64)
    },
    hasHeader : false,
    separatorChar : ','
);

                
var testData = mlContext.Data.LoadFromTextFile(
    path: "optdigits-test.csv",
    columns: new[]
    {
        new TextLoader.Column(nameof(InputData.PixelValues), DataKind.Single, 0, 63),
        new TextLoader.Column("Number", DataKind.Single, 64)
    },
    hasHeader: false,
    separatorChar: ','
);

// STEP 2: Common data process configuration with pipeline data transformations
// Use in-memory cache for small/medium datasets to lower training time. Do NOT use it (remove .AppendCacheCheckpoint()) when handling very large datasets.
var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Number", keyOrdinality: ValueToKeyMappingEstimator.KeyOrdinality.ByValue).
    Append(mlContext.Transforms.Concatenate("Features", nameof(InputData.PixelValues)).AppendCacheCheckpoint(mlContext));

// STEP 3: Set the training algorithm, then create and config the modelBuilder
var trainer = mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName: "Label", featureColumnName: "Features");
var trainingPipeline = dataProcessPipeline.Append(trainer).Append(mlContext.Transforms.Conversion.MapKeyToValue("Number","Label"));

// STEP 4: Train the model fitting to the DataSet

Console.WriteLine("=============== Training the model ===============");
var trainedModel = trainingPipeline.Fit(trainData);

Console.WriteLine("===== Evaluating Model's accuracy with Test data =====");
var predictions = trainedModel.Transform(testData);
var metrics = mlContext.MulticlassClassification.Evaluate(data:predictions, labelColumnName:"Number", scoreColumnName:"Score");

Console.WriteLine($"************************************************************");
Console.WriteLine($"*    Metrics for {trainer} multi-class classification model   ");
Console.WriteLine($"*-----------------------------------------------------------");
Console.WriteLine($"    AccuracyMacro = {metrics.MacroAccuracy:0.####}, a value between 0 and 1, the closer to 1, the better");
Console.WriteLine($"    AccuracyMicro = {metrics.MicroAccuracy:0.####}, a value between 0 and 1, the closer to 1, the better");
Console.WriteLine($"    LogLoss = {metrics.LogLoss:0.####}, the closer to 0, the better");
Console.WriteLine($"    LogLoss for class 1 = {metrics.PerClassLogLoss[0]:0.####}, the closer to 0, the better");
Console.WriteLine($"    LogLoss for class 2 = {metrics.PerClassLogLoss[1]:0.####}, the closer to 0, the better");
Console.WriteLine($"    LogLoss for class 3 = {metrics.PerClassLogLoss[2]:0.####}, the closer to 0, the better");
Console.WriteLine($"************************************************************");

mlContext.Model.Save(trainedModel, trainData.Schema, "MNIST-Model.zip");

Console.WriteLine($"The model is saved to MNIST-Model.zip");




var loadedModel = mlContext.Model.Load("MNIST-Model.zip", out var modelInputSchema);

// Create prediction engine related to the loaded trained model
var predEngine = mlContext.Model.CreatePredictionEngine<InputData, OutPutData>(loadedModel);

var resultprediction1 = predEngine.Predict(SampleMNISTData.MNIST1);

Console.WriteLine($"Actual: 1     Predicted probability:       zero:  {resultprediction1.Score[0]:0.####}");
Console.WriteLine($"                                           One :  {resultprediction1.Score[1]:0.####}");
Console.WriteLine($"                                           two:   {resultprediction1.Score[2]:0.####}");
Console.WriteLine($"                                           three: {resultprediction1.Score[3]:0.####}");
Console.WriteLine($"                                           four:  {resultprediction1.Score[4]:0.####}");
Console.WriteLine($"                                           five:  {resultprediction1.Score[5]:0.####}");
Console.WriteLine($"                                           six:   {resultprediction1.Score[6]:0.####}");
Console.WriteLine($"                                           seven: {resultprediction1.Score[7]:0.####}");
Console.WriteLine($"                                           eight: {resultprediction1.Score[8]:0.####}");
Console.WriteLine($"                                           nine:  {resultprediction1.Score[9]:0.####}");
Console.WriteLine();

var resultprediction2 = predEngine.Predict(SampleMNISTData.MNIST2);

Console.WriteLine($"Actual: 7     Predicted probability:       zero:  {resultprediction2.Score[0]:0.####}");
Console.WriteLine($"                                           One :  {resultprediction2.Score[1]:0.####}");
Console.WriteLine($"                                           two:   {resultprediction2.Score[2]:0.####}");
Console.WriteLine($"                                           three: {resultprediction2.Score[3]:0.####}");
Console.WriteLine($"                                           four:  {resultprediction2.Score[4]:0.####}");
Console.WriteLine($"                                           five:  {resultprediction2.Score[5]:0.####}");
Console.WriteLine($"                                           six:   {resultprediction2.Score[6]:0.####}");
Console.WriteLine($"                                           seven: {resultprediction2.Score[7]:0.####}");
Console.WriteLine($"                                           eight: {resultprediction2.Score[8]:0.####}");
Console.WriteLine($"                                           nine:  {resultprediction2.Score[9]:0.####}");
Console.WriteLine();

var resultprediction3 = predEngine.Predict(SampleMNISTData.MNIST3);

Console.WriteLine($"Actual: 9     Predicted probability:       zero:  {resultprediction3.Score[0]:0.####}");
Console.WriteLine($"                                           One :  {resultprediction3.Score[1]:0.####}");
Console.WriteLine($"                                           two:   {resultprediction3.Score[2]:0.####}");
Console.WriteLine($"                                           three: {resultprediction3.Score[3]:0.####}");
Console.WriteLine($"                                           four:  {resultprediction3.Score[4]:0.####}");
Console.WriteLine($"                                           five:  {resultprediction3.Score[5]:0.####}");
Console.WriteLine($"                                           six:   {resultprediction3.Score[6]:0.####}");
Console.WriteLine($"                                           seven: {resultprediction3.Score[7]:0.####}");
Console.WriteLine($"                                           eight: {resultprediction3.Score[8]:0.####}");
Console.WriteLine($"                                           nine:  {resultprediction3.Score[9]:0.####}");
Console.WriteLine();
