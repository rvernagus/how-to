#! "netcoreapp3.0"
#r "nuget: System.Threading.Tasks.Dataflow, 4.10.0"
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;


var cancellationSource = new CancellationTokenSource();
var options = new ExecutionDataflowBlockOptions { CancellationToken = cancellationSource.Token };

var actionBlock = new ActionBlock<int>(n =>
{
    Thread.Sleep(200);
    Console.WriteLine($"  Action on {n}...");
}, options);

_ = Task.Run(() =>
{
    Thread.Sleep(1500);
    cancellationSource.Cancel();
});

_ = Task.Run(() =>
{
    var i = 0;
    while (true)
    {
        Thread.Sleep(100);
        i += 1;
        Console.WriteLine($"Posting {i}...");
        actionBlock.Post(i);
    }
});

actionBlock.Completion.Wait();
