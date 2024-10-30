using FilterOrder.Model;

namespace FilterOrder.BL;
internal class OrderValidator
{
    private double _maxWeight;
    private List<string> _districts;

    public OrderValidator()
    {
        ValidStartTime = DateTime.Today;
        MaxTimeOffset = new(1, 0, 0, 0); // 1 день
        MaxWeight = 5.0;
        _districts = ["Сангородок", "Ветлосян", "Подгорный", "Аэропорт", "Сосновка", "УРМЗ"];
    }

    #region Properties
    internal DateTime ValidStartTime { get; set; }

    internal TimeSpan MaxTimeOffset { get; set; }

    internal IEnumerable<string> Districts
    {
        get
        {
            return _districts.AsEnumerable();
        }
        set
        {
            if (!value.Any())
            {
                throw new ArgumentException("Попытка присвоить пустую коллекцию районов");
            }
            _districts = value.ToList();
        }
    }

    internal double MaxWeight
    {
        get
        {
            return _maxWeight;
        }
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Попытка присвоить отрицательный вес");
            }
            _maxWeight = value;
        }
    }
    #endregion

    internal bool IsValid(Order order)
    {
        return IsValidWeight(order.Weight) &&
            IsValidDistrict(order.District) && IsValidDeliveryTime(order.DeliveryTime);
    }

    internal bool IsValidWeight(double weight)
    {
        return weight < _maxWeight && weight > 0;
    }

    internal bool IsValidDistrict(string district)
    {
        return _districts.Contains(district);
    }

    internal bool IsValidDeliveryTime(DateTime deliveryTime)
    {
        DateTime validEndTime = ValidStartTime.Add(MaxTimeOffset);
        return ValidStartTime < deliveryTime && deliveryTime < validEndTime;
    }
}
