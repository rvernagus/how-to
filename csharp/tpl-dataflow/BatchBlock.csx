#! "netcoreapp3.0"
#r "nuget: System.Threading.Tasks.Dataflow, 4.10.0"
using System;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

var batchBlock = new BatchBlock<int>(5);

var actionBlock = new ActionBlock<int[]>(x => Console.WriteLine($"Received {string.Join(",", x)}"));

_ = Task.Run(() =>
{
    var i = 0;
    while (true)
    {
        if (i > 15)  batchBlock.Complete();

        Thread.Sleep(100);
        i += 1;
        Console.WriteLine($"Posting {i}...");
        batchBlock.Post(i);
    }
});

batchBlock.LinkTo(actionBlock);
batchBlock.Completion.Wait();
