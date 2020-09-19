#r "nuget: Microsoft.Tpl.Dataflow"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

let func x =
  [ x; x*2; x*3 ]
  |> List.toSeq

let transformBlock = new TransformManyBlock<int, int>(fun x ->
    [ x; x*2; x*3 ]
    |> List.toSeq)

let actionBlock = new ActionBlock<int>(printfn "Receiving %A")

Task.Run(fun () ->
  let mutable i = 0
  while true do
    if i > 4 then transformBlock.Complete()
    Thread.Sleep(1000)
    i <- i+1
    printfn "Posting %d..." i
    transformBlock.Post(i) |> ignore)

transformBlock.LinkTo(actionBlock)
transformBlock.Completion.Wait()
