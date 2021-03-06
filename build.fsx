// Copyright (c) 2016 Theodore Tsirpanis
// 
// This software is released under the MIT License.
// http://opensource.org/licenses/mit-license.php
// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"
#r "./packages/Fantomas/lib/FantomasLib.dll"
#r "./packages/FSharp.Compiler.Service/lib/net45/FSharp.Compiler.Service.dll"

open Fake
open Fake.AppVeyor
open Fake.AssemblyInfoFile
open Fake.Git
open Fantomas.FakeHelpers
open Fantomas.FormatConfig
open System
open System.IO

// Directories
[<Literal>]
let AppName = "Brainsharp"

[<Literal>]
let AppVersionMessage = 
    "Brainsharp Version {0}. \nGit commit hash: {1}. \nBuilt on {2} (UTC)." 
    + "\nCreated by Theodore Tsirpanis and licensed under the MIT License." 
    + "\nCheck out new releases on https://github.com/teo-tsirpanis/brainsharp/releases"

// version info
[<Literal>]
let BuildVersion = "1.3"

let version = 
    match buildServer with
    | AppVeyor -> AppVeyorEnvironment.BuildVersion
    | _ -> BuildVersion // or retrieve from CI server

let buildDir = "./build/"

let fantomasConfig = 
    { FormatConfig.Default with PageWidth = 80
                                ReorderOpenDeclaration = true }

// Filesets
let appReferences = !!"/**/*.csproj" ++ "/**/*.fsproj"
let sourceFiles = !!"src/**/*.fs" ++ "src/**/*.fsx" ++ "build.fsx"

let attributes = 
    let gitHash = Information.getCurrentHash()
    let buildDate = DateTime.UtcNow.ToString()
    [ Attribute.Title "Brainsharp"
      Attribute.Description "A Brainfuck toolchain written in F#."
      
      Attribute.Copyright 
          "Licensed under the MIT License. Created by Theodore Tsirpanis."
      Attribute.Metadata("Git Hash", gitHash)
      Attribute.Metadata("Build Date", buildDate)
      
      Attribute.Metadata
          ("Version Message", 
           String.Format(AppVersionMessage, version, gitHash, buildDate))
      Attribute.Version version ]

// Targets
Target "Clean" (fun _ -> CleanDir buildDir)

let DoBuild f = 
    CreateFSharpAssemblyInfo "./src/bsc/AssemblyInfo.fs" attributes
    f buildDir "Build" appReferences |> Log "AppBuild-Output: "

Target "Debug" (fun _ -> DoBuild MSBuildDebug)
Target "Release" (fun _ -> DoBuild MSBuildRelease)
Target "FormatCode" (fun _ -> 
    sourceFiles
    |> formatCode fantomasConfig
    |> Log "Formatted Files: ")
// Build order
"Clean" ?=> "Debug"
"Clean" ?=> "Release"
// start build
RunTargetOrDefault "Release"
