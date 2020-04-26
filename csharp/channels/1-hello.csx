#! "netcoreapp3.0"
using System.Threading.Channels;

var channel = Channel.CreateUnbounded<string>();

var result = channel.Writer.TryWrite("hello");
Console.WriteLine($"Able to write to channel? {result}");

result = channel.Reader.TryRead(out var message);
Console.WriteLine($"Able to read message? {result}");
Console.WriteLine($"Message: {message}");
