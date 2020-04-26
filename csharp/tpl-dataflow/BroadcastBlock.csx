#! "netcoreapp3.0"
#r "nuget: System.Threading.Tasks.Dataflow, 4.10.0"
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

var broadcastBlock = new BroadcastBlock<int>(x => x);

var actionBlock1 = new ActionBlock<int>(n =>
    Console.WriteLine($"Consumer1 received {n}..."));

var actionBlock2 = new ActionBlock<int>(n =>
    Console.WriteLine($"Consumer2 received {n}..."));

_ = Task.Run(() =>
{
    var i = 0;
    while (true)
    {
        Thread.Sleep(100);
        if (i > 10) broadcastBlock.Complete();
        i += 1;
        broadcastBlock.Post(i);
    }
});

broadcastBlock.LinkTo(actionBlock1);
broadcastBlock.LinkTo(actionBlock2);
broadcastBlock.Completion.Wait();
