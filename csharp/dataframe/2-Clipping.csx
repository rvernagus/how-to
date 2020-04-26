#r "nuget:Microsoft.Data.Analysis,0.2.0"
using Microsoft.Data.Analysis;

var age = new PrimitiveDataFrameColumn<int>("Age");
var rank = new PrimitiveDataFrameColumn<int>("Rank");
var df = new DataFrame(age, rank);

foreach (var n in Enumerable.Range(1, 10))
{
    df.Append(new object[] { n*2, n });
}

var dfClipped = df.Clip(4, 10);
Console.WriteLine(dfClipped);
