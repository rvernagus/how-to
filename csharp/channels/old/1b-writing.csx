#! "netcoreapp3.0"
using System.Threading.Channels;

var channel = Channel.CreateUnbounded<string>();

channel.Writer.TryWrite("1");
channel.Writer.TryWrite("2");
channel.Writer.TryWrite("3");
channel.Writer.TryWrite("4");

string message;
channel.Reader.TryRead(out message);
Console.WriteLine(message);
channel.Reader.TryRead(out message);
Console.WriteLine(message);
channel.Reader.TryRead(out message);
Console.WriteLine(message);
channel.Reader.TryRead(out message);
Console.WriteLine(message);

// Default value is returned if there is nothing in the channel
channel.Reader.TryRead(out message);
Console.WriteLine(message);
Console.WriteLine(message is null);
channel.Reader.TryRead(out message);
Console.WriteLine(message);
