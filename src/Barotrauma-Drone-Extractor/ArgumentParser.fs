module Barotrauma_Drone_Extractor.Arguments

open System
open System.IO
open Argu

type Arguments =
    | [<MainCommand>] InputPath of path: string
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | InputPath _ -> "The path to a .sub file to extract drones/shuttles from"

let errorHandler =
    ProcessExiter
        (function
        | ErrorCode.HelpText -> None
        | _ -> Some ConsoleColor.Red)

let parser =
    ArgumentParser.Create<Arguments>(errorHandler = errorHandler)

let parseArgv argv =
    let results = parser.Parse argv

    let parseSub (p: string) =
        match p |> Path.GetExtension |> (=) ".sub" with
        | true -> p
        | false -> failwith $"Invalid path passed: %s{p}"

    results.PostProcessResults(<@ InputPath @>, parseSub)
