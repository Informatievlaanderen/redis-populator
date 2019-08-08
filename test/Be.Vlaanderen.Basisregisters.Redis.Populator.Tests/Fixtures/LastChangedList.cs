namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests.Fixtures
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;

    public static class LastChangedList
    {
        public static LastChangedListContext CreateInMemoryContext() =>
            new LastChangedListContext(
                new DbContextOptionsBuilder<LastChangedListContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options);
    }
}