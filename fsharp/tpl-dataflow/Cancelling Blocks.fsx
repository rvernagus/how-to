#r "System.Threading.Tasks"
#r "System.Threading.Tasks.Dataflow"
#r "System.Runtime"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow


let cancellationSource = new CancellationTokenSource()
let options = new ExecutionDataflowBlockOptions(CancellationToken=cancellationSource.Token)

let actionBlock = new ActionBlock<int>((fun n ->
  Thread.Sleep(200)
  printfn "  Action on %d..." n), options)

Task.Run(fun () ->
  Thread.Sleep(1500)
  cancellationSource.Cancel())

Task.Run(fun () ->
  let mutable i = 0
  while true do
    Thread.Sleep(100)
    i <- i+1
    printfn "Posting %d..." i
    actionBlock.Post(i) |> ignore)

actionBlock.Completion.Wait()