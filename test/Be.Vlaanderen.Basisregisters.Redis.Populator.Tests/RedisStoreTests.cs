namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests
{
    using Infrastructure;
    using Xunit;
    using System;
    using Fixtures;

    public class GivenABatchWasAlreadyStarted
    {
        private readonly RedisStore _sut;

        public GivenABatchWasAlreadyStarted()
        {
            _sut = new RedisStore(
                RedisFixture.MockConnectionMultiplexer(RedisFixture.MockRedisBatch().Object),
                ETagFixture.MockETagGenerator());

            _sut.CreateBatch();
        }

        [Fact]
        public void ThenItShouldThrowAnException_WhenStartingANewBatch()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.CreateBatch());
        }
    }

    public class GiveNoBatchWasStarted
    {
        private readonly RedisStore _sut;

        public GiveNoBatchWasStarted()
        {
            _sut = new RedisStore(
                RedisFixture.MockConnectionMultiplexer(RedisFixture.MockRedisBatch().Object),
                ETagFixture.MockETagGenerator());
        }

        [Fact]
        public void ThenItShouldThrowAnException_WhenExecutingABatch()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.ExecuteBatch());
        }
    }
}
