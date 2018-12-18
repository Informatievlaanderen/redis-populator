namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Infrastructure
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Net.Http.Headers;

    public interface IHttpClientHandler
    {
        Task<HttpResponseMessage> GetAsync(string url, string acceptType);
    }

    public class HttpClientHandler : IHttpClientHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public const string ClientName = "ApiClient";

        public HttpClientHandler(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

        public async Task<HttpResponseMessage> GetAsync(string url, string contentType)
        {
            var client = _httpClientFactory.CreateClient(ClientName);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

            return await client.SendAsync(request);
        }
    }
}
