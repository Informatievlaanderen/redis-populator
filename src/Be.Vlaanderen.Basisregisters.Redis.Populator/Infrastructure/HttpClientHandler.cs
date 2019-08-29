namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Infrastructure
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Net.Http.Headers;
    using System.Threading;

    public interface IHttpClientHandler
    {
        Task<HttpResponseMessage> GetAsync(string url, string acceptType, CancellationToken cancellationToken);
    }

    public class HttpClientHandler : IHttpClientHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public const string ClientName = "ApiClient";

        public HttpClientHandler(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

        public async Task<HttpResponseMessage> GetAsync(string url, string contentType, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient(ClientName);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

            try
            {
                return await client.SendAsync(request, cancellationToken);
            }
            catch (OperationCanceledException ex) when (ex.GetType() == typeof(OperationCanceledException))
            {
                // HTTP timeout throws OperationCanceledException
                // A real cancellationToken.IsCancellationRequested would throw TaskCanceledException from inside
                throw new HttpTimeoutException("Request timed out.", ex, cancellationToken);
            }
        }
    }

    public class HttpTimeoutException : OperationCanceledException
    {
        public HttpTimeoutException(string message, Exception innerException, CancellationToken token)
            : base(message, innerException, token)
        {
        }
    }
}
