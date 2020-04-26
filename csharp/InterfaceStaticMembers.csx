interface IGreetable
{
    private static string greeting = "Default Greeting";

    static void SetGreeting(string newGreeting)
    {
        greeting = newGreeting;
    }

    public void Greet()
    {
        Console.WriteLine(greeting);
    }
}

public class DefaultGreeter : IGreetable {}

class CustomGreetingGreeter : IGreetable
{
    public CustomGreetingGreeter(string greeting)
    {
        IGreetable.SetGreeting(greeting);
    }
}

class OverrideGreeter : IGreetable
{
    public void Greet()
    {
        Console.WriteLine("I just do my own thang!");
    }
}

var defaultGreeter = new DefaultGreeter() as IGreetable;
var customGreeter = new CustomGreetingGreeter("Custom Greeting!") as IGreetable;
var overrideGreeter = new OverrideGreeter() as IGreetable;

defaultGreeter.Greet();
customGreeter.Greet();
overrideGreeter.Greet();
