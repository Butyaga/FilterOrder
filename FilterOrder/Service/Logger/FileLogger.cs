using Microsoft.Extensions.Logging;

namespace FilterOrder.Service.Logger;
internal class FileLogger(string _filePath, string _categoryName) : ILogger
{
    private const string _timestampFormat = "yyyy-MM-ddTHH:mm:ss.fffffff";

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string timestamp = DateTime.Now.ToString(_timestampFormat);
        string message = $"{timestamp} {logLevel} [{eventId.Id}]  <{_categoryName}> {formatter(state, exception)}";
        File.AppendAllText(_filePath, message + Environment.NewLine);
    }
}
