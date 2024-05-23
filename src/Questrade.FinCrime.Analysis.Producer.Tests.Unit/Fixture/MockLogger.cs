using Microsoft.Extensions.Logging;

namespace Questrade.FinCrime.Analysis.Producer.Tests.Unit.Fixture;

internal class MockLogger<TCategoryName> : ILogger<TCategoryName>
{
    private readonly List<LogEntry> _entries = new();

    private readonly LogLevel _minimumLevel;

    public MockLogger() : this(LogLevel.Trace)
    {
    }

    public MockLogger(LogLevel minimumLevel)
    {
        _minimumLevel = minimumLevel;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return new MockLoggerScope<TState>();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _minimumLevel;
    }

    public void Log<TState>
    (
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception, string> formatter
    )
    {
        if (IsEnabled(logLevel))
            _entries.Add(new LogEntry
                { Level = logLevel, Message = exception?.Message + formatter(state, exception!) });
    }

    public string GetAllMessages()
    {
        return GetAllMessages(null!);
    }

    public string GetAllMessages(string separator)
    {
        return string.Join(
            separator ?? Environment.NewLine,
            _entries.Select(logEntry => logEntry.Message));
    }

    public IEnumerable<LogEntry> GetMessageEntriesForSpecificLogLevel(LogLevel logLevel)
    {
        return _entries.Where(x => x.Level == logLevel);
    }

    public IEnumerable<LogEntry> GetMessagesStartingLogLevel(LogLevel logLevel)
    {
        return _entries.Where(x => x.Level >= logLevel);
    }

    public int Count()
    {
        return _entries.Count;
    }

    public int CountExactLevel(LogLevel logLevel)
    {
        return GetMessageEntriesForSpecificLogLevel(logLevel).Count();
    }

    public int CountFromLevel(LogLevel logLevel)
    {
        return GetMessagesStartingLogLevel(logLevel).Count();
    }

    public LogEntry? Last()
    {
        return _entries.LastOrDefault();
    }
}

internal class LogEntry
{
    public LogLevel Level { get; set; }

    public string Message { get; set; } = default!;
}

internal class MockLoggerScope<TState> : IDisposable
{
    public void Dispose()
    {
    }
}
