#! "netcoreapp3.0"
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Linq;

var channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions()
{
    SingleWriter = true,
    SingleReader = true
});

var producer = Task.Run(() =>
{
    Enumerable.Range(0, 10).ToList().ForEach(async n =>
    {
        Console.WriteLine($"Writing #{n}");
        await channel.Writer.WriteAsync($"Hello #{n}");
        Task.Delay(1000).Wait();
    });
    channel.Writer.Complete();
});

var consumer = Task.Run(async () =>
{
    while (await channel.Reader.WaitToReadAsync())
    {
        if (channel.Reader.TryRead(out var message))
        {
            Console.WriteLine(message);
        }
    }
});

consumer.Wait();
Console.WriteLine("Done!");
