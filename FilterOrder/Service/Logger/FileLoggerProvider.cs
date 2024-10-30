using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace FilterOrder.Service.Logger;
internal class FileLoggerProvider(string _filepath) : ILoggerProvider
{
    private readonly Dictionary<string, ILogger> _loggers = [];

    public ILogger CreateLogger(string categoryName)
    {
        if (!_loggers.TryGetValue(categoryName, out ILogger? logger))
        {
            logger = new FileLogger(_filepath, categoryName);
            _loggers.Add(categoryName, logger);
        }
        return logger;
    }

    public void Dispose() { }
}
