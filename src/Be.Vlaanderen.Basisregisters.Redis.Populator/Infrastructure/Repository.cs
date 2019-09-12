namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Infrastructure
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using ProjectionHandling.LastChangedList;
    using ProjectionHandling.LastChangedList.Model;

    public interface IRepository
    {
        Task<List<LastChangedRecord>> GetUnpopulatedRecordsAsync(int limit, int maxErrorCount, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public class Repository : IRepository
    {
        private readonly LastChangedListContext _context;

        public Repository(LastChangedListContext context) => _context = context;
        
        public async Task<List<LastChangedRecord>> GetUnpopulatedRecordsAsync(int limit, int maxErrorCount, CancellationToken cancellationToken)
            => await _context
                .LastChangedList
                .OrderBy(x => x.Id)
                .Where(r => r.Position > r.LastPopulatedPosition && r.ErrorCount < maxErrorCount)
                .Take(limit)
                .ToListAsync(cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _context
                .SaveChangesAsync(cancellationToken);
    }
}
