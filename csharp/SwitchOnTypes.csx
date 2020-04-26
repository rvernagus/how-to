static string SwitchOnType(object thing) =>
    thing switch
    {
        string s => "thing is a string",
        int i    => "thing is an int",
        float f  => "thing is a float",
        { }      => "thing is a non-null object",
        null     => "thing is null"
    };

Console.WriteLine(SwitchOnType("HI"));
Console.WriteLine(SwitchOnType(1));
Console.WriteLine(SwitchOnType(1.1f));
Console.WriteLine(SwitchOnType(DateTime.Now));
Console.WriteLine(SwitchOnType(null));
