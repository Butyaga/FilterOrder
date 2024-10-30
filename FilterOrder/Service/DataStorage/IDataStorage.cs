using FilterOrder.Model;

namespace FilterOrder.Service.DataStorage;
public interface IDataStorage
{
    public IEnumerable<Order> GetOrders();
    public void SaveOreders(IEnumerable<Order> orders);
}
