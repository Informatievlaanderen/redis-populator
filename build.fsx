#load "packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/build-generic.fsx"

open Fake
open Fake.NpmHelper
open ``Build-generic``

let dockerRepository = "redis"

let assemblyVersionNumber = (sprintf "2.0.0.%s")
let nugetVersionNumber = (sprintf "2.0.%s")

let build = buildSolution assemblyVersionNumber
let publish = publishSolution assemblyVersionNumber
let pack = packSolution nugetVersionNumber
let containerize = containerize dockerRepository
let push = push dockerRepository

Target "Clean" (fun _ ->
  CleanDir buildDir
)

// Redis Populator -----------------------------------------------------------------------

Target "RedisPopulator_Build" (fun _ -> build "Be.Vlaanderen.Basisregisters.Redis.Populator")
Target "RedisPopulator_Test" (fun _ -> [ "test" @@ "Be.Vlaanderen.Basisregisters.Redis.Populator.Tests" ] |> List.iter testWithDotNet)

Target "RedisPopulator_Publish" (fun _ -> publish "Be.Vlaanderen.Basisregisters.Redis.Populator")
Target "RedisPopulator_Containerize" (fun _ -> containerize "Be.Vlaanderen.Basisregisters.Redis.Populator" "redis-populator")
Target "RedisPopulator_PushContainer" (fun _ -> push "redis-populator")

// --------------------------------------------------------------------------------

Target "PublishRedisPopulator" DoNothing
Target "PublishAll" DoNothing

Target "PackageRedisPopulator" DoNothing
Target "PackageAll" DoNothing

Target "PushRedisPopulator" DoNothing
Target "PushAll" DoNothing

// Publish ends up with artifacts in the build folder
"DotNetCli" ==> "Clean" ==> "Restore" ==> "RedisPopulator_Build" ==> "RedisPopulator_Test"  ==> "RedisPopulator_Publish" ==> "PublishRedisPopulator"
"PublishRedisPopulator" ==> "PublishAll"

// Package ends up with local Docker images
"PublishRedisPopulator" ==> "RedisPopulator_Containerize" ==> "PackageRedisPopulator"
"PackageRedisPopulator" ==> "PackageAll"

// Push ends up with Docker images in AWS
"PackageRedisPopulator" ==> "DockerLogin" ==> "RedisPopulator_PushContainer" ==> "PushRedisPopulator"
"PushRedisPopulator" ==> "PushAll"

RunTargetOrDefault "PackageAll"
