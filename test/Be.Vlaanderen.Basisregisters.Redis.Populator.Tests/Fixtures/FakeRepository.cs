namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests.Fixtures
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using Model;

    public class FakeRepository : IRepository
    {
        private readonly Repository _repository;

        public FakeRepository(LastChangedListContext context) => _repository = new Repository(context);

        public Task<int> ClearErrors(CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public Task<List<LastChangedRecord>> GetUnpopulatedRecordsAsync(int limit, CancellationToken cancellationToken = default)
            => _repository.GetUnpopulatedRecordsAsync(limit, cancellationToken);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => _repository.SaveChangesAsync(cancellationToken);
    }
}
