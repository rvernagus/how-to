#! "netcoreapp3.0"
using System.Threading.Channels;

var channel = Channel.CreateUnbounded<string>();
var result = channel.Writer.TryWrite("hello");
Console.WriteLine(result);

channel.Writer.Complete();

while (await channel.Reader.WaitToReadAsync())
{
    if (channel.Reader.TryRead(out var message))
    {
        Console.WriteLine(message);
    }
}
