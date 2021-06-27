module Barotrauma_Drone_Extractor.Extractor

open System.IO
open System.Xml.Linq
open BaroLib
open FSharp.XExtensions

let convertDrone (drone: XElement) =
    let attributesToRemove =
        [ "filepath"
          "pos"
          "linkedto"
          "originallinkedto"
          "originalmyport" ]

    attributesToRemove
    |> List.iter (fun a -> drone.SetAttributeValue(a, null))

    drone.Name <- XName.Get "Submarine"

    XDocument drone

let extractDrones (sub: XDocument) =
    sub
        .Element("Submarine")
        .Elements "LinkedSubmarine"
    |> Seq.map convertDrone

let saveDrones directory fallback (drones: XDocument seq) =
    let getFilepath (drone: XDocument) =
        let name =
            let attr =
                drone.Element("Submarine").Attribute("name").Value

            match System.String.IsNullOrWhiteSpace(attr) with
            | false -> attr
            | true -> fallback

        let mutable tempName = $"%s{name}.sub"
        let mutable i = 0

        while Directory.Exists(Path.Combine(directory, name)) do
            i <- i + 1
            tempName <- $"%s{name} (%i{i}).sub"

        Path.Combine(directory, tempName)

    drones
    |> Seq.iter
        (fun d ->
            let path = getFilepath d
            printfn $"Saving %s{path}"
            IoUtil.SaveSub(d, path))

let parseFilepath (path: string) =
    let info =
        Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)))

    let sub = IoUtil.LoadSub path

    let fallback =
        $"""%s{sub.Element("Submarine").Name.LocalName}Drone"""

    info.FullName, fallback, sub

let processFilepath (path: string) =
    let fileName = Path.GetFileName path
    printfn $"Processing %s{fileName}"
    let dir, fallback, sub = path |> parseFilepath
    extractDrones sub |> saveDrones dir fallback
