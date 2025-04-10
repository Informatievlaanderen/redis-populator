namespace Be.Vlaanderen.Basisregisters.Redis.Populator
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using AspNetCore.Mvc.Formatters.Json;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Modules;
    using Newtonsoft.Json;
    using ProjectionHandling.LastChangedList;
    using Serilog;
    using Aws.DistributedMutex;

    public class Program
    {
        private static readonly AutoResetEvent Closing = new AutoResetEvent(false);
        private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public static async Task Main(string[]? args)
        {
            var ct = CancellationTokenSource.Token;

            ct.Register(() => Closing.Set());
            Console.CancelKeyPress += (sender, _) => CancellationTokenSource.Cancel();

            AppDomain.CurrentDomain.FirstChanceException += (_, eventArgs) =>
                Log.Debug(
                    eventArgs.Exception,
                    "FirstChanceException event raised in {AppDomain}.",
                    AppDomain.CurrentDomain.FriendlyName);

            AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
                Log.Fatal((Exception)eventArgs.ExceptionObject, "Encountered a fatal exception, exiting program.");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .AddCommandLine(args ?? [])
                .Build();

            var container = ConfigureServices(configuration);
            var logger = container.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Starting Be.Vlaanderen.Basisregisters.Redis.LastChangedList service.");

            var jsonSettings = JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi();
            JsonConvert.DefaultSettings = () => jsonSettings;

            var timeSpanBetweenRuns = configuration.GetValue<TimeSpan?>("TimeSpanBetweenRuns") ?? TimeSpan.FromMinutes(5);
            try
            {
                await DistributedLock<Program>.RunAsync(
                    async () =>
                    {
                        while(!ct.IsCancellationRequested)
                        {
                            var startTime = DateTime.Now;
                            try
                            {
                                var runner = container.GetRequiredService<PopulatorRunner>();

                                logger.LogInformation("Running... Press CTRL + C to exit.");

                                var timeoutInSeconds = configuration.GetValue<int?>("TaskTimeoutInSeconds") ?? 14400; //4hours (4 * 60 * 60)

                                var task = runner.RunAsync(ct);
                                await task.WaitAsync(TimeSpan.FromSeconds(Convert.ToDouble(timeoutInSeconds)), ct);
                            }
                            catch (TimeoutException)
                            {
                                Log.Error("Redis populator timed out. Cancelling task and exiting.");
                            }
                            catch (Exception e)
                            {
                                Log.Fatal(e, "Encountered a fatal exception, exiting program.");
                                throw;
                            }

                            var endTime = DateTime.Now;
                            var elapsedTime = endTime - startTime;
                            var timeSpanToDelay = timeSpanBetweenRuns.Subtract(elapsedTime);
                            if (timeSpanToDelay.TotalSeconds > 0)
                            {
                                await Task.Delay(timeSpanToDelay, ct);
                            }
                        }
                    },
                    DistributedLockOptions.LoadFromConfiguration(configuration) ?? DistributedLockOptions.Defaults,
                    logger);
            }
            catch (Exception e)
            {
                // Console.WriteLine(e.ToString());
                logger.LogCritical(e, "Encountered a fatal exception, exiting program.");
                await Log.CloseAndFlushAsync();

                // Allow some time for flushing before shutdown.
                Thread.Sleep(1000);
                throw;
            }

            logger.LogInformation("Stopping...");
            Closing.Close();
        }

        private static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            var builder = new ContainerBuilder();

            builder.RegisterModule(new LoggingModule(configuration, services));

            var tempProvider = services.BuildServiceProvider();
            var loggerFactory = tempProvider.GetRequiredService<ILoggerFactory>();

            builder
                .RegisterModule(
                    new LastChangedListModule(
                        configuration.GetConnectionString("LastChangedList")!,
                        services,
                        loggerFactory))

                .RegisterModule(
                    new PopulatorModule(configuration, services, loggerFactory))

                .RegisterModule(
                    new RedisModule(configuration, loggerFactory));

            builder.Populate(services);

            return new AutofacServiceProvider(builder.Build());
        }
    }
}
