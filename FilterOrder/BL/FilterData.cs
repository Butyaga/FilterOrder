using FilterOrder.Model;
using FilterOrder.Service.AppConfiguration;
using FilterOrder.Service.DataStorage;
using FilterOrder.Service.Logger;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterOrder.BL;
internal class FilterData(string sourceFileName, string destFileName)
{
    private readonly ILogger _logger = LogService.LogFactory.CreateLogger<Program>();
    private readonly IDataStorage _storage = new FileStorage(sourceFileName, destFileName);
    private readonly string _district = Configuration.Instance.District;
    private readonly DateTime _startDateTime = Configuration.Instance.DeliveryTime;
    private readonly DateTime _endDateTime = Configuration.Instance.DeliveryTime.AddMinutes(30);


    internal void Filter()
    {
        _logger.LogInformation("Начало операции фильтрации");
        IEnumerable<Order> orders = GetOrders();
        _logger.LogInformation("Данные для фильтрации получены");
        IEnumerable<Order> validOrders = Validate(orders);
        _logger.LogInformation("Данные для фильтрации прошли валидацию");
        IEnumerable<Order> filteredOrders = FilterByParameters(validOrders);
        _logger.LogInformation("Данные успешно отфильтованы");
        SaveOrders(filteredOrders);
        _logger.LogInformation("Успешная запись отфильтованных данных в исходящий файл");
        _logger.LogInformation("Успешное окончание операции фильтрации");
    }

    private IEnumerable<Order> GetOrders()
    {
        IEnumerable<Order> orders;
        try
        {
            orders = _storage.GetOrders();
        }
        catch (Exception exc)
        {
            _logger.LogCritical(exc, "Ошибка получения объетов из хранилища");
            throw;
        }
        return orders;
    }

    private void SaveOrders(IEnumerable<Order> orders)
    {
        try
        {
            _storage.SaveOreders(orders);
        }
        catch (Exception exc)
        {
            _logger.LogCritical(exc, "Ошибка записи данных в хранилище");
            throw;
        }
    }

    private static IEnumerable<Order> Validate(IEnumerable<Order> orders)
    {
        OrderValidator validator = new();
        IEnumerable<Order> validOrders = from order in orders where validator.IsValid(order) select order;
        return validOrders;
    }

    private IEnumerable<Order> FilterByParameters(IEnumerable<Order> orders)
    {
        IEnumerable<Order> fileredOrders = from order in orders where CheckOrder(order) select order;
        return fileredOrders;
    }

    private bool CheckOrder(Order order)
    {
        if (order.District == _district &&
            order.DeliveryTime > _startDateTime &&
            order.DeliveryTime < _endDateTime)
        { return true; }
        return false;
    }
}
