#r "nuget:Microsoft.Data.Analysis,0.2.0"
using Microsoft.Data.Analysis;

var col1 = new StringDataFrameColumn("First", 0);
var col2 = new StringDataFrameColumn("Last", 0);
var df = new DataFrame(col1, col2);

df.Append(new[] { "Ray", "Vernagus" });
df.Append(new[] { "Karenn", "Vernagus" });
df.Append(new[] { "Ari", "Vernagus" });
df.Append(new[] { "Rahm", "Vernagus" });

Console.WriteLine(df);

var prefixDf = df.AddPrefix("Name");
Console.WriteLine(prefixDf);

var suffixDf = df.AddSuffix("Name");
Console.WriteLine(suffixDf);
