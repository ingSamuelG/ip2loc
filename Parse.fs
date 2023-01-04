namespace Ip2Location

module Parse =
    open System.IO
    open System.Text.Json
    open System.Text.RegularExpressions

    // Read all the JSON files in a folder
    let readJsonFiles (folder: string) =
        Directory.GetFiles(folder, "*.json")
        |> Array.map (fun file -> file, File.ReadAllText(file))

    let createCsvFile (data: (string * string * string) array) =
        use writer = new StreamWriter("./exports/data.csv")

        writer.WriteLine("ip,isp,domain")

        for (ip, isp, domain) in data do
            writer.WriteLine("{0},{1},{2}", ip, isp, domain)

    // Check if a JSON object has the property "domain" with value "hetzner"
    let hasDomain domainName (filePath: string, json: JsonElement) =
        let domain = (json.GetProperty "domain").ToString()
        let pareseDomainRegex = sprintf $".*{domainName}.*"
        let hetznerRegex = new Regex(pareseDomainRegex)
        hetznerRegex.IsMatch(domain)

    let addDomainsIpsToFile domainName (ips: string []) =
        // Create a new file or overwrite an existing one
        if ips.Length > 0 then
            use writer = new StreamWriter($"./exports/{domainName}_ips.txt", false)
            // Write each element of the array to the file, separated by a new line
            ips |> Array.iter (fun ip -> writer.WriteLine(ip))
            // Make sure to close the file when you're done
            writer.Close()
            printfn $"Success for domain {domainName} check your export folder"
        else
            printfn $"Error:{domainName} its not on your current JSON files"

    let getListOfIpFromFileName (filePath: string) =
        let trimedFilePath = filePath.Trim()

        (Path.GetFileName trimedFilePath)
            .Replace(".json", "")

    // Get the names of the files with domain "hetzner"
    let getDomainFiles domainName files =
        // let filename, content = files
        files
        |> Array.map (fun (filePath: string, read: string) -> filePath, JsonSerializer.Deserialize<JsonElement>(read))
        |> Array.filter (hasDomain domainName)
        |> Array.map (fun (filePath, json: JsonElement) -> getListOfIpFromFileName filePath)
        |> (addDomainsIpsToFile domainName)

    let getNotDomainFiles domainName files =
        // let filename, content = files
        files
        |> Array.map (fun (filePath: string, read: string) -> filePath, JsonSerializer.Deserialize<JsonElement>(read))
        |> Array.filter (fun (x) -> not (hasDomain domainName x))
        |> Array.map (fun (filePath, json: JsonElement) ->
            getListOfIpFromFileName filePath,
            (json.GetProperty "domain").ToString(),
            (json.GetProperty "isp").ToString())
        |> createCsvFile

    let parseDomainbyName domain =
        let folder = "./files"
        let files = readJsonFiles folder
        getDomainFiles domain files |> ignore

    let parseNotDomain domain =
        let folder = "./files"
        let files = readJsonFiles folder
        getNotDomainFiles domain files |> ignore
// printfn "%A" hetznerFiles
