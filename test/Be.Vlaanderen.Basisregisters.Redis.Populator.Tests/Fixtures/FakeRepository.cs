namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests.Fixtures
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using ProjectionHandling.LastChangedList;
    using ProjectionHandling.LastChangedList.Model;

    public class FakeRepository : IRepository
    {
        private readonly Repository _repository;

        public FakeRepository(LastChangedListContext context) => _repository = new Repository(context);

        public Task<List<LastChangedRecord>> GetUnpopulatedRecordsAsync(int limit, int maxErrorCount, DateTimeOffset maxErrorTime, CancellationToken cancellationToken)
            => _repository.GetUnpopulatedRecordsAsync(limit, maxErrorCount, maxErrorTime, cancellationToken);

        public Task SaveChangesAsync(CancellationToken cancellationToken)
            => _repository.SaveChangesAsync(cancellationToken);
    }
}
