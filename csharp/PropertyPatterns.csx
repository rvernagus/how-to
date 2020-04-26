public class Address
{
    public string State { get; set; }
}

public static decimal ComputeSalesTax(Address location, decimal salePrice) =>
    location switch
    {
        { State: "WA" } => salePrice * 0.06M,
        { State: "MN" } => salePrice * 0.75M,
        { State: "MI" } => salePrice * 0.05M,
        Address a when a.State.StartsWith("D") => salePrice * 0.04M,
        // other cases removed for brevity...
        _ => 0M
    };

public static decimal ComputeSalesTax2(object location, decimal salePrice) =>
    location switch
    {
        Address a => a.State switch
        {
            "WA" => salePrice * 0.06M,
            "MN" => salePrice * 0.75M,
            "MI" => salePrice * 0.05M,
            // other cases removed for brevity...
            _ => 0M
        },
        // { }  => "Unknown location type",
        // null => "Null location",
    };

var mi = new Address { State = "MI" };
Console.WriteLine(ComputeSalesTax(mi, 10.0M));
var da = new Address { State = "DA" };
Console.WriteLine(ComputeSalesTax(da, 10.0M));

Console.WriteLine(ComputeSalesTax2(mi, 10.0M));
