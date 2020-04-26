#r "System.Threading.Tasks"
#r "System.Threading.Tasks.Dataflow"
#r "System.Runtime"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

let bufferBlock = new BufferBlock<int>()

let options = new ExecutionDataflowBlockOptions(MaxDegreeOfParallelism=1)

let actionBlock = new ActionBlock<int>((fun n ->
  Thread.Sleep(1200)
  printfn "  Received %d..." n), options)

Task.Run(fun () ->
  let mutable i = 0
  while true do
    if i > 10 then bufferBlock.Complete()
    Thread.Sleep(600)
    i <- i+1
    printfn "Posting %d : %b" i <| bufferBlock.Post(i)
    printfn "  bufferBlock.Count: %d" bufferBlock.Count
    printfn "  actionBlock.InputCount: %d" actionBlock.InputCount)

let linkOptions = new DataflowLinkOptions(PropagateCompletion=true)

bufferBlock.LinkTo(actionBlock, linkOptions)
actionBlock.Completion.Wait()