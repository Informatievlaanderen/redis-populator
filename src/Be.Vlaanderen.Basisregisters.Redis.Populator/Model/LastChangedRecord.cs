namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Model
{
    public class LastChangedRecord
    {
        public string Id { get; set; }
        public string CacheKey { get; set; }
        public string Uri { get; set; }
        public string AcceptType { get; set; }
        public long Position { get; set; }
        public long LastPopulatedPosition { get; set; }
        public bool HasErrors { get; set; }
    }
}
