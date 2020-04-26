#r "System.Threading.Tasks"
#r "System.Threading.Tasks.Dataflow"
#r "System.Runtime"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

let broadcastBlock = new BroadcastBlock<int>(fun x -> x)

let actionBlock1 = new ActionBlock<int>(fun n ->
    printfn "Consumer1 received %d..." n)

let actionBlock2 = new ActionBlock<int>(fun n ->
    printfn "Consumer2 received %d..." n)

Task.Run(fun () ->
  let mutable i = 0
  while true do
    Thread.Sleep(100)
    if i > 10 then broadcastBlock.Complete()
    i <- i+1
    broadcastBlock.Post(i) |> ignore)

broadcastBlock.LinkTo(actionBlock1)
broadcastBlock.LinkTo(actionBlock2)
broadcastBlock.Completion.Wait()