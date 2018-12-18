namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Modules
{
    using Autofac;
    using Infrastructure;
    using Microsoft.Extensions.Configuration;
    using StackExchange.Redis;

    public class RedisModule : Module
    {
        private readonly ConfigurationOptions _redisOptions;

        public RedisModule(IConfiguration configuration)
        {
            _redisOptions = ConfigurationOptions.Parse(configuration["Redis:ConnectionString"]);
            _redisOptions.ClientName = configuration["Redis:ClientName"];
            _redisOptions.ReconnectRetryPolicy = new ExponentialRetry(configuration.GetValue<int?>("Redis:ReconnectRetryPolicyMilliseconds") ?? 5000);
            _redisOptions.KeepAlive = configuration.GetValue<int?>("Redis:KeepAliveSeconds") ?? 60;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var redis = ConnectionMultiplexer.Connect(_redisOptions);

            builder.Register<IConnectionMultiplexer>(c => redis).SingleInstance();
            builder.RegisterType<RedisStoreFactory>().As<IRedisStoreFactory>();
        }
    }
}
