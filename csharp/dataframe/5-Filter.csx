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

var rowFilter = new PrimitiveDataFrameColumn<int>("Filter", new[] { 0, 2, 3, 6, 10 });
var dfFiltered = df.Filter(rowFilter);
Console.WriteLine(dfFiltered);

var boolFilter = new PrimitiveDataFrameColumn<bool>("Filter", new[] { true, false, true, true, false, false, true, false, false, false, true });
dfFiltered = df.Filter(boolFilter);
Console.WriteLine(dfFiltered);

var dfMasks = df.ElementwiseGreaterThan(50);
dfFiltered = df.Filter(dfMasks["Col1"] as PrimitiveDataFrameColumn<bool>);
Console.WriteLine(dfFiltered);
