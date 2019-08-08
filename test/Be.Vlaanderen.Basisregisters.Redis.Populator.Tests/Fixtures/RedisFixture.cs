namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests.Fixtures
{
    using System.Threading.Tasks;
    using Infrastructure;
    using Moq;
    using StackExchange.Redis;

    public static class RedisFixture
    {
        public static IConnectionMultiplexer MockConnectionMultiplexer(IBatch batch)
        {
            var databaseMock = new Mock<IDatabase>();
            databaseMock.Setup(m => m.CreateBatch(It.IsAny<object>())).Returns(batch);

            var multiplexerMock = new Mock<IConnectionMultiplexer>();
            multiplexerMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(databaseMock.Object);

            return multiplexerMock.Object;
        }

        public static Mock<IBatch> MockRedisBatch()
        {
            var redisBatchMock = new Mock<IBatch>();

            redisBatchMock
                .Setup(r => r.HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<HashEntry[]>(), It.IsAny<CommandFlags>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            redisBatchMock
                .Setup(r => r.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .Returns(Task.FromResult(true))
                .Verifiable();

            return redisBatchMock;
        }

        public static IRedisStoreFactory MockStoreFactory(IBatch batch)
        {
            var redisFactoryMock = new Mock<IRedisStoreFactory>();

            redisFactoryMock
                .Setup(r => r.CreateRedisStore())
                .Returns(
                    new RedisStore(
                        MockConnectionMultiplexer(batch),
                        ETagFixture.MockETagGenerator()));

            return redisFactoryMock.Object;
        }
    }
}
