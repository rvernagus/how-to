using System;

// Records implement value equality
var p1 = new Person { FirstName="Ray", LastName="Vernagus" };
var p2 = new Person { FirstName="Ray", LastName="Vernagus" };
Console.WriteLine(p1 == p2);

PersonRecord p3 = new("Ray", "Vernagus");
PersonRecord p4 = new("Ray", "Vernagus");
Console.WriteLine(p3 == p4);

// with expressions
Console.WriteLine(p3 with { LastName = "Smith" });

// Comes with deconstructor
var (firstName, lastName) = new PersonRecord2("Ray", "Vernagus");
Console.WriteLine((firstName, lastName));

public class Person
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
}

public record PersonRecord
{
    // Records can have constructors
    public PersonRecord(string firstName, string lastName) =>
        (FirstName, LastName) = (firstName, lastName);

    // Default behavior in with expressions can be overridden
    protected PersonRecord(PersonRecord original)
    {
        Console.WriteLine("  ! This is used in with expressions");
        FirstName = original.FirstName;
        LastName = original.LastName;
    }

    public string FirstName { get; init; }
    // Shorthand version of above
    public string LastName;
}

// Records have a one-line declaration syntax
public record PersonRecord2(string FirstName, string LastName);
