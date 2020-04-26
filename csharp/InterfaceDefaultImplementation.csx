interface ICanGreet
{
    public void Greet()
    {
        Console.WriteLine("Hello!");
    }
}

public class DefaultGreeter : ICanGreet { }

public class DifferentGreeter : ICanGreet
{
    public void Greet()
    {
        Console.WriteLine("Aloha!");
    }
}

var greeter = new DefaultGreeter() as ICanGreet;
greeter.Greet();
greeter = new DifferentGreeter();
greeter.Greet();
