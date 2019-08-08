namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests.Fixtures
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using Infrastructure;
    using Model;

    public abstract class RedisPopulatorFixture
    {
        public string ApiPrefix { get; }
        public HttpStatusCode ApiResponseStatusCode { get; }

        protected RedisPopulatorFixture(
            string apiPrefix,
            HttpStatusCode apiResponseStatusCode)
        {
            ApiPrefix = apiPrefix;
            ApiResponseStatusCode = apiResponseStatusCode;
        }

        public IConfigurationRoot MockConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("BatchSize", "1000"),
                new KeyValuePair<string, string>("ApiBaseAddress", $"https://{ApiPrefix}.vlaanderen"),
                new KeyValuePair<string, string>("ValidStatusCodes:0", "200"),
                new KeyValuePair<string, string>("ValidStatusCodes:1", "410"),
                new KeyValuePair<string, string>("ValidStatusCodesToDelete:0", "410")
            });

            return configBuilder.Build();
        }

        public Mock<IHttpClientHandler> MockHttpClientHandler(IEnumerable<LastChangedRecord> records)
        {
            var httpClientMock = new Mock<IHttpClientHandler>();

            foreach (var record in records)
            {
                var validHttpResponseMessage = new HttpResponseMessage(ApiResponseStatusCode) { Content = new StringContent(ApiPrefix) };

                httpClientMock.Setup(h => h.GetAsync($"https://{ApiPrefix}.vlaanderen{record.Uri}", record.AcceptType))
                    .Returns(Task.FromResult(validHttpResponseMessage)).Verifiable();
            }

            return httpClientMock;
        }
    }

    public class ValidRedisPopulatorFixture : RedisPopulatorFixture
    {
        public ValidRedisPopulatorFixture() : base("valid", HttpStatusCode.OK)
        {
        }
    }

    public class GoneRedisPopulatorFixture : RedisPopulatorFixture
    {
        public GoneRedisPopulatorFixture() : base("gone", HttpStatusCode.Gone)
        {
        }
    }
}
