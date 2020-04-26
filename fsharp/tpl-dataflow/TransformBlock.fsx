#r "System.Threading.Tasks"
#r "System.Threading.Tasks.Dataflow"
#r "System.Runtime"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

let transformBlock = new TransformBlock<int, string>(fun x ->
  x * 2 |> string)

let actionBlock = new ActionBlock<string>(printfn "Received %A")

Task.Run(fun () ->
  let mutable i = 0
  while true do
    if i = 5 then transformBlock.Complete()
    Thread.Sleep(1000)
    i <- i+1
    printfn "Posting %d..." i
    transformBlock.Post(i) |> ignore)

transformBlock.LinkTo(actionBlock)
transformBlock.Completion.Wait()