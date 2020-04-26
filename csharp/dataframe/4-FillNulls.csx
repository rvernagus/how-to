#r "nuget:Microsoft.Data.Analysis,0.2.0"
using Microsoft.Data.Analysis;

var random = new Random();
int? GetValue()
{
    var nullChance = random.Next(0, 10);
    var randInt = random.Next(1, 100);
    return nullChance switch
    {
        1 => (int?)null,
        _ => randInt,
    };
}

var col1 = new PrimitiveDataFrameColumn<int>("Col1");
var col2 = new PrimitiveDataFrameColumn<int>("Col2");
var df = new DataFrame(col1, col2);


df.Append(new object[] { null, null });
Enumerable.Range(0, 10)
    .ToList()
    .ForEach(_ => df.Append(new object[] { GetValue(), GetValue() }));

Console.WriteLine(df);

var dfFilled = df.FillNulls(-1);
Console.WriteLine(dfFilled);
