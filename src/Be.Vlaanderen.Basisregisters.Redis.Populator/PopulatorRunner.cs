namespace Be.Vlaanderen.Basisregisters.Redis.Populator
{
    using System.Threading;
    using System;
    using System.Linq;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Net.Http;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using ProjectionHandling.LastChangedList.Model;

    public class PopulatorRunner
    {
        private readonly IRepository _repository;
        private readonly IRedisStoreFactory _redisStoreFactory;
        private readonly IHttpClientHandler _httpClient;
        private readonly ILogger<PopulatorRunner> _logger;

        private readonly int _databaseBatchSize;
        private readonly int _collectorBatchSize;
        private readonly int _maxErrorTimeInSeconds;

        private readonly IEnumerable<int> _validStatusCodes;
        private readonly IEnumerable<int> _validStatusCodesToDelete;
        private readonly IEnumerable<string> _headersToStore;
        private readonly string _apiBaseAddressV1;
        private readonly string _apiBaseAddressV2;

        public PopulatorRunner(
            IRepository repository,
            IRedisStoreFactory redisStoreFactory,
            IHttpClientHandler httpClient,
            IConfiguration configuration,
            ILogger<PopulatorRunner> logger)
        {
            _repository = repository;
            _redisStoreFactory = redisStoreFactory;
            _httpClient = httpClient;
            _logger = logger;

            _databaseBatchSize = configuration.GetValue<int?>("DatabaseBatchSize") ?? 1000;
            _collectorBatchSize = configuration.GetValue<int?>("CollectorBatchSize") ?? 100;
            _maxErrorTimeInSeconds = configuration.GetValue<int?>("MaxErrorTimeInSeconds") ?? 60;

            _validStatusCodes = configuration.GetSection("ValidStatusCodes").Get<int[]>();
            _validStatusCodesToDelete = configuration.GetSection("ValidStatusCodesToDelete").Get<int[]>();
            _headersToStore = configuration.GetSection("HeadersToStore").Get<string[]>();
            _apiBaseAddressV1 = configuration["ApiBaseAddressV1"];
            _apiBaseAddressV2 = configuration["ApiBaseAddressV2"];
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Runner is running...");

            try
            {
                var maxErrorTime = DateTimeOffset.UtcNow.AddSeconds(-1 * _maxErrorTimeInSeconds);
                var unpopulatedRecords = await _repository.GetUnpopulatedRecordsAsync(_databaseBatchSize, maxErrorTime, cancellationToken);

                while (unpopulatedRecords.Any())
                {
                    _logger.LogInformation("Processing {UnpopulatedRecords} unpopulated records.", unpopulatedRecords.Count);
                    await ProcessDatabaseBatchAsync(unpopulatedRecords, cancellationToken);

                    unpopulatedRecords = await _repository.GetUnpopulatedRecordsAsync(_databaseBatchSize, maxErrorTime, cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        private async Task ProcessDatabaseBatchAsync(IReadOnlyCollection<LastChangedRecord> unpopulatedRecords, CancellationToken cancellationToken)
        {
            try
            {
                var tasks = new List<Task>();
                var numberOfProcessedRecords = 0;

                while (numberOfProcessedRecords < unpopulatedRecords.Count)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    var batchRecords = unpopulatedRecords
                        .Skip(numberOfProcessedRecords)
                        .Take(_collectorBatchSize)
                        .ToList();

                    tasks.Add(ProcessBatchAsync(batchRecords, cancellationToken));

                    numberOfProcessedRecords += batchRecords.Count;
                }

                await Task.WhenAll(tasks);
            }
            finally
            {
                await _repository.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task ProcessBatchAsync(IEnumerable<LastChangedRecord> records, CancellationToken cancellationToken)
        {
            var redisStore = _redisStoreFactory.CreateRedisStore();

            redisStore.CreateBatch();

            foreach (var record in records)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                try
                {
                    _logger.LogInformation(
                        "Processing {Id}, Position: {Position}, LastPopulatedPosition: {LastPopulatedPosition}",
                        record.Id,
                        record.Position,
                        record.LastPopulatedPosition);

                    await UpdateRecordInRedisAsync(record, redisStore, cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    throw;
                }
            }

            redisStore.ExecuteBatch();
        }

        private async Task UpdateRecordInRedisAsync(LastChangedRecord record, RedisStore redisStore, CancellationToken cancellationToken)
        {
            var requestUrl = GetBaseUri(record.Uri) + record.Uri;

            using var response = await _httpClient.GetAsync(requestUrl, record.AcceptType ?? "application/json", cancellationToken);
            var responseStatusCode = (int)response.StatusCode;

            if (await HasInvalidStatusCode(record, redisStore, responseStatusCode, requestUrl, response))
            {
                return;
            }

            if (await EligibleForDeletion(record, redisStore, responseStatusCode, requestUrl, response))
            {
                return;
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            var responseHeaders = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            foreach (var headerToStore in _headersToStore)
            {
                var headerName = headerToStore.ToLowerInvariant();
                if (response.Headers.TryGetValues(headerName, out var headerValues))
                {
                    responseHeaders.Add(headerName, headerValues.ToArray());
                }
            }

            await redisStore.SetAsync(
                record.CacheKey?.ToLowerInvariant(),
                responseContent,
                responseStatusCode,
                responseHeaders,
                record.Position);

            record.LastPopulatedPosition = record.Position;
        }

        private string GetBaseUri(string? recordUri)
        {
            if (recordUri != null && recordUri.Contains("v2/", StringComparison.InvariantCultureIgnoreCase))
                return _apiBaseAddressV2;

            return _apiBaseAddressV1;
        }

        private async Task<bool> HasInvalidStatusCode(
            LastChangedRecord record,
            RedisStore redisStore,
            int responseStatusCode,
            string requestUrl,
            HttpResponseMessage response)
        {
            if (_validStatusCodes.Contains(responseStatusCode))
                return false;

            _logger.LogWarning(
                "Backend call to {RequestUrl} ({AcceptType}) returned statuscode {StatusCode} which was invalid.",
                requestUrl,
                record.AcceptType,
                response.StatusCode);

            record.ErrorCount++;
            record.LastError = DateTimeOffset.UtcNow;
            record.LastErrorMessage = $"Backend call to {requestUrl} ({record.AcceptType}) returned statuscode {response.StatusCode} which was invalid.";

            if (record.ErrorCount >= 10)
            {
                _logger.LogInformation(
                    "{CacheKey} reached {MaxErrorCount} errors, purging from cache.",
                    record.CacheKey.ToLowerInvariant(),
                    record.ErrorCount);

                await redisStore.DeleteKeyAsync(record.CacheKey.ToLowerInvariant());
            }

            return true;
        }

        private async Task<bool> EligibleForDeletion(
            LastChangedRecord record,
            RedisStore redisStore,
            int responseStatusCode,
            string requestUrl,
            HttpResponseMessage response)
        {
            if (!_validStatusCodesToDelete.Contains(responseStatusCode))
                return false;

            _logger.LogInformation(
                "Backend call to {RequestUrl} ({AcceptType}) returned statuscode {StatusCode} which is eligible for deletion. ({CacheKey})",
                requestUrl,
                record.AcceptType,
                response.StatusCode,
                record.CacheKey.ToLowerInvariant());

            await redisStore.DeleteKeyAsync(record.CacheKey.ToLowerInvariant());
            record.LastPopulatedPosition = record.Position;
            return true;
        }
    }
}
