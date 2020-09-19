#r "nuget: Microsoft.Tpl.Dataflow"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

let func x =
  Thread.Sleep 100
  printfn "Received %d" x
  x + 1

let transformBlock1 = new TransformBlock<int, int>(func)
let transformBlock2 = new TransformBlock<int, int>(func)
let transformBlock3 = new TransformBlock<int, int>(func)
let transformBlock4 = new TransformBlock<int, int>(func)

let printBlockStatuses () =
  printfn "transformBlock1 status: %A" transformBlock1.Completion.Status
  printfn "transformBlock2 status: %A" transformBlock2.Completion.Status
  printfn "transformBlock3 status: %A" transformBlock3.Completion.Status
  printfn "transformBlock4 status: %A" transformBlock4.Completion.Status

printBlockStatuses()
// Swap the next two lines to errors propagate or not
// let options = new DataflowLinkOptions(PropagateCompletion=true)
let options = new DataflowLinkOptions(PropagateCompletion=false)
transformBlock1.LinkTo(transformBlock2, options)
transformBlock2.LinkTo(transformBlock3, options)
transformBlock3.LinkTo(transformBlock4, options)

Task.Run(fun () ->
  while true do
    Thread.Sleep 500
    if transformBlock1.Post(1) then
        printfn "Message sent successfully"
    else
        printfn "Message was not able to be sent"
        printBlockStatuses())


Task.Run(fun () ->
  Thread.Sleep 2000
  let block = transformBlock1 :> IDataflowBlock
  let ex = new ApplicationException("BOOM!")
  block.Fault(ex))


printfn "Press <Enter> to exit..."
Console.ReadLine()
