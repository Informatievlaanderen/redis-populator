namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Marvin.Cache.Headers;
    using Marvin.Cache.Headers.Interfaces;
    using Newtonsoft.Json;
    using StackExchange.Redis;

    public class RedisStore
    {
        public const string ETagKey = "eTag";
        public const string ETagTypeKey = "eTagType";
        public const string LastModifiedKey = "lastModified";
        public const string SetByRegistryKey = "setByRegistry";
        public const string ValueKey = "value";
        public const string HeadersKey = "headers";
        public const string ResponseStatusCodeKey = "responseStatusCode";
        public const string PositionKey = "position";

        private readonly IConnectionMultiplexer _redis;
        private readonly IETagGenerator _eTagGenerator;

        private IBatch _batch;
        private bool _batchInProgress;

        public RedisStore(IConnectionMultiplexer redis, IETagGenerator eTagGenerator)
        {
            _redis = redis;
            _eTagGenerator = eTagGenerator;
        }

        public void CreateBatch()
        {
            if (_batchInProgress)
                throw new InvalidOperationException("You must execute a batch before creating a new one!");

            _batch = _redis.GetDatabase().CreateBatch();
            _batchInProgress = true;
        }

        public void ExecuteBatch()
        {
            if (!_batchInProgress)
                throw new InvalidOperationException("You must create a batch before executing it!");

            _batch.Execute();
            _batchInProgress = false;
            _batch = null;
        }

        public async Task SetAsync(
            RedisKey key,
            string response,
            int responseStatusCode,
            Dictionary<string, string[]> headers,
            long position)
        {
            var storeKey = new StoreKey {{ "key", key }};
            var etag = await _eTagGenerator.GenerateETag(storeKey, response);

            var hashFields = new[]
            {
                new HashEntry(ETagKey, etag.Value),
                new HashEntry(ETagTypeKey, etag.ETagType.ToString()),
                new HashEntry(LastModifiedKey, DateTime.Now.ToString("O", CultureInfo.InvariantCulture)),
                new HashEntry(SetByRegistryKey, true.ToString(CultureInfo.InvariantCulture)),
                new HashEntry(ValueKey, response),
                new HashEntry(HeadersKey, JsonConvert.SerializeObject(headers)),
                new HashEntry(ResponseStatusCodeKey, responseStatusCode),
                new HashEntry(PositionKey, position)
            };

            if (_batch == null)
            {
                var db = _redis.GetDatabase();
                await db.HashSetAsync(key, hashFields, CommandFlags.FireAndForget);
            }
            else
            {
                await _batch.HashSetAsync(key, hashFields, CommandFlags.FireAndForget);
            }
        }

        public async Task DeleteKeyAsync(RedisKey key)
        {
            if (_batch == null)
            {
                var db = _redis.GetDatabase();
                await db.KeyDeleteAsync(key, CommandFlags.FireAndForget);
            }
            else
            {
                await _batch.KeyDeleteAsync(key, CommandFlags.FireAndForget);
            }
        }
    }
}
