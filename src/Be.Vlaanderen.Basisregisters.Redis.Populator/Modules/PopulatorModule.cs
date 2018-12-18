namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Modules
{
    using System;
    using Autofac;
    using DataDog.Tracing.Http;
    using Infrastructure;
    using Marvin.Cache.Headers;
    using Marvin.Cache.Headers.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Polly;
    using HttpClientHandler = System.Net.Http.HttpClientHandler;

    public class PopulatorModule : Module
    {
        private readonly IConfiguration _configuration;

        public PopulatorModule(
            IConfiguration configuration,
            IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<PopulatorModule>();

            _configuration = configuration;

            // https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory
            services
                .AddHttpClient(Infrastructure.HttpClientHandler.ClientName, client =>
                {
                    client.BaseAddress = configuration.GetValue<Uri>("ApiBaseAddress");
                    client.DefaultRequestHeaders.Add("User-Agent", "RedisPopulator");
                })

                .ConfigurePrimaryHttpMessageHandler(c =>
                    new TraceHttpMessageHandler(
                        new HttpClientHandler(),
                        configuration["DataDog:ServiceName"]))

                // HttpRequestException, HTTP 5XX, and HTTP 408
                .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder
                    .WaitAndRetryAsync(
                        5,
                        retryAttempt =>
                        {
                            var value = Math.Pow(2, retryAttempt) / 4;
                            var randomValue = new Random().Next((int)value * 3, (int)value * 5);
                            logger?.LogInformation("Retrying after {Seconds} seconds...", randomValue);
                            return TimeSpan.FromSeconds(randomValue);
                        }));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterInstance(_configuration)
                .As<IConfiguration>();

            builder
                .RegisterType<Repository>()
                .As<IRepository>();

            builder
                .RegisterType<Infrastructure.HttpClientHandler>()
                .As<IHttpClientHandler>();

            builder
                .RegisterType<DefaultStrongETagGenerator>()
                .As<IETagGenerator>();

            builder
                .RegisterType<PopulatorRunner>()
                .AsSelf()
                .SingleInstance();
        }
    }
}
