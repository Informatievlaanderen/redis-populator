namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Infrastructure
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using Model;

    public interface IRepository
    {
        Task<int> ClearErrors(CancellationToken cancellationToken = default);
        Task<List<LastChangedRecord>> GetUnpopulatedRecordsAsync(int limit, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public class Repository : IRepository
    {
        private readonly LastChangedListContext _context;

        public Repository(LastChangedListContext context) => _context = context;

        public async Task<int> ClearErrors(CancellationToken cancellationToken = default)
            => await _context.Database.ExecuteSqlCommandAsync(
                string.Format("UPDATE {0}.LastChangedList SET HasErrors = 0", LastChangedListContext.Schema),
                cancellationToken);

        public async Task<List<LastChangedRecord>> GetUnpopulatedRecordsAsync(int limit, CancellationToken cancellationToken)
            => await _context
                .LastChangedList
                .OrderBy(x => x.Id)
                .Where(r => r.Position > r.LastPopulatedPosition && !r.HasErrors)
                .Take(limit)
                .ToListAsync(cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _context
                .SaveChangesAsync(cancellationToken);
    }
}
