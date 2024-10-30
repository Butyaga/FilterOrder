using Microsoft.Extensions.Logging;

namespace FilterOrder.Service.Logger;
internal static class LogService
{
    private static ILoggerFactory? _logFactory;

    internal static ILoggerFactory LogFactory
    {
        get
        {
            _logFactory ??= LoggerFactory.Create(builder => builder.AddFile("app.log"));
            return _logFactory;
        }
        private set
        {  _logFactory = value; }
    }

    internal static void CreateLogFactory(string fileName)
    {
        LogFactory = LoggerFactory.Create(builder => builder.AddFile(fileName));
    }
}
