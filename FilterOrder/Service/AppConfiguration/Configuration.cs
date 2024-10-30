using FilterOrder.Service.Logger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FilterOrder.Service.AppConfiguration;
internal class Configuration()
{
    private static readonly ILogger _logger = LogService.LogFactory.CreateLogger<Configuration>();
    internal static Configuration Instance { get; } = new Configuration();

    internal ValidatorOptions ValidOptions { get; private set; } = new();
    internal string District { get; private set; } = string.Empty;
    internal DateTime DeliveryTime { get; private set; }

    internal void Populate(string[] fileNames, string[] args)
    {
        _logger.LogInformation("Начало загрузки параметров конфигурации");
        IConfiguration config = LoadConfiguration(fileNames, args);
        SetValidOptions(config);
        SetParameters(config);
        _logger.LogInformation("Успешное окончание загрузки параметров конфигурации");
    }

    #region private methods
    private static IConfiguration LoadConfiguration(string[] fileNames, string[] args)
    {
        IConfigurationBuilder configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
        foreach (var fileName in fileNames)
        {
            configBuilder.AddJsonFile(fileName);
        }

        IConfiguration config;
        try
        {
            config = configBuilder.AddCommandLine(args).Build();
        }
        catch (Exception exc)
        {
            _logger.LogCritical(exc, "Ошибка при считывании файлов конфигурации", fileNames);
            throw;
        }

        _logger.LogInformation("Файлы конфигурации успешно считаны", fileNames);
        return config;
    }

    private void SetValidOptions(IConfiguration config)
    {
        ValidatorOptions? options;
        try
        {
            IConfigurationSection? section = config.GetSection("ValidatorOptions");
            options = section.Get<ValidatorOptions>();
        }
        catch (Exception exc)
        {
            _logger.LogCritical(exc, "Ошибка при считывании опций валидатора из файлов конфигурации.");
            throw;
        }

        if (options is not null)
        {
            ValidOptions = options;
        }
    }

    private void SetParameters(IConfiguration config)
    {
        string? district = config.GetValue<string>("district");
        if (string.IsNullOrEmpty(district) || !ValidOptions.Districts.Contains(district))
        {
            const string msg = "Ошибка при считывании параметра \"district\"";
            _logger.LogCritical(msg);
            throw new Exception(msg);
        }
        District = district;

        DateTime deliveryTime;
        try
        {
            deliveryTime = config.GetValue("deliverytime", DateTime.Now);
        }
        catch (Exception exc)
        {
            _logger.LogCritical(exc, "Ошибка при считывании параметра \"deliverytime\"");
            throw;
        }
        DeliveryTime = deliveryTime;
    }
    #endregion
}
