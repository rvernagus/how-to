#r "System.Threading.Tasks"
#r "System.Threading.Tasks.Dataflow"
#r "System.Runtime"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow


let dataSource = new BufferBlock<int>()

let workers = 
  // This example requires nongreedy blocks
  let options = new ExecutionDataflowBlockOptions(BoundedCapacity = 1)
  
  [ for i = 0 to 5 do
      let block = new ActionBlock<int>((fun x ->
        Thread.Sleep(5000)
        printfn "  Worker %d received %d" i x), options)
      dataSource.LinkTo(block) |> ignore
      yield block ]

Task.Run(fun () ->
  let mutable n = 0
  while true do
    Thread.Sleep(1000)
    n <- n+1
    printfn "Posting %d..." n
    dataSource.Post(n) |> ignore)


printfn "Press <Enter> to exit..."
Console.ReadLine()
