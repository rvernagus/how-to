let l = [1..10]
printfn $"%A{l}"

let a = [|1..10|]
printfn $"%A{a}"

printfn $"%A{l.[3..6]}"
printfn $"{l.[11..15]}"

// Slicing 3D arrays
let dim = 2
let m = Array3D.zeroCreate<int> dim dim dim

let mutable cnt = 0

for z in 0..dim-1 do
    for y in 0..dim-1 do
        for x in 0..dim-1 do
            m.[x,y,z] <- cnt
            cnt <- cnt + 1
            
printfn $"%A{m.[*, 0, 1]}"

let xs = [1..10]
printfn $"{xs.[^1]}"
printfn $"{xs.[^2..]}"
