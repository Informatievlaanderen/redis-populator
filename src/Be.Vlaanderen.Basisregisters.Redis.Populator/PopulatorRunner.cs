namespace Be.Vlaanderen.Basisregisters.Redis.Populator
{
    using System.Threading;
    using System;
    using System.Linq;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Model;

    public class PopulatorRunner
    {
        private readonly IRepository _repository;
        private readonly IRedisStoreFactory _redisStoreFactory;
        private readonly IHttpClientHandler _httpClient;
        private readonly ILogger<PopulatorRunner> _logger;

        private readonly int _databaseBatchSize;
        private readonly int _collectorBatchSize;

        private readonly IEnumerable<int> _validStatusCodes;
        private readonly IEnumerable<int> _validStatusCodesToIgnore;
        private readonly string _apiBaseAddress;

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

            _validStatusCodes = configuration.GetSection("ValidStatusCodes").Get<int[]>();
            _validStatusCodesToIgnore = configuration.GetSection("ValidStatusCodesToIgnore").Get<int[]>();
            _apiBaseAddress = configuration["ApiBaseAddress"];
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Runner is running...");

            var unpopulatedRecords = await _repository.GetUnpopulatedRecordsAsync(_databaseBatchSize, cancellationToken);

            while (unpopulatedRecords.Any())
            {
                _logger.LogInformation("Processing {UnpopulatedRecords} unpopulated records.", unpopulatedRecords.Count);
                await ProcessDatabaseBatchAsync(unpopulatedRecords, cancellationToken);

                unpopulatedRecords = await _repository.GetUnpopulatedRecordsAsync(_databaseBatchSize, cancellationToken);
            }

            var restoredRecords = await _repository.ClearErrors(cancellationToken);
            _logger.LogInformation($"{restoredRecords} were in a faulted state. These will be tried again in the next run.");
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
                    _logger.LogInformation($"Processing {record.Id}, Position: {record.Position}, LastPopulatedPosition: {record.LastPopulatedPosition}");

                    var storedToRedis = await SendRecordToRedisAsync(record, redisStore);

                    if (storedToRedis)
                        record.LastPopulatedPosition = record.Position;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }

            redisStore.ExecuteBatch();
        }

        private async Task<bool> SendRecordToRedisAsync(LastChangedRecord record, RedisStore redisStore)
        {
            var requestUrl = _apiBaseAddress + record.Uri;

            using (var response = await _httpClient.GetAsync(requestUrl, record.AcceptType))
            {
                var responseStatusCode = (int)response.StatusCode;
                if (!_validStatusCodes.Contains(responseStatusCode))
                {
                    _logger.LogWarning($"Calling backend for {requestUrl} ({record.AcceptType}) returned statuscode {response.StatusCode} which was invalid.");
                    record.HasErrors = true;

                    return false;
                }

                if (_validStatusCodesToIgnore.Contains(responseStatusCode))
                    return true;

                var responseContent = await response.Content.ReadAsStringAsync();
                await redisStore.SetAsync(record.CacheKey, responseContent, responseStatusCode);
                return true;
            }
        }
    }
}
