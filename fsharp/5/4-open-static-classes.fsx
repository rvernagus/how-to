// Open the 'Math' static class from .NET
open type System.Math

printfn $"{Abs(-100)}"

// Define our own static class and opening it
[<AbstractClass;Sealed>]
type A =
    static member Add(x, y) = x + y

open type A

printfn $"{Add(2.0, PI)}"
