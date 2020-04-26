#r "System.Threading.Tasks"
#r "System.Threading.Tasks.Dataflow"
#r "System.Runtime"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

// Writing to and Reading from a Dataflow Block Synchronously
printfn "----Example 1----"
let block = new BufferBlock<int>()
for i in 0..3 do
  printfn "Posting %A..." i
  block.Post(i) |> ignore

for i in 0..3 do
  printfn "Receiving %A..."
  <| block.Receive()

// The TryReceive method does not block the current thread
// and is useful when you occasionally poll for data
printfn "----Example 2----"
let t = Task.Run(fun () ->
  for i in 0..3 do
    printfn "Posting %A..." i
    block.Post(i) |> ignore
    Thread.Sleep(500))

let item = ref 0
while not t.IsCompleted do
  if block.TryReceive(item)
    then printfn "Received %A" item.Value
    else printfn "No value received"
  Thread.Sleep(300)

(* Because the Post<TInput> method acts synchronously, 
the BufferBlock<T> object in the previous examples receives 
all data before the second loop reads data. The following 
example extends the first example by using Tasks to read 
from and write to the message block concurrently. Because 
Invoke performs actions concurrently, the values are not 
written to the BufferBlock<T> object in any specific order. *)
printfn "----Example 3----"
let post1 = Task.Run(fun() ->
  block.Post(0) |> ignore
  block.Post(1) |> ignore)

let receive = Task.Run(fun() ->
  for i in 0..2 do
    block.Receive()
    |> printfn "Receiving %A...")

let post2 = Task.Run(fun() ->
  block.Post(2) |> ignore)

Task.WaitAll(post1, receive, post2)
