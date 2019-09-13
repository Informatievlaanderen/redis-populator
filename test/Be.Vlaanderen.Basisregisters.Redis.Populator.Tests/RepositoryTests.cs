namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;
    using Infrastructure;
    using Givens;
    using Fixtures;
    using ProjectionHandling.LastChangedList.Model;

    // ReSharper disable InconsistentNaming
    public class WhenGettingTheUnpopulatedRecords_GivenThreeUnpopulated : GivenThreeUnpopulatedRecordsInDb
    {
        private readonly IRepository _sut;

        public WhenGettingTheUnpopulatedRecords_GivenThreeUnpopulated()
         : base (LastChangedList.CreateInMemoryContext()) => _sut = new Repository(Context);

        [Fact]
        public async Task ThenAllRecordsAreReturned()
        {
            var dbRecords = await _sut.GetUnpopulatedRecordsAsync(1000, 5, DateTimeOffset.UtcNow, CancellationToken.None);

            Assert.NotEmpty(dbRecords);
            Assert.Equal(Records.Count, dbRecords.Count);
            Assert.True(dbRecords.TrueForAll(r => r.Position > r.LastPopulatedPosition));
        }
    }

    public class WhenGettingTheUnpopulatedRecords_GivenTwoUnpopulated : GivenTwoUnpopulatedRecordsAndOnePopulatedRecordInDb
    {
        private readonly IRepository _sut;
        private LastChangedRecord _populatedRecord;

        public WhenGettingTheUnpopulatedRecords_GivenTwoUnpopulated()
            : base (LastChangedList.CreateInMemoryContext()) => _sut = new Repository(Context);

        [Fact]
        public async Task ThenAllUnpopulatedRecordsAreReturned()
        {
            var allDbRecords = Context.LastChangedList.ToList();
            Assert.Equal(Records.Count, allDbRecords.Count);

            _populatedRecord = allDbRecords.FirstOrDefault(r => r.Position == r.LastPopulatedPosition);
            Assert.NotNull(_populatedRecord);

            var unpopulatedRecords = await _sut.GetUnpopulatedRecordsAsync(1000, 5, DateTimeOffset.UtcNow, CancellationToken.None);

            Assert.NotEmpty(unpopulatedRecords);
            Assert.Equal(2, unpopulatedRecords.Count);
            Assert.True(unpopulatedRecords.TrueForAll(r => r.Position > r.LastPopulatedPosition));
            Assert.DoesNotContain(unpopulatedRecords, r => r.Id == _populatedRecord.Id);
        }
    }

    public class WhenGettingTheUnpopulatedRecords_GivenOneError : GivenOneErrorRecordInDb
    {
        private readonly IRepository _sut;

        public WhenGettingTheUnpopulatedRecords_GivenOneError()
            : base(LastChangedList.CreateInMemoryContext()) => _sut = new Repository(Context);

        [Fact]
        public async Task ThenOnlyTheRecordsWithoutErrorsAreReturned()
        {
            var maxErrorCount = 2;
            var unpopulatedRecords = await _sut.GetUnpopulatedRecordsAsync(1000, maxErrorCount, DateTimeOffset.UtcNow, CancellationToken.None);

            Assert.NotEmpty(unpopulatedRecords);
            Assert.Equal(2, unpopulatedRecords.Count);
            Assert.True(unpopulatedRecords.TrueForAll(r => r.Position > r.LastPopulatedPosition));
            Assert.True(unpopulatedRecords.TrueForAll(r => r.ErrorCount < maxErrorCount));
        }
    }

    public class WhenGettingTheUnpopulatedRecords_GivenTwoErrors : GivenTwoErrorRecordsInDb
    {
        private readonly IRepository _sut;

        public WhenGettingTheUnpopulatedRecords_GivenTwoErrors()
            : base(LastChangedList.CreateInMemoryContext()) => _sut = new Repository(Context);

        [Fact]
        public async Task ThenOnlyTheRecordsWithoutErrorsAreReturned()
        {
            var maxErrorCount = 5;
            var unpopulatedRecords = await _sut.GetUnpopulatedRecordsAsync(1000, maxErrorCount, DateTimeOffset.UtcNow.AddMinutes(-1), CancellationToken.None);

            Assert.NotEmpty(unpopulatedRecords);
            Assert.Equal(2, unpopulatedRecords.Count);
            Assert.True(unpopulatedRecords.TrueForAll(r => r.Position > r.LastPopulatedPosition));
            Assert.True(unpopulatedRecords.TrueForAll(r => r.ErrorCount < maxErrorCount));
        }
    }
}
