using FilterOrder.Model;
using FilterOrder.Service.AppConfiguration;
using FilterOrder.Service.DataStorage;
using FilterOrder.Service.Logger;
using Microsoft.Extensions.Logging;

namespace FilterOrder.BL;
internal class SourceFileGenerator(string _fileName)
{
    private readonly ILogger _logger = LogService.LogFactory.CreateLogger<Program>();
    private readonly IDataStorage _storage = new FileStorage(_fileName, _fileName);
    private readonly double _maxWeight = Configuration.Instance.ValidOptions.MaxWeight;
    private readonly string[] _districts = Configuration.Instance.ValidOptions.Districts;

    internal void Generate(DateTime startDateTime, TimeSpan timeSpan, int count = 0)
    {
        List<Order> orders = GetRandomOrders(count, startDateTime, timeSpan);

        try
        {
            _storage.SaveOreders(orders);
        }
        catch (Exception exc)
        {
            _logger.LogCritical(exc, "Ошибка сохранения коллекции объектов");
            throw;
        }
    }

    private List<Order> GetRandomOrders(int count, DateTime startDateTime, TimeSpan timeSpan)
    {
        Random rnd = new();
        int orderID = 1;
        List<Order> orders = [];
        int districtCount = _districts.Length;
        if (count == 0) { count = districtCount * 10; }

        for (int i = 0; i < count; i++)
        {
            double randomWeight = _maxWeight * rnd.NextDouble();
            string randomDistrict = _districts[rnd.Next(districtCount)];
            DateTime randomDeliveryTime = startDateTime.AddMinutes(rnd.NextDouble() * timeSpan.TotalMinutes);
            Order order = new(orderID++, randomWeight, randomDistrict, randomDeliveryTime);
            orders.Add(order);
        }

        return orders;
    }
}
