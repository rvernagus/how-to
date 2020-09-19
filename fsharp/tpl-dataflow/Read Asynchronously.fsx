#r "nuget: Microsoft.Tpl.Dataflow"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

let block = new BufferBlock<int>()

Task.Run(fun () ->
  for i in 0..3 do
    Thread.Sleep(100)
    printfn "Sending %d" i
    block.SendAsync(i) |> ignore)

for i in 0..3 do
  block.ReceiveAsync().ContinueWith(fun (t : Task<int>) ->
    t.Result
    |> printfn "Received %A...")
  |> ignore

printfn "Do some work..."
Console.ReadLine()
