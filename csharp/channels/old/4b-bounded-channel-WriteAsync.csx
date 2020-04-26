#! "netcoreapp3.0"
using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Linq;

var channel = Channel.CreateBounded<string>(3);

var producer = Task.Run(() =>
{
    Enumerable.Range(0, 10).ToList().ForEach(async n =>
    {
        Console.WriteLine($"Attempting to write #{n}");
        await channel.Writer.WriteAsync($"{n}");
    });
    // channel.Writer.Complete();
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

_ = Task.WhenAll(producer).ContinueWith(_ => channel.Writer.Complete());
Task.WaitAll(consumer);
Console.WriteLine("Done!");
