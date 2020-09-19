#r "nuget: Microsoft.Tpl.Dataflow"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

let batchBlock = new BatchBlock<int>(5)

let actionBlock = new ActionBlock<int[]>(printfn "Received %A")

Task.Run(fun () ->
  let mutable i = 0
  while true do
    if i > 15 then batchBlock.Complete()
    Thread.Sleep(100)
    i <- i+1
    printfn "Posting %d..." i
    batchBlock.Post(i) |> ignore)

batchBlock.LinkTo(actionBlock)
batchBlock.Completion.Wait()
