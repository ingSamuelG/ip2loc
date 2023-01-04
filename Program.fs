open System.Net
open System.Net.Http
open System.IO

let isValidIPv4 (ip: string) =
    let isValid = IPAddress.TryParse(ip)
    isValid

let fromIPtoLocationJsonFromParams ips =
    let apiKey = "HCYFRHMCLD"

    for ip in ips do
        let isValid, parseIp = isValidIPv4 ip

        if isValid then
            let client = new HttpClient()

            let response =
                client
                    .GetAsync(
                        sprintf
                            $"https://api.ip2location.com/v2/?key={apiKey}&ip={parseIp}&format=json&package=WS25&&addon=continent,country,region,city,geotargeting,country_groupings,time_zone_info&lang=zh-cn"
                    )
                    .Result

            let content = response.Content.ReadAsStringAsync().Result

            System.IO.File.WriteAllText($"./files/{ip}.json", content)
            printfn $"Success: in the response for {parseIp}"
        else
            printfn $"Error: in the response for {parseIp}"

let fromIPtoLocationJsonFromList () =
    let apiKey = "HCYFRHMCLD"
    let rows = File.ReadAllLines "./ips/ips.txt"

    for ip in rows do
        let trimIp = ip.Trim()
        let isValid, parseIp = isValidIPv4 trimIp

        if isValid then
            let client = new HttpClient()

            let response =
                client
                    .GetAsync(
                        sprintf
                            $"https://api.ip2location.com/v2/?key={apiKey}&ip={parseIp}&format=json&package=WS25&&addon=continent,country,region,city,geotargeting,country_groupings,time_zone_info&lang=zh-cn"
                    )
                    .Result

            let content = response.Content.ReadAsStringAsync().Result

            System.IO.File.WriteAllText($"./files/{ip}.json", content)
            printfn $"Success: in the response for {parseIp}"
        else
            printfn $"Error: in the response for {parseIp}"

[<EntryPoint>]
let main argv =
    let option = argv.[0]
    let param = argv |> Array.skip 1

    if option = "list" then
        fromIPtoLocationJsonFromList ()
        0
    elif option = "param" then
        fromIPtoLocationJsonFromParams param
        0
    else
        printfn "Not a valid option"
        1
