#r "System.Threading.Tasks"
#r "System.Threading.Tasks.Dataflow"
#r "System.Runtime"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

let options = new ExecutionDataflowBlockOptions(MaxDegreeOfParallelism = 5)
let actionBlock = new ActionBlock<int>((fun x ->
  Thread.Sleep(2000)
  printfn "  Action on Thread #%d value %d..." Thread.CurrentThread.ManagedThreadId x), options)

Task.Run(fun () ->
  let mutable i = 0
  while true do
    Thread.Sleep(500)
    i <- i+1
    printfn "Posting %d..." i
    actionBlock.Post(i) |> ignore)

printfn "Press <Enter> to exit..."
Console.ReadLine()
