#r "nuget:Microsoft.Data.Analysis,0.2.0"
using Microsoft.Data.Analysis;

var dateTimes = new PrimitiveDataFrameColumn<DateTime>("DateTimes"); // Default length is 0.
var ints = new PrimitiveDataFrameColumn<int>("Ints", 3); // Makes a column of length 3. Filled with nulls initially
var strings = new StringDataFrameColumn("Strings", 3); // Makes a column of length 3. Filled with nulls initially

// Append 3 values to dateTimes
dateTimes.Append(DateTime.Parse("2019/01/01"));
dateTimes.Append(DateTime.Parse("2019/01/01"));
dateTimes.Append(DateTime.Parse("2019/01/02"));

var df = new DataFrame(dateTimes, ints, strings ); // This will throw if the columns are of different lengths
Console.WriteLine(df);

static void PrintDataFrame(DataFrame dataFrame)
{
    var header = new List<string>();
    header.Add("index");
    header.AddRange(dataFrame.Columns.Select(c => c.Name));
    var rows = new List<List<string>>();
    var take = 20;
    for (var i = 0; i < Math.Min(take, dataFrame.Rows.Count); i++)
    {
        var values = new List<string>();
        values.Add($"{i}");
        foreach (var obj in dataFrame.Rows[i])
        {
            values.Add($"{obj}");
        }
        rows.Add(values);
    }
    Console.WriteLine(string.Join("\t", header));
    foreach (var line in rows)
    {
        Console.WriteLine(string.Join("\t", line));
    }
    Console.WriteLine("\n");
}

PrintDataFrame(df);

// To change a value directly through df
df[0, 1] = 10; // 0 is the rowIndex, and 1 is the columnIndex. This sets the 0th value in the Ints columns to 10
PrintDataFrame(df);

// Modify ints and strings columns by indexing
ints[1] = 100;
strings[1] = "Foo!";
PrintDataFrame(df);

// Indexing can throw when types don't match.
// ints[1] = "this will throw because I am a string";  
// Info can be used to figure out the type of data in a column. 
Console.WriteLine(df.Info());

// Add 5 to ints through the DataFrame
df["Ints"].Add(5, inPlace: true);
PrintDataFrame(df);

// We can also use binary operators. Binary operators produce a copy, so assign it back to our Ints column 
df["Ints"] = (ints / 5) * 100;
PrintDataFrame(df);

// Fill null values
df["Ints"].FillNulls(-1, inPlace: true);
df["Strings"].FillNulls("Bar", inPlace: true);
PrintDataFrame(df);

// Access rows by index
var row0 = df.Rows[0];
Console.WriteLine(row0);

// Filter rows based on equality
var boolFilter = df["Strings"].ElementwiseEquals("Bar");
Console.WriteLine(boolFilter);
var filtered = df.Filter(boolFilter);
PrintDataFrame(filtered);

// Sort our dataframe using the Ints column
var sorted = df.Sort("Ints");
PrintDataFrame(sorted);

// GroupBy 
var groupBy = df.GroupBy("DateTimes");
var groupCounts = groupBy.Count();
PrintDataFrame(groupCounts);

// Alternatively find the sum of the values in each group in Ints
var intGroupSum = groupBy.Sum("Ints");
PrintDataFrame(intGroupSum);
