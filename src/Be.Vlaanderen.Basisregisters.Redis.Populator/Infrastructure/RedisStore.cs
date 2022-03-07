namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Marvin.Cache.Headers;
    using Marvin.Cache.Headers.Interfaces;
    using Microsoft.Net.Http.Headers;
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
            var etag = await DetermineETag(response, headers, storeKey);
            headers.Remove(HeaderNames.ETag);

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

        private async Task<ETag> DetermineETag(string response, Dictionary<string, string[]> headers, StoreKey? storeKey)
        {
            if (headers.ContainsKey(HeaderNames.ETag))
            {
                var etagValue = headers[HeaderNames.ETag].Single();
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/ETag
                var etagType = etagValue.StartsWith("W/")
                    ? ETagType.Weak
                    : ETagType.Strong;

                return etagType == ETagType.Weak
                    ? new ETag(etagType, etagValue[2..])
                    : new ETag(etagType, etagValue);
            }

            return await _eTagGenerator.GenerateETag(storeKey, response);
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
