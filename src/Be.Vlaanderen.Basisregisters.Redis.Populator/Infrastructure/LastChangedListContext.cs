namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Infrastructure
{
    using Model;
    using Microsoft.EntityFrameworkCore;

    public class LastChangedListContext : DbContext
    {
        public const string Schema = "Redis";
        public const string MigrationsHistoryTable = "__EFMigrationsHistoryLastChangedList";

        public DbSet<LastChangedRecord> LastChangedList { get; set; }

        public LastChangedListContext(DbContextOptions<LastChangedListContext> dbContextOptions)
            : base (dbContextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.HasDefaultSchema(Schema);
    }
}
