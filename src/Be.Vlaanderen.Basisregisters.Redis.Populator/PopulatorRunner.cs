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
        private readonly IEnumerable<int> _validStatusCodesToDelete;
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
            _validStatusCodesToDelete = configuration.GetSection("ValidStatusCodesToDelete").Get<int[]>();
            _apiBaseAddress = configuration["ApiBaseAddress"];
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Runner is running...");

            try
            {
                var unpopulatedRecords = await _repository.GetUnpopulatedRecordsAsync(_databaseBatchSize, cancellationToken);

                while (unpopulatedRecords.Any())
                {
                    _logger.LogInformation("Processing {UnpopulatedRecords} unpopulated records.", unpopulatedRecords.Count);
                    await ProcessDatabaseBatchAsync(unpopulatedRecords, cancellationToken);

                    unpopulatedRecords = await _repository.GetUnpopulatedRecordsAsync(_databaseBatchSize, cancellationToken);
                }

                var restoredRecords = await _repository.ClearErrors(cancellationToken);
                _logger.LogInformation("{RestoredRecords} were in a faulted state. These will be tried again in the next run.", restoredRecords);
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

                    var storedToRedis = await UpdateRecordInRedisAsync(record, redisStore);

                    if (storedToRedis)
                        record.LastPopulatedPosition = record.Position;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    throw;
                }
            }

            redisStore.ExecuteBatch();
        }

        private async Task<bool> UpdateRecordInRedisAsync(LastChangedRecord record, RedisStore redisStore)
        {
            var requestUrl = _apiBaseAddress + record.Uri;

            using (var response = await _httpClient.GetAsync(requestUrl, record.AcceptType))
            {
                var responseStatusCode = (int)response.StatusCode;
                if (!_validStatusCodes.Contains(responseStatusCode))
                {
                    _logger.LogWarning(
                        "Calling backend for {RequestUrl} ({AcceptType}) returned statuscode {StatusCode} which was invalid.",
                        requestUrl,
                        record.AcceptType,
                        response.StatusCode);

                    record.HasErrors = true;

                    return false;
                }

                if (_validStatusCodesToDelete.Contains(responseStatusCode))
                {
                    _logger.LogInformation(
                        "Backend for {RequestUrl} ({AcceptType}) returned statuscode {StatusCode} which is eligible for deletion. ({CacheKey})",
                        requestUrl,
                        record.AcceptType,
                        response.StatusCode,
                        record.CacheKey);

                    await redisStore.DeleteKeyAsync(record.CacheKey);
                    return true;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                await redisStore.SetAsync(record.CacheKey, responseContent, responseStatusCode);
                return true;
            }
        }
    }
}
