namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Xunit;
    using Givens;
    using Infrastructure;
    using System.Threading;
    using Moq;
    using StackExchange.Redis;
    using Fixtures;
    using Xunit.Abstractions;

    public class WhenRunningThePopulatorWithValidResponses : GivenThreeUnpopulatedRecordsInDb, IClassFixture<ValidRedisPopulatorFixture>
    {
        private readonly ValidRedisPopulatorFixture _fixture;

        private readonly Mock<IHttpClientHandler> _httpClientHandler;
        private readonly Mock<IBatch> _redisBatch;

        public WhenRunningThePopulatorWithValidResponses(
            ValidRedisPopulatorFixture fixture,
            ITestOutputHelper xUnitLogger)
            : base(LastChangedList.CreateInMemoryContext())
        {
            _fixture = fixture;
            _httpClientHandler = fixture.MockHttpClientHandler(Records);
            _redisBatch = RedisFixture.MockRedisBatch();

            var logger = FakePopulatorRunnerLogger.CreateLogger(xUnitLogger);
            var redisStoreFactory = RedisFixture.MockStoreFactory(_redisBatch.Object);
            var configuration = fixture.MockConfiguration();

            var runner = new PopulatorRunner(
                new FakeRepository(Context),
                redisStoreFactory,
                _httpClientHandler.Object,
                configuration,
                logger);

            runner.RunAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        [Fact]
        public void ThenThreeApiCallsWereMade()
        {
            foreach (var record in Records)
                _httpClientHandler
                    .Verify(r => r.GetAsync($"https://{_fixture.ApiPrefix}.vlaanderen{record.Uri}", record.AcceptType, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public void ThenThreeCallsToRedisBatchWereMade()
        {
            foreach (var record in Records)
                _redisBatch
                    .Verify(r => r.HashSetAsync(record.CacheKey, It.IsAny<HashEntry[]>(), CommandFlags.FireAndForget), Times.Once);
        }

        [Fact]
        public void ThenExtraHeadersToStoreAreSaved()
        {
            foreach (var record in Records)
                _redisBatch
                    .Verify(r => r.HashSetAsync(record.CacheKey, It.Is<HashEntry[]>(entries => entries.Any(x => x.Name.Equals("headers") && x.Value.Equals("{\"x-basisregister-version\":[\"1.42.0.0\"]}"))), CommandFlags.FireAndForget), Times.Once);
        }
    }
}
