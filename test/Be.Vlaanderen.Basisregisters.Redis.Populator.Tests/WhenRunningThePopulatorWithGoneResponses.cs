namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests
{
    using Xunit;
    using Givens;
    using Infrastructure;
    using System.Threading;
    using Moq;
    using StackExchange.Redis;
    using Fixtures;
    using Xunit.Abstractions;

    public class WhenRunningThePopulatorWithGoneResponses : GivenOnePopulatedRecord, IClassFixture<GoneRedisPopulatorFixture>
    {
        private readonly GoneRedisPopulatorFixture _fixture;

        private readonly Mock<IHttpClientHandler> _httpClientHandler;
        private readonly Mock<IBatch> _redisBatch;

        public WhenRunningThePopulatorWithGoneResponses(
            GoneRedisPopulatorFixture fixture,
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
        public void ThenOneApiCallWasMade()
        {
            foreach (var record in Records)
                _httpClientHandler
                    .Verify(r => r.GetAsync($"https://{_fixture.ApiPrefix}.vlaanderen{record.Uri}", record.AcceptType), Times.AtLeastOnce);
        }

        [Fact]
        public void ThenOneCallToRedisBatchWasMade()
        {
            foreach (var record in Records)
                _redisBatch
                    .Verify(r => r.KeyDeleteAsync(record.CacheKey, CommandFlags.FireAndForget), Times.Once);
        }
    }
}
