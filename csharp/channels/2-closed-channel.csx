#! "netcoreapp3.0"
using System.Threading.Channels;

var channel = Channel.CreateUnbounded<string>();

var result = channel.Writer.TryWrite("hello");
Console.WriteLine($"Able to write to channel? {result}");

channel.Writer.Complete();
System.Console.WriteLine(channel);

result = channel.Writer.TryWrite("hello again");
Console.WriteLine($"Able to write to channel? {result}");

// Can try to WriteAsync but nothing will be written
channel.Writer.WriteAsync("hello again").GetAwaiter();

result = channel.Reader.TryRead(out var message);
Console.WriteLine($"Able to read message? {result}");
Console.WriteLine($"Message: {message}");
