#r "System.Threading.Tasks"
#r "System.Threading.Tasks.Dataflow"
#r "System.Runtime"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

let joinBlock = new JoinBlock<int, int>()

let actionBlock = new ActionBlock<int * int>(printfn "  Received %A")

Task.Run(fun () ->
  let mutable i = 0
  while true do
    Thread.Sleep(1000)
    i <- i+1
    printfn "Posting %d..." i
    joinBlock.Target1.Post(i) |> ignore)

Task.Run(fun () ->
  let mutable i = 0
  while true do
    if i < -4 then joinBlock.Complete()
    Thread.Sleep(1500)
    i <- i-1
    printfn "Posting %d..." i
    joinBlock.Target2.Post(i) |> ignore)

joinBlock.LinkTo(actionBlock)
joinBlock.Completion.Wait()
