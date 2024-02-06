#r "paket:
version 7.0.2
framework: net6.0
source https://api.nuget.org/v3/index.json
nuget Be.Vlaanderen.Basisregisters.Build.Pipeline 6.0.3 //"

#load "packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/build-generic.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO.FileSystemOperators
open ``Build-generic``

let dockerRepository = "redis"

let assemblyVersionNumber = (sprintf "%s.0")
let nugetVersionNumber = (sprintf "%s")

let buildSource = build assemblyVersionNumber
let buildTest = buildTest assemblyVersionNumber
let publishSource = publish assemblyVersionNumber
let pack = packSolution nugetVersionNumber
let containerize = containerize dockerRepository
let push = push dockerRepository

supportedRuntimeIdentifiers <- [ "linux-x64" ]

// Redis Populator -----------------------------------------------------------------------

Target.create "RedisPopulator_Build" (fun _ ->
    buildSource "Be.Vlaanderen.Basisregisters.Redis.Populator"
    buildTest "Be.Vlaanderen.Basisregisters.Redis.Populator.Tests"
)

Target.create "RedisPopulator_Test" (fun _ ->
    [
        "test" @@ "Be.Vlaanderen.Basisregisters.Redis.Populator.Tests"
    ] |> List.iter testWithDotNet
)

Target.create "RedisPopulator_Publish" (fun _ -> publishSource "Be.Vlaanderen.Basisregisters.Redis.Populator")
Target.create "RedisPopulator_Containerize" (fun _ -> containerize "Be.Vlaanderen.Basisregisters.Redis.Populator" "redis-populator")
Target.create "RedisPopulator_PushContainer" (fun _ -> push "redis-populator")

// --------------------------------------------------------------------------------
Target.create "PublishAll" ignore
Target.create "PackageAll" ignore
Target.create "PushAll" ignore

// Publish ends up with artifacts in the build folder
"DotNetCli"
==> "Clean"
==> "Restore"
==> "RedisPopulator_Build"
==> "RedisPopulator_Test"
==> "RedisPopulator_Publish"
==> "PublishAll"

// Package ends up with local Docker images
"PublishAll"
==> "RedisPopulator_Containerize"
==> "PackageAll"

// Push ends up with Docker images in AWS
"PackageAll"
==> "DockerLogin"
==> "RedisPopulator_PushContainer"
==> "PushAll"

Target.runOrDefault "PackageAll"
