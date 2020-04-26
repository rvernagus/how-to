#! "netcoreapp3.0"
#r "nuget: System.Threading.Tasks.Dataflow, 4.10.0"
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

var bufferBlock = new BufferBlock<int>();

var options = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 };

var actionBlock = new ActionBlock<int>(n =>
{
    Thread.Sleep(1200);
    Console.WriteLine($"  Received {n}...");
}, options);

_ = Task.Run(() =>
{
    var i = 0;
    while (true)
    {
        if (i > 10) bufferBlock.Complete();
        Thread.Sleep(600);
        i += 1;
        Console.WriteLine($"Posting {i} : {bufferBlock.Post(i)}");
        Console.WriteLine($"  bufferBlock.Count: {bufferBlock.Count}");
        Console.WriteLine($"  actionBlock.InputCount: {actionBlock.InputCount}");
    }
});

var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

bufferBlock.LinkTo(actionBlock, linkOptions);
actionBlock.Completion.Wait();
