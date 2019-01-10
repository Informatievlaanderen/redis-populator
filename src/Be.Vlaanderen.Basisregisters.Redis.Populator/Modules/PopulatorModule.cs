namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Modules
{
    using System;
    using System.Net.Http.Headers;
    using System.Text;
    using Autofac;
    using DataDog.Tracing.Http;
    using Infrastructure;
    using Marvin.Cache.Headers;
    using Marvin.Cache.Headers.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Polly;

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
                .AddHttpClient(HttpClientHandler.ClientName, client =>
                {
                    client.BaseAddress = configuration.GetValue<Uri>("ApiBaseAddress");
                    client.DefaultRequestHeaders.Add("User-Agent", "RedisPopulator");

                    var apiUserName = configuration["ApiAuthUserName"];
                    var apiPassword = configuration["ApiAuthPassword"];
                    if (!string.IsNullOrEmpty(apiUserName) && !string.IsNullOrEmpty(apiPassword))
                    {
                        var encodedString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiUserName}:{apiPassword}"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedString);
                    }
                })

                .ConfigurePrimaryHttpMessageHandler(c =>
                    new TraceHttpMessageHandler(
                        new System.Net.Http.HttpClientHandler(),
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
                .RegisterType<HttpClientHandler>()
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
