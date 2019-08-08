namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests.Givens
{
    using System;
    using System.Collections.Generic;
    using Infrastructure;
    using Model;

    public class GivenOnePopulatedRecord : IDisposable
    {
        protected LastChangedListContext Context;

        protected List<LastChangedRecord> Records = new List<LastChangedRecord>
        {
            new LastChangedRecord
            {
                Id = $"{Guid.NewGuid()}.xml",
                AcceptType = "application/xml",
                CacheKey = "legacy/municipality:71018.xml",
                Uri = "/v1/gemeenten/71018",
                LastPopulatedPosition = 5000,
                Position = 5210,
                HasErrors = false
            }
        };

        protected GivenOnePopulatedRecord(LastChangedListContext context)
        {
            context.LastChangedList.AddRange(Records);
            context.SaveChanges();

            Context = context;
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}
