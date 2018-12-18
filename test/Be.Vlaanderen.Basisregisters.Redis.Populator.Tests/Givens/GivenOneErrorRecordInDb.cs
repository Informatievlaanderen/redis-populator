namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests.Givens
{
    using System;
    using System.Collections.Generic;
    using Infrastructure;
    using Model;

    public abstract class GivenOneErrorRecordInDb : IDisposable
    {
        protected LastChangedListContext Context;

        protected List<LastChangedRecord> Records = new List<LastChangedRecord>
        {
            new LastChangedRecord
            {
                Id = $"{Guid.NewGuid()}.json",
                AcceptType = "application/json",
                CacheKey = "legacy/municipality:71016.json",
                Uri = "/v1/gemeenten/71016",
                LastPopulatedPosition = 0,
                Position = 5210,
                HasErrors = true
            },
            new LastChangedRecord
            {
                Id = $"{Guid.NewGuid()}.jsonld",
                AcceptType = "application/ld+json",
                CacheKey = "legacy/municipality:71016.jsonld",
                Uri = "/v1/gemeenten/71016",
                LastPopulatedPosition = 0,
                Position = 5210,
                HasErrors = false
            },
            new LastChangedRecord
            {
                Id = $"{Guid.NewGuid()}.xml",
                AcceptType = "application/xml",
                CacheKey = "legacy/municipality:71016.xml",
                Uri = "/v1/gemeenten/71016",
                LastPopulatedPosition = 0,
                Position = 5210,
                HasErrors = false
            }
        };

        protected GivenOneErrorRecordInDb(LastChangedListContext context)
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
