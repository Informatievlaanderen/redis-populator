namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Infrastructure
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using ProjectionHandling.LastChangedList;
    using ProjectionHandling.LastChangedList.Model;

    public interface IRepository
    {
        Task<List<LastChangedRecord>> GetUnpopulatedRecordsAsync(
            int limit,
            int maxErrorCount,
            DateTimeOffset maxErrorTime,
            CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }

    public class Repository : IRepository
    {
        private readonly LastChangedListContext _context;

        public Repository(LastChangedListContext context, int commandTimeoutInSeconds)
        {
            _context = context;
            _context.Database.SetCommandTimeout(commandTimeoutInSeconds);
        }

        public Repository(LastChangedListContext context) => _context = context;

        public async Task<List<LastChangedRecord>> GetUnpopulatedRecordsAsync(
            int limit,
            int maxErrorCount,
            DateTimeOffset maxErrorTime,
            CancellationToken cancellationToken)
            => await _context
                .LastChangedList
                .OrderBy(x => x.Id)
                .Where(r =>
                    r.Position > r.LastPopulatedPosition &&
                    r.ErrorCount < maxErrorCount &&
                    (r.LastError == null || r.LastError < maxErrorTime))
                .Take(limit)
                .ToListAsync(cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _context
                .SaveChangesAsync(cancellationToken);
    }
}
