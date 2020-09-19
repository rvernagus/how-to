#r "nuget: Microsoft.Tpl.Dataflow"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

let block1 = new ActionBlock<int>(printfn "  Received %d in Even #s Block...")
let block2 = new ActionBlock<int>(printfn "  Received %d in Odd #s Block...")

let dataSource = new BufferBlock<int>()
dataSource.LinkTo(block1, fun n -> n%2 = 0)
dataSource.LinkTo(block2, fun n -> n%2 = 1)

Task.Run(fun () ->
  let rand = new Random()
  for i = 0 to 10 do
    Thread.Sleep(1000)
    let n = rand.Next(10)
    printfn "Posting %d..." n
    dataSource.Post(n) |> ignore
  dataSource.Complete())

dataSource.Completion.Wait()
