namespace Be.Vlaanderen.Basisregisters.Redis.Populator.Tests.Fixtures
{
    using System;
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;

    public class FakePopulatorRunnerLogger : ILogger<PopulatorRunner>
    {
        private readonly ITestOutputHelper _xUnitLogger;

        public FakePopulatorRunnerLogger(ITestOutputHelper xUnitLogger) => _xUnitLogger = xUnitLogger;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            => _xUnitLogger.WriteLine(formatter(state, exception));

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();

        public static FakePopulatorRunnerLogger CreateLogger(ITestOutputHelper xUnitLogger)
            => new FakePopulatorRunnerLogger(xUnitLogger);
    }
}
