open Ip2Location



[<EntryPoint>]
let main argv =
    let option = argv.[0]

    if option = "list" then
        Fetch.fromIPtoLocationJsonFromList ()
        0
    elif option = "param" then
        let param = argv |> Array.skip 1
        Fetch.fromIPtoLocationJsonFromParams param
        0
    elif option = "parse" then
        let ty = argv.[1]

        match ty with
        | "" ->
            printfn "Error: not type selected use domain, <other future options>"
            1
        | "domain" ->
            let domain = argv.[2]
            Parse.parseDomainbyName domain
            0
        | _ ->
            printfn "Error: not a valid type,use domain, <other future options>"
            2
    else
        printfn "Not a valid option"
        1
