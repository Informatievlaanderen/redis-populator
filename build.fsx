#r "paket:
version 5.241.6
framework: netstandard20
source https://api.nuget.org/v3/index.json
nuget Be.Vlaanderen.Basisregisters.Build.Pipeline 3.3.2 //"

#load "packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/build-generic.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO.FileSystemOperators
open ``Build-generic``

let dockerRepository = "redis"

let assemblyVersionNumber = (sprintf "2.0.0.%s")
let nugetVersionNumber = (sprintf "2.0.%s")

let build = buildSolution assemblyVersionNumber
let publish = publishSolution assemblyVersionNumber
let pack = packSolution nugetVersionNumber
let containerize = containerize dockerRepository
let push = push dockerRepository

// Redis Populator -----------------------------------------------------------------------

Target.create "RedisPopulator_Build" (fun _ -> build "Be.Vlaanderen.Basisregisters.Redis.Populator")
Target.create "RedisPopulator_Test" (fun _ ->
  [
    "test" @@ "Be.Vlaanderen.Basisregisters.Redis.Populator.Tests"
  ] |> List.iter testWithDotNet
)

Target.create "RedisPopulator_Publish" (fun _ -> publish "Be.Vlaanderen.Basisregisters.Redis.Populator")
Target.create "RedisPopulator_Containerize" (fun _ -> containerize "Be.Vlaanderen.Basisregisters.Redis.Populator" "redis-populator")
Target.create "RedisPopulator_PushContainer" (fun _ -> push "redis-populator")

// --------------------------------------------------------------------------------

Target.create "PublishRedisPopulator" ignore
Target.create "PublishAll" ignore

Target.create "PackageRedisPopulator" ignore
Target.create "PackageAll" ignore

Target.create "PushRedisPopulator" ignore
Target.create "PushAll" ignore

// Publish ends up with artifacts in the build folder
"DotNetCli" ==> "Clean" ==> "Restore" ==> "RedisPopulator_Build" ==> "RedisPopulator_Test"  ==> "RedisPopulator_Publish" ==> "PublishRedisPopulator"
"PublishRedisPopulator" ==> "PublishAll"

// Package ends up with local Docker images
"PublishRedisPopulator" ==> "RedisPopulator_Containerize" ==> "PackageRedisPopulator"
"PackageRedisPopulator" ==> "PackageAll"

// Push ends up with Docker images in AWS
"PackageRedisPopulator" ==> "DockerLogin" ==> "RedisPopulator_PushContainer" ==> "PushRedisPopulator"
"PushRedisPopulator" ==> "PushAll"

Target.runOrDefault "PackageAll"
