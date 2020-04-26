// Demonstrated in this example:
//   * F# Union Types
//   * F# Record Types
//   * F# Type Extensions
//   * F# Asynchronous Workflows
//   * F# Parallel Computation
//   * F# Sequence Expressions
//   * Interactive Execution vs. Compiled Executable
//   * Function Composition using >>
//   * Function Piping using |>
open System
open System.Diagnostics
open System.Net
open System.Net.Sockets

let checksum (bytes : byte[]) =
    let limit = bytes.Length - 1
    let pair i = BitConverter.ToUInt16(bytes, i)
    
    let mutable sum = 0
    for i in [0..2..limit-1] do
        sum <- sum + int (pair i)
    
    sum <- (sum >>> 16) + (sum &&& 0xffff)
    sum <- sum + (sum >>> 16)
    uint16 ~~~sum

type Socket with
    member this.AsyncReceiveFrom(receiveBuffer, offset, size, socketFlags, remoteEP) =
        Async.FromBeginEnd(
            (fun (cb,o) -> this.BeginReceiveFrom(receiveBuffer, offset, size, socketFlags, remoteEP, cb, o)),
            (fun ar -> this.EndReceiveFrom(ar, remoteEP)))

type IcmpPacket =
    { Type           : byte;
      Code           : byte;
      Checksum       : uint16;
      Identifier     : uint16;
      SequenceNumber : uint16;
      Data           : byte[] } with
      
      static member Echo =
        let data = Array.create 32 (byte '#')
        
        let packet =
            { Type           = 8uy;
              Code           = 0uy;
              Checksum       = 0us;
              Identifier     = 45us;
              SequenceNumber = 0us;
              Data           = data }
        
        let setChecksum cksum = { packet with Checksum = cksum }
        
        packet.ToByteArray()
        |> checksum
        |> setChecksum
      
      member this.ToByteArray() =
        let bytes (x : uint16) = BitConverter.GetBytes(x)
        
        Array.concat [
         [| this.Type; this.Code |];
            bytes this.Checksum;
            bytes this.Identifier;
            bytes this.SequenceNumber;
            this.Data ]

type PingResult =
    | Successful of string * string * int
    | HostNotFound of string
    | SocketError of string
    | Timeout of string * string

exception HostNotFoundException of string
exception SocketErrorException of string

        
let getHost (name : string) =
    try
        Dns.GetHostEntry(name)
    with _ -> raise (HostNotFoundException name)

let getLocalHost() =
    Dns.GetHostName()
    |> Dns.GetHostEntry

let getEndPoint (ipHostEntry : IPHostEntry) =
    new IPEndPoint(ipHostEntry.AddressList.[0], 0)
    :> EndPoint

let send (socket : Socket) ep packet =
    let result = socket.SendTo(packet, packet.Length, SocketFlags.None, ep)
    
    match result with
    | -1 -> raise (SocketErrorException (string ep))
    | _  -> result

let receive (socket : Socket) ep =
    let receiveAsync buffer = async { 
        let! result = socket.AsyncReceiveFrom(buffer, 0, 256, SocketFlags.None, ep)
        return result }
        
    let getResult f = Async.RunSynchronously(f, 1000)
    
    let checkResult = function
        | -1      -> raise (SocketErrorException (string ep))
        | result  -> result
    
    Array.zeroCreate 256
    |> receiveAsync
    |> getResult
    |> checkResult

let timed f =
    let watch = new Stopwatch()
    
    let result =
        try
            watch.Start()
            f()
        finally
            watch.Stop()
    
    (int watch.ElapsedMilliseconds, result)

let ping host =
    let ping' hostName serverEP clientEP =
        try
            use socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp)
            socket.Blocking <- false
            
            let packet = IcmpPacket.Echo.ToByteArray()
            
            send socket serverEP packet |> ignore
            // Cache a compiled function so we are not timing the compile overhead
            let receive = fun () -> receive socket clientEP
            let t, result = timed receive
            
            socket.Close()
            
            Successful (hostName, string serverEP, t)
        with
        | :? TimeoutException -> Timeout (hostName, string serverEP)

    let pingHost serverHost clientHost =
        try
            let serverEP = getEndPoint serverHost
            let clientEP = ref (getEndPoint clientHost)
            
            ping' serverHost.HostName serverEP clientEP
        with
        | SocketErrorException (ep) -> SocketError (ep)
    
    try
        let serverHost = getHost host
        let clientHost = getLocalHost()
        
        pingHost serverHost clientHost
    with
    | HostNotFoundException (host) -> HostNotFound (host)

let pingAsync host = async { return ping host }

let pingAllAsync =
    Seq.map pingAsync >>
    Async.Parallel >>
    Async.RunSynchronously

let printPingResults =
    pingAllAsync >>
    Seq.iter (printfn "%A")

// #if COMPILED
// [<EntryPoint>]
// let main(args : string[]) =
//     args
//     |> printPingResults
    
//     0
// #else
// [ "www.google.com"; "www.yahoo.com"; "www.microsoft.com"; "noping" ]
// |> printPingResults
// #endif

[ "www.google.com"; "www.yahoo.com"; "www.microsoft.com"; "noping" ]
|> printPingResults
