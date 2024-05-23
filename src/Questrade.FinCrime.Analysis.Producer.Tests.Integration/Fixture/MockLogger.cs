using Microsoft.Extensions.Logging;

namespace Questrade.FinCrime.Analysis.Producer.Tests.Integration.Fixture;

internal class MockLogger<TCategoryName> : ILogger<TCategoryName>
{
    private readonly List<LogEntry> _entries = new List<LogEntry>();

    private readonly LogLevel _minimumLevel;

    public string GetAllMessages() => GetAllMessages(null!);

    public string GetAllMessages(string separator) =>
        string.Join(
            separator ?? Environment.NewLine,
            _entries.Select(logEntry => logEntry.Message));

    public IEnumerable<LogEntry> GetMessageEntriesForSpecificLogLevel(LogLevel logLevel) =>
        _entries.Where(x => x.Level == logLevel);

    public IEnumerable<LogEntry> GetMessagesStartingLogLevel(LogLevel logLevel) =>
        _entries.Where(x => x.Level >= logLevel);

    public int Count() => _entries.Count;

    public int CountExactLevel(LogLevel logLevel) => GetMessageEntriesForSpecificLogLevel(logLevel).Count();

    public int CountFromLevel(LogLevel logLevel) => GetMessagesStartingLogLevel(logLevel).Count();

    public LogEntry? Last() => _entries.LastOrDefault();

    public MockLogger() : this(LogLevel.Trace) { }

    public MockLogger(LogLevel minimumLevel)
    {
        _minimumLevel = minimumLevel;
    }

    public IDisposable BeginScope<TState>(TState state) => new MockLoggerScope<TState>();

    public bool IsEnabled(LogLevel logLevel) => logLevel >= _minimumLevel;

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
}

internal class LogEntry
{
    public LogLevel Level { get; set; }

    public string Message { get; set; } = default!;
}

internal class MockLoggerScope<TState> : IDisposable
{
    public void Dispose() { }
}
