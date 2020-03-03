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
