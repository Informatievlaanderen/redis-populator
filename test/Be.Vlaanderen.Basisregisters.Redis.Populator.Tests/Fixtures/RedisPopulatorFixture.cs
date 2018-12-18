namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests.Fixtures
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using StackExchange.Redis;
    using Infrastructure;
    using Marvin.Cache.Headers;
    using Marvin.Cache.Headers.Interfaces;
    using Microsoft.Extensions.Logging;
    using Model;

    public class RedisPopulatorFixture
    {
        public IConnectionMultiplexer ConnectionMultiplexer;
        public IETagGenerator ETagGenerator;
        public IRedisStoreFactory RedisStoreFactory;
        public Mock<IHttpClientHandler> HttpClientHandlerMock;
        public Mock<IBatch> RedisBatchMock;
        public ILogger<PopulatorRunner> Logger;
        public IConfiguration Configuration;

        public RedisPopulatorFixture()
        {
            MockConnectionMultiplexer();
            MockETagGenerator();
            MockRedisStoreFactory();
            MockConfiguration();
            MockLogger();
        }

        public LastChangedListContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<LastChangedListContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new LastChangedListContext(options);
        }

        public void MockLogger()
        {
            var mock = new Mock<ILogger<PopulatorRunner>>();
            Logger = mock.Object;
        }

        public void MockHttpClientHandler(IEnumerable<LastChangedRecord> records)
        {
            var httpClientMock = new Mock<IHttpClientHandler>();

            foreach (var record in records)
            {
                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                httpResponseMessage.Content = new StringContent("foo");

                httpClientMock.Setup(h => h.GetAsync($"https://foo.vlaanderen{record.Uri}", record.AcceptType))
                    .Returns(Task.FromResult(httpResponseMessage)).Verifiable();
            }

            HttpClientHandlerMock = httpClientMock;
        }

        private void MockConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("BatchSize", "1000"),
                new KeyValuePair<string, string>("ApiBaseAddress", "https://foo.vlaanderen"),
                new KeyValuePair<string, string>("StatusCodesWhiteList:0", "200"),
                new KeyValuePair<string, string>("StatusCodesWhiteList:1", "410")
            });

            Configuration = configBuilder.Build();
        }

        private void MockRedisStoreFactory()
        {
            var redisFactoryMock = new Mock<IRedisStoreFactory>();

            redisFactoryMock.Setup(r => r.CreateRedisStore()).Returns(new RedisStore(ConnectionMultiplexer, ETagGenerator));

            RedisStoreFactory = redisFactoryMock.Object;
        }

        private void MockConnectionMultiplexer()
        {
            RedisBatchMock = new Mock<IBatch>();
            RedisBatchMock.Setup(r => r.HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<HashEntry[]>(), It.IsAny<CommandFlags>())).Verifiable();

            var databaseMock = new Mock<IDatabase>();
            databaseMock.Setup(m => m.CreateBatch(It.IsAny<object>())).Returns(RedisBatchMock.Object);

            var multiplexerMock = new Mock<IConnectionMultiplexer>();
            multiplexerMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(databaseMock.Object);

            ConnectionMultiplexer = multiplexerMock.Object;
        }

        private void MockETagGenerator()
        {
            var eTagGeneratorMock = new Mock<IETagGenerator>();
            eTagGeneratorMock.Setup(e => e.GenerateETag(It.IsAny<StoreKey>(), It.IsAny<string>())).Returns(Task.FromResult(new ETag(ETagType.Strong, "2a4ee7af8bb7a657")));

            ETagGenerator = eTagGeneratorMock.Object;
        }
    }
}
