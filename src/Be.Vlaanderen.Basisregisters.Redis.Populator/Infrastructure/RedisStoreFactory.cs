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
        private readonly RedisCompression _compression;

        public RedisStoreFactory(IConnectionMultiplexer redis, IETagGenerator eTagGenerator, RedisCompression compression)
        {
            _redis = redis;
            _eTagGenerator = eTagGenerator;
            _compression = compression;
        }

        public RedisStore CreateRedisStore() => new RedisStore(_redis, _eTagGenerator, _compression);
    }
}
