#r "nuget: Microsoft.Tpl.Dataflow"
open System
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow


type MyFilterBlock<'TInput, 'TOutput>(func : 'TInput -> 'TOutput, predicate : 'TInput -> bool) =
  let _block = new TransformBlock<'TInput, 'TOutput>(func)

  interface IDataflowBlock with
    member this.Complete() =
      _block.Complete()

    member this.Fault(ex) =
      let block = _block :> IDataflowBlock
      block.Fault(ex)

    member this.Completion with get () = _block.Completion

  interface ITargetBlock<'TInput> with
    member this.OfferMessage(messageHeader, messageValue, source, consumeToAccept) =
      match not <| predicate messageValue with
      | true  -> DataflowMessageStatus.Declined
      | false ->
          let target = _block :> ITargetBlock<'TInput>
          target.OfferMessage(messageHeader, messageValue, source, consumeToAccept)

  interface ISourceBlock<'TOutput> with
    member this.ConsumeMessage(messageHeader, target, messageConsumed) =
      let source = _block :> ISourceBlock<'TOutput>
      source.ConsumeMessage(messageHeader, target, &messageConsumed)

    member this.LinkTo(target, linkOptions) =
      let source = _block :> ISourceBlock<'TOutput>
      source.LinkTo(target, linkOptions)

    member this.ReleaseReservation(messageHeader, target) =
      let source = _block :> ISourceBlock<'TOutput>
      source.ReleaseReservation(messageHeader, target)

    member this.ReserveMessage(messageHeader, target) =
      let source = _block :> ISourceBlock<'TOutput>
      source.ReserveMessage(messageHeader, target)


let actionBlock = new ActionBlock<int>(printfn "Received %d...")

let filterBlock = new MyFilterBlock<int, int>((fun n -> n), (fun n -> n % 2 = 0))
filterBlock.LinkTo actionBlock
filterBlock.Post 1
filterBlock.Post 2
filterBlock.Post 3
filterBlock.Post 4
filterBlock.Post 5
filterBlock.Post 6
(filterBlock :> IDataflowBlock).Complete()
(filterBlock :> IDataflowBlock).Completion.Wait()
