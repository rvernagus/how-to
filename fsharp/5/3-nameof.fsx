module M =
    let f x = nameof x

printfn $"{M.f 12}"
printfn $"{nameof M}"
printfn $"{nameof M.f}"

type Person =
    { Name : string
      Age : int }

let reflectOnPerson p prop =
    match prop with
    | nameof p.Name -> p.Name
    | nameof p.Age  -> string p.Age

printfn $"""{reflectOnPerson {Name="Ray"; Age=47} "Name"}"""
printfn $"""{reflectOnPerson {Name="Ray"; Age=47} "Age" }"""
