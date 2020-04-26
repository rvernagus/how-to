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
        await channel.Writer.WaitToWriteAsync();
        if (!channel.Writer.TryWrite($"{n}"))
            Console.WriteLine($"Could not write {n}");
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
        Task.Delay(1000).Wait();
    }
});

Task.WaitAll(consumer);
Console.WriteLine("Done!");
