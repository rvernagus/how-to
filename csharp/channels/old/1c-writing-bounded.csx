#! "netcoreapp3.0"
using System.Threading.Channels;

var channel = Channel.CreateBounded<string>(3);

channel.Writer.TryWrite("1");
channel.Writer.TryWrite("2");
channel.Writer.TryWrite("3");
Console.WriteLine($"4 was written? {channel.Writer.TryWrite("4")}");

string message;
channel.Reader.TryRead(out message);
Console.WriteLine(message);
channel.Reader.TryRead(out message);
Console.WriteLine(message);
channel.Reader.TryRead(out message);
Console.WriteLine(message);

channel.Reader.TryRead(out message);
Console.WriteLine(message);
Console.WriteLine($"message is null? {message is null}");
channel.Reader.TryRead(out message);
Console.WriteLine(message);

Console.WriteLine($"4 was written? {channel.Writer.TryWrite("4")}");

channel.Reader.TryRead(out message);
Console.WriteLine(message);
