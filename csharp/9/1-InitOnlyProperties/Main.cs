using System;

var p = new Person { FirstName="Ray", LastName="Vernagus" };
Console.WriteLine(p);

// Error
// p.FirstName = "Bob";

public class Person
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
}
