open System
open System.Net
open System.Net.Http
open System.Net.Http.Json
open System.IO

let isValidIPv4 (ip: string) =
    let isValid = IPAddress.TryParse(ip)
    isValid

let fromIPtoLocationJson ips =
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


[<EntryPoint>]
let main argv =
    fromIPtoLocationJson argv
    0
