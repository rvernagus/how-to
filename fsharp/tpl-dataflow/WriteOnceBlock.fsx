#r "System.Threading.Tasks"
#r "System.Threading.Tasks.Dataflow"
#r "System.Runtime"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

let writeOnceBlock = new WriteOnceBlock<int>(fun x -> x)
let actionBlock = new ActionBlock<int>(fun x ->
    printfn "  Receiving %d..." x)

Task.Run(fun () ->
  let targetBlock = writeOnceBlock :> ITargetBlock<int>
  let mutable i = 0
  while true do
    if i > 3 then actionBlock.Complete()
    Thread.Sleep(1000)
    i <- i+1
        
    printfn "Offering %d..." i
    let messageHeader = new DataflowMessageHeader(1L)
    targetBlock.OfferMessage(messageHeader, i, null, false)
    |> printfn "  MessageStatus: %A")


writeOnceBlock.LinkTo(actionBlock)
actionBlock.Completion.Wait()