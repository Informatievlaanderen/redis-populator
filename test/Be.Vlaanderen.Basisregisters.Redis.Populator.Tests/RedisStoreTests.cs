namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests
{
    using Infrastructure;
    using Xunit;
    using System;
    using Fixtures;

    public class GivenABatchWasAlreadyStarted : IClassFixture<RedisPopulatorFixture>
    {
        private readonly RedisStore _sut;

        public GivenABatchWasAlreadyStarted(RedisPopulatorFixture fixture)
        {
            _sut = new RedisStore(fixture.ConnectionMultiplexer, fixture.ETagGenerator);
            _sut.CreateBatch();
        }

        [Fact]
        public void ThenItShouldThrowAnException_WhenStartingANewBatch()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.CreateBatch());
        }
    }

    public class GiveNoBatchWasStarted : IClassFixture<RedisPopulatorFixture>
    {
        private readonly RedisStore _sut;

        public GiveNoBatchWasStarted(RedisPopulatorFixture fixture)
        {
            _sut = new RedisStore(fixture.ConnectionMultiplexer, fixture.ETagGenerator);
        }

        [Fact]
        public void ThenItShouldThrowAnException_WhenExecutingABatch()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.ExecuteBatch());
        }
    }
}
