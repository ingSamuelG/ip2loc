namespace Ip2Location

module Parse =
    open System.IO
    open System.Text.Json
    open System.Text.RegularExpressions

    // Read all the JSON files in a folder
    let readJsonFiles (folder: string) =
        Directory.GetFiles(folder, "*.json")
        |> Array.map (fun file -> file, File.ReadAllText(file))

    // Check if a JSON object has the property "domain" with value "hetzner"
    let hasDomain domainName (filePath: string, json: JsonElement) =
        let domain = (json.GetProperty "domain").ToString()
        let pareseDomainRegex = sprintf $".*{domainName}.*"
        let hetznerRegex = new Regex(pareseDomainRegex)
        hetznerRegex.IsMatch(domain)

    let addIpsToFile domainName (ips: string []) =
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
    let getHetznerFiles domainName files =
        // let filename, content = files
        files
        // |> Array.map (fun (filename, file) -> JsonSerializer.Deserialize<JsonElement>(file))
        |> Array.map (fun (filePath: string, read: string) -> filePath, JsonSerializer.Deserialize<JsonElement>(read))
        |> Array.filter (hasDomain domainName)
        |> Array.map (fun (filePath, json: JsonElement) -> getListOfIpFromFileName filePath)
        |> (addIpsToFile domainName)
    // |> Array.map (fun file -> file.GetProperty "domain")

    let parseDomainbyName domain =
        let folder = "./files"
        let files = readJsonFiles folder
        getHetznerFiles domain files |> ignore
// printfn "%A" hetznerFiles
