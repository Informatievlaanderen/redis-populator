namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests.Fixtures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using ProjectionHandling.LastChangedList;
    using ProjectionHandling.LastChangedList.Model;

    public class FakeRepository : IRepository
    {
        private readonly LastChangedListContext _context;

        public FakeRepository(LastChangedListContext context) => _context = context;

        // NOTE: This is the implementation which is present in SQL Server, behind to ToBeIndexed property
        public async Task<List<LastChangedRecord>> GetUnpopulatedRecordsAsync(
            int limit,
            DateTimeOffset maxErrorTime,
            CancellationToken cancellationToken)
            => await _context
                .LastChangedList
                .OrderBy(x => x.Id)
                .Where(r =>
                    r.Position > r.LastPopulatedPosition &&
                    r.ErrorCount < 10 &&
                    (r.LastError == null || r.LastError < maxErrorTime))
                .Take(limit)
                .ToListAsync(cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _context
                .SaveChangesAsync(cancellationToken);
    }
}
