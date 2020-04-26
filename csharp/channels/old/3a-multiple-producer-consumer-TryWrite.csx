#! "netcoreapp3.0"
using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Linq;

var channel = Channel.CreateUnbounded<string>();
var rand = new Random();

var producers = Enumerable.Range(0, 4).Select(nProducer =>
    Task.Run(() =>
    {
        Enumerable.Range(0, 3).ToList().ForEach(async n =>
        {
            Console.WriteLine($"P{nProducer} => {n}");
            await channel.Writer.WaitToWriteAsync().ConfigureAwait(false);
            if (!channel.Writer.TryWrite($"P{nProducer} => {n}"))
                Console.WriteLine($"P{nProducer} could not write {n}");

            Task.Delay(rand.Next(2000)).Wait();
        });
    })
).ToArray();

var consumers = Enumerable.Range(0, 4).Select(nConsumer =>
    Task.Run(async () =>
    {
        while (await channel.Reader.WaitToReadAsync())
        {
            if (channel.Reader.TryRead(out var message))
            {
                Console.WriteLine($"C{nConsumer} <= {message}");
            }
            Task.Delay(rand.Next(2000)).Wait();
        }
    })
).ToArray();

_ = Task.WhenAll(producers).ContinueWith(_ => channel.Writer.Complete());
Task.WaitAll(consumers);
Console.WriteLine("Done!");
