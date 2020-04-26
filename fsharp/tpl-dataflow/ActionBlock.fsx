#r "System.Threading.Tasks"
#r "System.Threading.Tasks.Dataflow"
#r "System.Runtime"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

let actionBlock = new ActionBlock<int>(printfn "  Action on %d...")

Task.Run(fun () ->
  let mutable i = 0
  while true do
    if i > 5 then actionBlock.Complete()
    Thread.Sleep(1000)
    i <- i+1
    printfn "Posting %d..." i
    actionBlock.Post(i) |> ignore)

actionBlock.Completion.Wait()
