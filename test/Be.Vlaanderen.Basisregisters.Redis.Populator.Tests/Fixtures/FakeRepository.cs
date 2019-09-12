namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests.Fixtures
{
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

        public Task<List<LastChangedRecord>> GetUnpopulatedRecordsAsync(int limit, int maxErrorCount, CancellationToken cancellationToken = default)
            => _repository.GetUnpopulatedRecordsAsync(limit, maxErrorCount, cancellationToken);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => _repository.SaveChangesAsync(cancellationToken);
    }
}
