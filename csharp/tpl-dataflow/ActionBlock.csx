#! "netcoreapp3.0"
#r "nuget: System.Threading.Tasks.Dataflow, 4.10.0"
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

var actionBlock = new ActionBlock<int>(x => Console.WriteLine($"  Action on {x}..."));

_ = Task.Run(() =>
{
    var i = 0;
    while (true)
    {
        if (i > 5) actionBlock.Complete();

        Thread.Sleep(1000);
        i += 1;
        Console.WriteLine($"Posting {i}...");
        actionBlock.Post(i);
    }
});

actionBlock.Completion.Wait();
