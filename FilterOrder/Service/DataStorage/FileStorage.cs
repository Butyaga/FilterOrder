using FilterOrder.Model;
using FilterOrder.Service.Logger;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace FilterOrder.Service.DataStorage;
internal class FileStorage(string _inputFileName, string _outputFileName) : IDataStorage
{
    private readonly ILogger _logger = LogService.LogFactory.CreateLogger<FileStorage>();
    private static readonly string _currentDirectory = Directory.GetCurrentDirectory();
    private static readonly string _dateTimeformat = "yyyy-MM-dd HH:mm:ss";

    public IEnumerable<Order> GetOrders()
    {
        _logger.LogInformation("Начало чтения исходного файла данных");
        string path = _currentDirectory + "\\" + _inputFileName;
        if (!File.Exists(path))
        {
            _logger.LogError("Файл не существует: {path}", path);
            throw new Exception($"Файл не существует: {path}");
        }
        List<Order> orders = [];

        using StreamReader reader = new(path);
        string? line;
        try
        {
            while ((line = reader.ReadLine()) != null)
            {
                AddOrderCollection(line, orders);
            }
        }
        catch (Exception exc)
        {
            _logger.LogCritical(exc, "Ошибка чтения из файла: {path}", path);
            throw;
        }

        _logger.LogInformation("Успешное окончание чтения исходного файла данных");
        return orders;
    }

    public void SaveOreders(IEnumerable<Order> orders)
    {
        _logger.LogInformation("Начало записи файла данных");
        string path = _currentDirectory + "\\" + _outputFileName;

        using StreamWriter writer = new(path, false);
        try
        {
            foreach (Order order in orders)
            {
                string line = ConvertTosting(order);
                writer.WriteLine(line);
            }
        }
        catch (Exception exc)
        {
            _logger.LogCritical(exc, "Ошибка записи в файл: {path}", path);
            throw;
        }
        _logger.LogInformation("Успешное окончание записи файла данных");
    }

    private void AddOrderCollection(string line, List<Order> orders)
    {
        const int fieldCount = 4;

        string[] fields = line.Split(';');
        if (fields.Length < fieldCount)
        {
            _logger.LogWarning("В строке не достаточно полей: {line}", line);
            return;
        }

        if (!int.TryParse(fields[0], out int id))
        {
            _logger.LogWarning("Неправильный формат поля ID: {field}", fields[0]);
            return;
        }

        if (!double.TryParse(fields[1], out double weight))
        {
            _logger.LogWarning("Неправильный формат поля Weight: {field}", fields[1]);
            return;
        }

        string district = fields[2];

        if (!DateTime.TryParseExact(fields[3], _dateTimeformat, CultureInfo.InvariantCulture,
                                DateTimeStyles.AssumeLocal, out DateTime deliveryTime))
        {
            _logger.LogWarning("Неправильный формат поля DeliveryTime: {field}", fields[3]);
            return;
        }

        orders.Add(new Order(id, weight, district, deliveryTime));
    }

    private static string ConvertTosting(Order order)
    {
        string[] fields =
        [
            order.Id.ToString(),
            order.Weight.ToString("0.00"),
            order.District,
            order.DeliveryTime.ToString(_dateTimeformat),
        ];
        return string.Join(";", fields);
    }
}
