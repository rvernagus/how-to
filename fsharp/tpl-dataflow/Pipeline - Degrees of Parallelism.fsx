#r "nuget: Microsoft.Tpl.Dataflow"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

let mutable fastInputCount = 0
let mutable fastOutputCount = 0
let mutable slowQueueCount = 0

let fast x =
  Thread.Sleep 100
  printfn "%d ->   FAST on Thread #%d (queue [%d|%d])" x Thread.CurrentThread.ManagedThreadId fastInputCount fastOutputCount
  x + 1

let slow x =
  Thread.Sleep 500
  printfn "  %d -> SLOW on Thread #%d (queue [%d])" x Thread.CurrentThread.ManagedThreadId slowQueueCount
  x + 1

let singleThreadedOptions = new ExecutionDataflowBlockOptions(MaxDegreeOfParallelism=1)
let multiThreadedOptions = new ExecutionDataflowBlockOptions(MaxDegreeOfParallelism=Int32.MaxValue)

// Swap the next two lines to demonstrate single vs. multi-threaded
let slowBlock = new TransformBlock<int, int>(slow, multiThreadedOptions)
// let slowBlock = new TransformBlock<int, int>(slow, singleThreadedOptions)
let fastBlock = new TransformBlock<int, int>(fast, singleThreadedOptions)

fastBlock.LinkTo(slowBlock)

Task.Run(fun () ->
  while true do
    Thread.Sleep 50
    fastInputCount <- fastBlock.InputCount
    fastOutputCount <- fastBlock.OutputCount
    slowQueueCount <- slowBlock.InputCount
    fastBlock.Post(1) |> ignore)

printfn "Press <Enter> to exit..."
Console.ReadLine()
