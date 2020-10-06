using System;

var p = new Person { FirstName="Ray", LastName="Vernagus" };
Console.WriteLine(p);

// Error
// p.FirstName = "Bob";

public class Person
{
    private readonly string firstName;
    private readonly string lastName;

    public string FirstName
    {
        get => firstName;
        init => firstName = (value ?? throw new ArgumentNullException(nameof(FirstName)));
    }

    public string LastName
    {
        get => lastName;
        init => lastName = (value ?? throw new ArgumentNullException(nameof(LastName)));
    }
}
