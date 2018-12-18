namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Infrastructure
{
    using Marvin.Cache.Headers.Interfaces;
    using StackExchange.Redis;

    public interface IRedisStoreFactory
    {
        RedisStore CreateRedisStore();
    }

    public class RedisStoreFactory : IRedisStoreFactory
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IETagGenerator _eTagGenerator;

        public RedisStoreFactory(IConnectionMultiplexer redis, IETagGenerator eTagGenerator)
        {
            _redis = redis;
            _eTagGenerator = eTagGenerator;
        }

        public RedisStore CreateRedisStore() => new RedisStore(_redis, _eTagGenerator);
    }
}
