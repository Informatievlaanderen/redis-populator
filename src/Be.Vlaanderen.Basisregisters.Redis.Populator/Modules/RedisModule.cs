namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Modules
{
    using System.IO;
    using System.Text;
    using Autofac;
    using Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using StackExchange.Redis;

    public class RedisModule : Module
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ConfigurationOptions _redisOptions;

        public RedisModule(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _redisOptions = ConfigurationOptions.Parse(configuration["Redis:ConnectionString"]!);
            _redisOptions.ClientName = configuration["Redis:ClientName"];
            _redisOptions.ReconnectRetryPolicy = new ExponentialRetry(configuration.GetValue<int?>("Redis:ReconnectRetryPolicyMilliseconds") ?? 5000);
            _redisOptions.KeepAlive = configuration.GetValue<int?>("Redis:KeepAliveSeconds") ?? 60;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var logger = _loggerFactory.CreateLogger<RedisModule>();

            var redis = ConnectionMultiplexer.Connect(_redisOptions, new LoggerTextWriter(logger));

            builder.Register<IConnectionMultiplexer>(c => redis).SingleInstance();
            builder.RegisterType<RedisStoreFactory>().As<IRedisStoreFactory>();
        }
    }

    public class LoggerTextWriter : TextWriter
    {
        private readonly ILogger _logger;

        public override Encoding Encoding => Encoding.Default;

        public LoggerTextWriter(ILogger logger) => _logger = logger;

        public override void WriteLine(string? value) => _logger.LogDebug(value);
    }
}
