namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests
{
    using Xunit;
    using Givens;
    using Infrastructure;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using StackExchange.Redis;
    using Fixtures;

    public class WhenRunningThePopulator : GivenThreeUnpopulatedRecordsInDb, IClassFixture<RedisPopulatorFixture>
    {
        private readonly RedisPopulatorFixture _fixture;

        public WhenRunningThePopulator(RedisPopulatorFixture fixture)
            : base(fixture.CreateInMemoryContext())
        {
            fixture.MockHttpClientHandler(Records);
            _fixture = fixture;

            var runner = new PopulatorRunner(
                new Repository(Context),
                fixture.RedisStoreFactory,
                fixture.HttpClientHandlerMock.Object,
                fixture.Configuration,
                fixture.Logger
            );

            Task.Run(() => runner.RunAsync(CancellationToken.None));
            // Allow the runner to run for 2 seconds before invoking the test methods.
            // (Simply .Wait()-ing for the runner can cause the thread to be blocked indefinitely, hence Task.Run)
            Thread.Sleep(2000);
        }

        [Fact]
        public void ThenThreeApiCallsWereMade()
        {
            foreach (var record in Records)
                _fixture
                    .HttpClientHandlerMock
                    .Verify(r => r.GetAsync($"https://foo.vlaanderen{record.Uri}", record.AcceptType), Times.AtLeastOnce);
        }

        [Fact]
        public void ThenThreeCallsToRedisBatchWereMade()
        {
            foreach (var record in Records)
                _fixture
                    .RedisBatchMock
                    .Verify(r => r.HashSetAsync(record.CacheKey, It.IsAny<HashEntry[]>(), CommandFlags.FireAndForget), Times.Once);
        }
    }
}
