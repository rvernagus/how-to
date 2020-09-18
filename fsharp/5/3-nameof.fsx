module M =
    let f x = nameof x

printfn $"{M.f 12}"
printfn $"{nameof M}"
printfn $"{nameof M.f}"
