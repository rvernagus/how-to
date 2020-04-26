#! "netcoreapp3.0"
#r "nuget: System.Threading.Tasks.Dataflow, 4.10.0"
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

var bufferBlock = new BufferBlock<int>();

Action producer = () =>
{
    var i = 0;
    while (true)
    {
        Thread.Sleep(1000);
        i += 1;
        Console.WriteLine($"Posting {i}...");
        bufferBlock.Post(i);
    }
};

Action consumer = () =>
{
    var items = new List<int>() as IList<int>;
    while (true)
    {
        Thread.Sleep(3500);
        if (bufferBlock.TryReceiveAll(out items))
            Console.WriteLine($"Consumer received {string.Join(',', items)}...");
    }
};

_ = Task.Factory.StartNew(producer);
_ = Task.Factory.StartNew(consumer);
Console.WriteLine("Press <Enter> to exit...");
Console.ReadLine();
