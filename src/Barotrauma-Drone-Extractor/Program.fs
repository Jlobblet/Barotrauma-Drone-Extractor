// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.IO
open FSharp.XExtensions
open BaroLib
open Barotrauma_Drone_Extractor.Arguments
open Barotrauma_Drone_Extractor.Extractor

[<EntryPoint>]
let main argv =
    if argv |> Array.isEmpty then
        parser.PrintUsage() |> printfn "%s"
        exit 0

    argv |> parseArgv |> List.iter processFilepath

    0 // return an integer exit code
