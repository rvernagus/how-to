#! "netcoreapp3.0"
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Linq;

var channel = Channel.CreateBounded<string>(3);
long numProducersCompleted = 0;

Task CreateProducer(string name) =>
    Task.Run(async () =>
    {
        for (int n = 0; n < 10; n++)
        {
            Console.WriteLine($"[{name}] Writing #{n}");
            await channel.Writer.WriteAsync($"Hello #{n}");
            Task.Delay(500).Wait();
        }
        Interlocked.Increment(ref numProducersCompleted);
    });

var producer1 = CreateProducer("1");
var producer2 = CreateProducer("2");

_ = Task.Run(() =>
{
    do
    {
        Task.Delay(10).Wait();
    } while (Interlocked.Read(ref numProducersCompleted) < 2);
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
        Task.Delay(1000).Wait();
    }
});

consumer.Wait();
Console.WriteLine("Done!");
