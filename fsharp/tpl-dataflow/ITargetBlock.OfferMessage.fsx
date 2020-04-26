#r "System.Threading.Tasks"
#r "System.Threading.Tasks.Dataflow"
#r "System.Runtime"
open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow


let consumer = new ActionBlock<int>(fun n ->
  Thread.Sleep(10)
  printfn " Consumer received %d..." n)

Task.Run(fun () ->
  let mutable i = 0
  while true do
    Thread.Sleep(1000)
    i <- i+1
    let messageHeader = new DataflowMessageHeader(1L)
    printfn " Offering message: %d" i
    (consumer :> ITargetBlock<int>).OfferMessage(messageHeader, i, null, false)
    |> printfn "   MessageStatus: %A")

Task.Run(fun () ->
  Thread.Sleep(4000)
  consumer.Complete())

Console.ReadLine()