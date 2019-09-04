namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Infrastructure
{
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Helper class which provides <see cref="JsonSerializerSettings"/>.
    /// </summary>
    public static class JsonSerializerSettingsProvider
    {
        private const int DefaultMaxDepth = 32;

        /// <summary>
        /// Creates default <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <returns>Default <see cref="JsonSerializerSettings"/>.</returns>
        public static JsonSerializerSettings CreateSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new VbrContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },

                MissingMemberHandling = MissingMemberHandling.Ignore,

                // Limit the object graph we'll consume to a fixed depth. This prevents stackoverflow exceptions
                // from deserialization errors that might occur from deeply nested objects.
                MaxDepth = DefaultMaxDepth,

                // Do not change this setting
                // Setting this to None prevents Json.NET from loading malicious, unsafe, or security-sensitive types
                TypeNameHandling = TypeNameHandling.None,
            };
        }
    }

    public class VbrContractResolver : DefaultContractResolver
    {
        public bool SetStringDefaultValueToEmptyString { get; set; }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (prop.PropertyType == typeof(string) && SetStringDefaultValueToEmptyString)
                prop.DefaultValue = "";

            return prop;
        }
    }
}
