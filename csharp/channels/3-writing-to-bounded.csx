#! "netcoreapp3.0"
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

// Try with Unbounded versus Bounded
// var channel = Channel.CreateUnbounded<int>();
var channel = Channel.CreateBounded<int>(6);

Parallel.ForEach(Enumerable.Range(0, 4), n1 =>
{
    var couldNotBeAdded = new ConcurrentQueue<int>();

    Parallel.ForEach(Enumerable.Range(0, 4), n2 =>
    {
        if (!channel.Writer.TryWrite(n1 * 4 + n2))
        {
            couldNotBeAdded.Enqueue(n1 * 4 + n2);
        }
    });

    Console.WriteLine($"Unsent messages [{n1}]: {string.Join(",", couldNotBeAdded)}");
});

_ = Task.Delay(1000).ContinueWith(_ => channel.Writer.Complete());

Console.WriteLine("Received messages:");
await foreach (var message in channel.Reader.ReadAllAsync())
{
    Console.WriteLine(message);
}
