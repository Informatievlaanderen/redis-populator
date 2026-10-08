namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests
{
    using Infrastructure;
    using Xunit;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fixtures;
    using Moq;
    using StackExchange.Redis;
    using ZstdSharp;

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

    public class GivenARecordIsStored
    {
        private const string Response = "{\"identificator\":{\"id\":\"https://data.vlaanderen.be/id/adres/1\"}}";

        private static async Task<HashEntry[]> StoreRecord(RedisCompression compression)
        {
            HashEntry[] storedEntries = [];

            var batch = RedisFixture.MockRedisBatch();
            batch
                .Setup(r => r.HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<HashEntry[]>(), It.IsAny<CommandFlags>()))
                .Callback<RedisKey, HashEntry[], CommandFlags>((_, entries, _) => storedEntries = entries)
                .Returns(Task.CompletedTask);

            var sut = new RedisStore(
                RedisFixture.MockConnectionMultiplexer(batch.Object),
                ETagFixture.MockETagGenerator(),
                compression);

            sut.CreateBatch();
            await sut.SetAsync("key", Response, 200, new Dictionary<string, string[]>(), 1);
            sut.ExecuteBatch();

            return storedEntries;
        }

        private static RedisValue GetField(HashEntry[] entries, string name)
            => entries.Single(x => x.Name == name).Value;

        [Fact]
        public async Task WithoutCompression_ThenValueIsStoredAsIsWithCompressionNone()
        {
            var entries = await StoreRecord(RedisCompression.None);

            Assert.Equal(RedisStore.CompressionNone, GetField(entries, RedisStore.CompressionKey).ToString());
            Assert.Equal(Response, GetField(entries, RedisStore.ValueKey).ToString());
        }

        [Fact]
        public async Task WithZstdCompression_ThenValueIsZstdCompressedWithCompressionZstd()
        {
            var entries = await StoreRecord(RedisCompression.Zstd);

            Assert.Equal(RedisStore.CompressionZstd, GetField(entries, RedisStore.CompressionKey).ToString());

            using var decompressor = new Decompressor();
            var decompressed = decompressor.Unwrap((byte[])GetField(entries, RedisStore.ValueKey)!).ToArray();
            Assert.Equal(Response, Encoding.UTF8.GetString(decompressed));
        }
    }
}
