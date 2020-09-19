#r "nuget: Microsoft.Tpl.Dataflow"
open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

let bufferBlock = new BufferBlock<int>()

let producer () =
  let mutable i = 0
  while true do
    Thread.Sleep(1000)
    i <- i+1
    printfn "Posting %d..." i
    bufferBlock.Post(i) |> ignore

let consumer () =
  let items = ref (new List<int>() :> IList<int>)
  while true do
    Thread.Sleep(3500)
    if bufferBlock.TryReceiveAll(items) then
      printfn "Consumer received %A..." items

Task.Factory.StartNew(producer)
Task.Factory.StartNew(consumer)
printfn "Press <Enter> to exit..."
Console.ReadLine()

