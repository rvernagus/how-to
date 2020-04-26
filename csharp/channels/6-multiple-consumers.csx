#! "netcoreapp3.0"
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Linq;

var channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions()
{
    SingleWriter = false,
    SingleReader = false
});

Task CreateProducer(string name) =>
    Task.Run(() =>
    {
        Enumerable.Range(0, 10).ToList().ForEach(async n =>
        {
            Console.WriteLine($"[{name}] Writing #{n}");
            await channel.Writer.WriteAsync($"Hello #{n}");
            Task.Delay(1000).Wait();
        });
    });

var producer1 = CreateProducer("1");
var producer2 = CreateProducer("2");
_ = Task.WhenAll(producer1, producer2).ContinueWith(_ => channel.Writer.Complete());

Task CreateConsumer(string name) =>
    Task.Run(async () =>
    {
        while (await channel.Reader.WaitToReadAsync())
        {
            if (channel.Reader.TryRead(out var message))
            {
                Console.WriteLine($"[{name}] {message}");
            }
        }
    });

var consumer1 = CreateConsumer("1");
var consumer2 = CreateConsumer("2");

Task.WaitAll(consumer1, consumer2);
Console.WriteLine("Done!");
