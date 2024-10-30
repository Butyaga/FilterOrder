using FilterOrder.BL;
using FilterOrder.Service.AppConfiguration;
using FilterOrder.Service.Logger;
using Microsoft.Extensions.Logging;

namespace FilterOrder;
internal class Program
{
    const string _logFileName = "app.log";
    const string _configurationFileName = "settings.json";
    const string _parameterFileName = "params.json";
    const string _inputFileName = "input.txt";
    const string _outputFileName = "output.txt";
    const string _commandGenarateInputFile = "--generate";

    private static ILogger _logger = null!;

    static void Main(string[] args)
    {
        SetLogging();
        _logger.LogInformation("Начало логирования программы.");
        
        if (!SetConfiguration(args))
        {
            return;
        }

        if (args.Contains(_commandGenarateInputFile))
        {
            GenerateInputFile();
            return;
        }

        FilterData();
    }

    private static void SetLogging()
    {
        LogService.CreateLogFactory(_logFileName);
        _logger = LogService.LogFactory.CreateLogger<Program>();
    }

    private static bool SetConfiguration(string[] args)
    {
        try
        {
            Configuration.Instance.Populate([_configurationFileName, _parameterFileName], args);
        }
        catch (Exception exc)
        {
            _logger.LogCritical(exc, "Ошибка считывании файлов конфигурации или параметров.");
            Console.WriteLine("Ошибка считывании файлов конфигурации или параметров: {0}", exc.Message);
            return false;
        }
        _logger.LogInformation("Успешное применение параметров.");
        return true;
    }

    private static void GenerateInputFile()
    {
        _logger.LogInformation("Начало генерации исходного файла данных.");
        SourceFileGenerator generator = new(_inputFileName);
        try
        {
            generator.Generate(DateTime.Now, new TimeSpan(0, 1, 0, 0));
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Генерация исходного файла данных завершилась ошибкой.");
            Console.WriteLine("Генерация исходного файла данных завершилась ошибкой: {0}", exc.Message);
            return;
        }
        _logger.LogInformation("Успешное завершение генерации исходного файла данных.");
        Console.WriteLine("Успешное завершение генерации исходного файла данных.");
    }

    private static void FilterData()
    {
        _logger.LogInformation("Начало фильтрации исходного файла данных.");
        FilterData generator = new(_inputFileName, _outputFileName);
        try
        {
            generator.Filter();
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Фильтрация исходного файла данных завершилась ошибкой.");
            Console.WriteLine("Фильтрация исходного файла данных завершилась ошибкой: {0}", exc.Message);
            return;
        }
        _logger.LogInformation("Успешное завершение фильтрации исходного файла данных.");
        Console.WriteLine("Успешное завершение фильтрации исходного файла данных.");
    }
}
