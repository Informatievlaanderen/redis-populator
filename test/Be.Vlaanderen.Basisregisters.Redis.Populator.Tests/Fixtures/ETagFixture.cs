namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests.Fixtures
{
    using System.Threading.Tasks;
    using Marvin.Cache.Headers;
    using Marvin.Cache.Headers.Interfaces;
    using Moq;

    public static class ETagFixture
    {
        public static IETagGenerator MockETagGenerator()
        {
            var eTagGeneratorMock = new Mock<IETagGenerator>();

            eTagGeneratorMock
                .Setup(e => e.GenerateETag(It.IsAny<StoreKey>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new ETag(ETagType.Strong, "2a4ee7af8bb7a657")));

            return eTagGeneratorMock.Object;
        }
    }
}