## [5.2.6](https://github.com/informatievlaanderen/redis-populator/compare/v5.2.5...v5.2.6) (2024-02-19)


### Bug Fixes

* add extra logging + do not throw to not break current implementation ([498891d](https://github.com/informatievlaanderen/redis-populator/commit/498891d94aa9da4e0ebaac861bd636b3fd7d9ba7))

## [5.2.5](https://github.com/informatievlaanderen/redis-populator/compare/v5.2.4...v5.2.5) (2024-02-16)


### Bug Fixes

* add extra catch + logging ([42a4f20](https://github.com/informatievlaanderen/redis-populator/commit/42a4f204332c1b550cf7b904a15e90e6d04fe263))

## [5.2.4](https://github.com/informatievlaanderen/redis-populator/compare/v5.2.3...v5.2.4) (2024-02-06)


### Bug Fixes

* style to trigger build ([25fe5dc](https://github.com/informatievlaanderen/redis-populator/commit/25fe5dc7c8669d90b01c82cfa4467e00017f54d5))

## [5.2.3](https://github.com/informatievlaanderen/redis-populator/compare/v5.2.2...v5.2.3) (2024-02-06)


### Bug Fixes

* **bump:** ci ECR tagging ([2cec56e](https://github.com/informatievlaanderen/redis-populator/commit/2cec56ef5cd5b5a5b2c351442c1185f6a94d68b8))

## [5.2.2](https://github.com/informatievlaanderen/redis-populator/compare/v5.2.1...v5.2.2) (2024-02-06)


### Bug Fixes

* style to trigger bump ([677b644](https://github.com/informatievlaanderen/redis-populator/commit/677b644aebec7c02affae3d914e1f859d4acd6b7))

## [5.2.1](https://github.com/informatievlaanderen/redis-populator/compare/v5.2.0...v5.2.1) (2024-02-06)


### Bug Fixes

* **bump:** set new ECR ([d675bd6](https://github.com/informatievlaanderen/redis-populator/commit/d675bd6fd589cc351e06426c3b6d6ed3f882e98a))

# [5.2.0](https://github.com/informatievlaanderen/redis-populator/compare/v5.1.1...v5.2.0) (2023-05-15)


### Features

* add newproduction CI/CD ([430f201](https://github.com/informatievlaanderen/redis-populator/commit/430f2012523266add6e8014d3673b331aef97e57))

## [5.1.1](https://github.com/informatievlaanderen/redis-populator/compare/v5.1.0...v5.1.1) (2022-10-25)


### Bug Fixes

* handle timeout gracefully GAWR-3885 ([dcd0f6d](https://github.com/informatievlaanderen/redis-populator/commit/dcd0f6db16c7afd857afefa0b280dc28b8906a62))

# [5.1.0](https://github.com/informatievlaanderen/redis-populator/compare/v5.0.1...v5.1.0) (2022-07-08)


### Features

* add task timeout ([842c328](https://github.com/informatievlaanderen/redis-populator/commit/842c32895a96512f330384661d0c6d95958643cf))

## [5.0.1](https://github.com/informatievlaanderen/redis-populator/compare/v5.0.0...v5.0.1) (2022-03-25)


### Bug Fixes

* set runtime deps to 6.0.3 ([04c1a90](https://github.com/informatievlaanderen/redis-populator/commit/04c1a90c8129858ad0cf431fd8792aca9be55809))

# [5.0.0](https://github.com/informatievlaanderen/redis-populator/compare/v4.2.1...v5.0.0) (2022-03-25)


### Bug Fixes

* remove unnecessary destructurama paket ([96ca413](https://github.com/informatievlaanderen/redis-populator/commit/96ca41382b7e0c9412c8d0f8882760d8809820d3))


### Features

* move to dotnet 6.0.3 ([5d9aa9b](https://github.com/informatievlaanderen/redis-populator/commit/5d9aa9b93d7341746d36c5561fced37396d11fc4))


### BREAKING CHANGES

* move to dotnet 6.0.3

## [4.2.1](https://github.com/informatievlaanderen/redis-populator/compare/v4.2.0...v4.2.1) (2022-03-07)


### Bug Fixes

* add header to use e-tag ([400f027](https://github.com/informatievlaanderen/redis-populator/commit/400f0274abd29428b29488afb2b22a4a8b3bda26))

# [4.2.0](https://github.com/informatievlaanderen/redis-populator/compare/v4.1.0...v4.2.0) (2022-03-03)


### Features

* add etag also from header if available ([c5a5e70](https://github.com/informatievlaanderen/redis-populator/commit/c5a5e70e1f6afcfa7a28476176fa07c4d6eb9148))

# [4.1.0](https://github.com/informatievlaanderen/redis-populator/compare/v4.0.0...v4.1.0) (2022-01-25)


### Bug Fixes

* add httpclientfactory for v2 ([1910408](https://github.com/informatievlaanderen/redis-populator/commit/191040858a121057fe0d7c40c0b3c5724eb007a6))


### Features

* add v2 url to settings ([4fb2a12](https://github.com/informatievlaanderen/redis-populator/commit/4fb2a127fbd12fe2ad4bc84ca28e007977a356b9))

# [4.0.0](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.17...v4.0.0) (2021-11-19)


### Features

* add position to Redis store ([b72d82e](https://github.com/informatievlaanderen/redis-populator/commit/b72d82ee022bf305d176acd385b2c4fb3c08b736))


### BREAKING CHANGES

* add required parameter to RedisStore.SetAsync

## [3.1.17](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.16...v3.1.17) (2021-10-27)


### Bug Fixes

* removed default acceskey/secret ([59dc967](https://github.com/informatievlaanderen/redis-populator/commit/59dc967c56c50d78495671ba1eca33d10c4bbf60))

## [3.1.16](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.15...v3.1.16) (2021-10-06)


### Bug Fixes

* push build to ECR test ([b6696ec](https://github.com/informatievlaanderen/redis-populator/commit/b6696ec2ef22b12cc9e4349fd5dd34bb90280bb7))

## [3.1.15](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.14...v3.1.15) (2021-06-25)


### Bug Fixes

* bump aws mutex package + api ([0cb24d1](https://github.com/informatievlaanderen/redis-populator/commit/0cb24d1d3b613cae5d370db88ac3918a265ecc20))

## [3.1.14](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.13...v3.1.14) (2021-05-28)


### Bug Fixes

* move to 5.0.6 ([7d542a6](https://github.com/informatievlaanderen/redis-populator/commit/7d542a6af2f77506e68939c0de846fa003830c4c))

## [3.1.13](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.12...v3.1.13) (2021-02-02)


### Bug Fixes

* move to 5.0.2 ([531839a](https://github.com/informatievlaanderen/redis-populator/commit/531839adbbb84819bbaa9a7e2526f96d0e14627c))

## [3.1.12](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.11...v3.1.12) (2021-01-07)


### Bug Fixes

* improve performance GRAR-1673 ([701bfa4](https://github.com/informatievlaanderen/redis-populator/commit/701bfa4b72c5a6ce2cf69b5da1a38d3a48c2cdc9))

## [3.1.11](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.10...v3.1.11) (2020-12-19)


### Bug Fixes

* move to 5.0.1 ([1398928](https://github.com/informatievlaanderen/redis-populator/commit/1398928a4c0261620061bb701c308486159cbd20))

## [3.1.10](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.9...v3.1.10) (2020-11-18)


### Bug Fixes

* remove set-env usage in gh-actions ([be3a211](https://github.com/informatievlaanderen/redis-populator/commit/be3a211d9aecba9bbc76ae1253b845e0609c3bb4))

## [3.1.9](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.8...v3.1.9) (2020-11-06)


### Bug Fixes

* logging ([a61d897](https://github.com/informatievlaanderen/redis-populator/commit/a61d897df0f45edfc57273ea4e9ee6d4a9cff1b3))

## [3.1.8](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.7...v3.1.8) (2020-09-23)


### Bug Fixes

* add dbcommandtimeout to repository ([c7a08ce](https://github.com/informatievlaanderen/redis-populator/commit/c7a08ce4feff538ca93b17c8e744d210158a9c67))
* add overload to ctor repository ([ea2e4df](https://github.com/informatievlaanderen/redis-populator/commit/ea2e4df61f5660ccfaf088fe6cea3a6c911f1ae4))

## [3.1.7](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.6...v3.1.7) (2020-09-21)


### Bug Fixes

* move to 3.1.8 ([91c2d17](https://github.com/informatievlaanderen/redis-populator/commit/91c2d17bce6e7f759f700dc9ecc3554a56f8eae0))

## [3.1.6](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.5...v3.1.6) (2020-07-18)


### Bug Fixes

* move to 3.1.6 ([49d54ce](https://github.com/informatievlaanderen/redis-populator/commit/49d54cefb3e0bd72df1003d16f0af0dbf1882f42))

## [3.1.5](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.4...v3.1.5) (2020-07-18)


### Bug Fixes

* move to 3.1.6 ([cfa7118](https://github.com/informatievlaanderen/redis-populator/commit/cfa7118c18d0eb779d548694e404593428b363fc))

## [3.1.4](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.3...v3.1.4) (2020-07-02)


### Bug Fixes

* update streamstore ([16c1e40](https://github.com/informatievlaanderen/redis-populator/commit/16c1e40484374f3c2844e2ea8e714c33de5f0b99))

## [3.1.3](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.2...v3.1.3) (2020-06-19)


### Bug Fixes

* move to 3.1.5 ([df9d96c](https://github.com/informatievlaanderen/redis-populator/commit/df9d96c085e703ca68267606c13f56260bae497f))

## [3.1.2](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.1...v3.1.2) (2020-05-20)


### Bug Fixes

* 3.1.4 docker ([f0a0710](https://github.com/informatievlaanderen/redis-populator/commit/f0a0710a41627c9e88eb537c43c0a0920fcca009))

## [3.1.1](https://github.com/informatievlaanderen/redis-populator/compare/v3.1.0...v3.1.1) (2020-05-20)


### Bug Fixes

* move to GH-actions ([2c2f84a](https://github.com/informatievlaanderen/redis-populator/commit/2c2f84af26c303c71bbfd8d57cbdc0ead5f7836d))

# [3.1.0](https://github.com/informatievlaanderen/redis-populator/compare/v3.0.7...v3.1.0) (2020-05-13)


### Features

* allow debugging of redis output ([42555bf](https://github.com/informatievlaanderen/redis-populator/commit/42555bf1fa1619dabeed53216ecb6fde3abd30db))

## [3.0.7](https://github.com/informatievlaanderen/redis-populator/compare/v3.0.6...v3.0.7) (2020-04-16)


### Bug Fixes

* always store redis keys in lowercase ([0da65ff](https://github.com/informatievlaanderen/redis-populator/commit/0da65ffdcd43bfbee38200b1c28efbac588583fd))

## [3.0.6](https://github.com/informatievlaanderen/redis-populator/compare/v3.0.5...v3.0.6) (2020-03-31)


### Bug Fixes

* update package to add errormessage when invalid statuscode ([a13e4de](https://github.com/informatievlaanderen/redis-populator/commit/a13e4de4e80fdd3be4ea850b6d08e1bad4370c8b))
* use correct build user ([c3a84c9](https://github.com/informatievlaanderen/redis-populator/commit/c3a84c9b69bb282808b2e2f8b392850853a01020))

## [3.0.5](https://github.com/informatievlaanderen/redis-populator/compare/v3.0.4...v3.0.5) (2020-03-04)


### Bug Fixes

* bump netcore dockerfiles ([eecb181](https://github.com/informatievlaanderen/redis-populator/commit/eecb1810610cf909bc10bec801bf89ab20f61892))

## [3.0.4](https://github.com/informatievlaanderen/redis-populator/compare/v3.0.3...v3.0.4) (2020-03-03)


### Bug Fixes

* update dockerid detection ([10c65a4](https://github.com/informatievlaanderen/redis-populator/commit/10c65a466845d625b42a5e01c566f601bdd15753))

## [3.0.3](https://github.com/informatievlaanderen/redis-populator/compare/v3.0.2...v3.0.3) (2020-03-03)


### Bug Fixes

* update dockerid detection ([989bb33](https://github.com/informatievlaanderen/redis-populator/commit/989bb33b833f8d69ceeb3afaf953812c2c38e07d))

## [3.0.2](https://github.com/informatievlaanderen/redis-populator/compare/v3.0.1...v3.0.2) (2020-03-03)


### Bug Fixes

* bump netcore to 3.1.2 ([fc4a489](https://github.com/informatievlaanderen/redis-populator/commit/fc4a4897a44fc804c2e1c23cc412d915a69d9fb6))

## [3.0.1](https://github.com/informatievlaanderen/redis-populator/compare/v3.0.0...v3.0.1) (2020-03-02)


### Bug Fixes

* get rid of netcoreapp old ([0259497](https://github.com/informatievlaanderen/redis-populator/commit/02594978c41ba7ae7721e97979acf45d195cb053))
* update mvc json formatter ([eda0e2c](https://github.com/informatievlaanderen/redis-populator/commit/eda0e2c1e646166d3ad5b71d4a3adee82a39959c))

# [3.0.0](https://github.com/informatievlaanderen/redis-populator/compare/v2.1.0...v3.0.0) (2020-02-06)


### Features

* use distributed mutex ([58058fa](https://github.com/informatievlaanderen/redis-populator/commit/58058fae83bc9a2110df1524b9ce254fb6a6ac83))


### BREAKING CHANGES

* Use Distributed Mutex to have only a single process running

# [2.1.0](https://github.com/informatievlaanderen/redis-populator/compare/v2.0.0...v2.1.0) (2020-02-01)


### Features

* upgrade netcoreapp31 and dependencies ([0ee74f0](https://github.com/informatievlaanderen/redis-populator/commit/0ee74f01e980ececcca1af178418d7c2c95739a3))

# [2.0.0](https://github.com/informatievlaanderen/redis-populator/compare/v1.7.1...v2.0.0) (2019-12-16)


### Bug Fixes

* add x-basisregister-downstream-version to headers ([041ad7c](https://github.com/informatievlaanderen/redis-populator/commit/041ad7c94ee7852e4b56d63a22fa5cb57913c65f))
* force build ([3782c44](https://github.com/informatievlaanderen/redis-populator/commit/3782c44f0da5d42ca6dabc0b12c07065901e9356))
* force build again ([3115c2e](https://github.com/informatievlaanderen/redis-populator/commit/3115c2e71c853d8727c35efaea692f0ec5ddb518))


### Code Refactoring

* upgrade to netcoreapp30 ([594c247](https://github.com/informatievlaanderen/redis-populator/commit/594c2477467f72d6bb58c1b7e034f96fa551258e))


### Features

* upgrade to netcoreapp31 ([0581c67](https://github.com/informatievlaanderen/redis-populator/commit/0581c678fe9edb689a1c1a8e52ea98ea0cd0e2c0))


### BREAKING CHANGES

* Upgrade to .NET Core 3

## [1.7.1](https://github.com/informatievlaanderen/redis-populator/compare/v1.7.0...v1.7.1) (2019-09-13)


### Bug Fixes

* deal with errors better ([11bae58](https://github.com/informatievlaanderen/redis-populator/commit/11bae58))

# [1.7.0](https://github.com/informatievlaanderen/redis-populator/compare/v1.6.1...v1.7.0) (2019-09-12)


### Features

* keep track of how many times lastchanged has errored ([e5eb16f](https://github.com/informatievlaanderen/redis-populator/commit/e5eb16f))

## [1.6.1](https://github.com/informatievlaanderen/redis-populator/compare/v1.6.0...v1.6.1) (2019-09-04)


### Bug Fixes

* get headers correctly ([c6cc5b6](https://github.com/informatievlaanderen/redis-populator/commit/c6cc5b6))

# [1.6.0](https://github.com/informatievlaanderen/redis-populator/compare/v1.5.6...v1.6.0) (2019-09-04)


### Features

* store http headers too ([61dc113](https://github.com/informatievlaanderen/redis-populator/commit/61dc113))

## [1.5.6](https://github.com/informatievlaanderen/redis-populator/compare/v1.5.5...v1.5.6) (2019-09-03)


### Bug Fixes

* do not log to console writeline ([64a3318](https://github.com/informatievlaanderen/redis-populator/commit/64a3318))

## [1.5.5](https://github.com/informatievlaanderen/redis-populator/compare/v1.5.4...v1.5.5) (2019-09-02)


### Bug Fixes

* do not log to console writeline ([9fd6cdf](https://github.com/informatievlaanderen/redis-populator/commit/9fd6cdf))

## [1.5.4](https://github.com/informatievlaanderen/redis-populator/compare/v1.5.3...v1.5.4) (2019-08-29)


### Bug Fixes

* deal with timeouts ([8f7e860](https://github.com/informatievlaanderen/redis-populator/commit/8f7e860))

## [1.5.3](https://github.com/informatievlaanderen/redis-populator/compare/v1.5.2...v1.5.3) (2019-08-29)


### Bug Fixes

* use compact json logging ([9cd9557](https://github.com/informatievlaanderen/redis-populator/commit/9cd9557))

## [1.5.2](https://github.com/informatievlaanderen/redis-populator/compare/v1.5.1...v1.5.2) (2019-08-27)


### Bug Fixes

* make datadog tracing check more for nulls ([a4cca4b](https://github.com/informatievlaanderen/redis-populator/commit/a4cca4b))

## [1.5.1](https://github.com/informatievlaanderen/redis-populator/compare/v1.5.0...v1.5.1) (2019-08-26)


### Bug Fixes

* use fixed datadog tracing ([4a48664](https://github.com/informatievlaanderen/redis-populator/commit/4a48664))

# [1.5.0](https://github.com/informatievlaanderen/redis-populator/compare/v1.4.0...v1.5.0) (2019-08-22)


### Features

* bump to .net 2.2.6 ([7730422](https://github.com/informatievlaanderen/redis-populator/commit/7730422))

# [1.4.0](https://github.com/informatievlaanderen/redis-populator/compare/v1.3.0...v1.4.0) (2019-08-08)


### Features

* add don't save some white listed status codes BRV-108 ([65a55e2](https://github.com/informatievlaanderen/redis-populator/commit/65a55e2))

# [1.3.0](https://github.com/informatievlaanderen/redis-populator/compare/v1.2.0...v1.3.0) (2019-06-06)


### Features

* push docker to production ([627ae30](https://github.com/informatievlaanderen/redis-populator/commit/627ae30))

# [1.2.0](https://github.com/informatievlaanderen/redis-populator/compare/v1.1.1...v1.2.0) (2019-04-25)


### Features

* use .net core 2.2.4 docker image ([2bcd20d](https://github.com/informatievlaanderen/redis-populator/commit/2bcd20d))
* use .net core 2.2.4 docker image from MCR ([b4f5763](https://github.com/informatievlaanderen/redis-populator/commit/b4f5763))

## [1.1.1](https://github.com/informatievlaanderen/redis-populator/compare/v1.1.0...v1.1.1) (2019-01-14)

# [1.1.0](https://github.com/informatievlaanderen/redis-populator/compare/v1.0.0...v1.1.0) (2019-01-10)


### Features

* support basic authentication ([77886fb](https://github.com/informatievlaanderen/redis-populator/commit/77886fb))

# 1.0.0 (2018-12-18)


### Features

* open source with MIT license as 'agentschap Informatie Vlaanderen' ([120287e](https://github.com/informatievlaanderen/redis-populator/commit/120287e))
