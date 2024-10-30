namespace FilterOrder.Service.AppConfiguration;
public class ValidatorOptions
{
    public double MaxWeight { get; set; } = 5;
    public string[] Districts { get; set; } = [];
    public DateTime StartTime { get; set; } = DateTime.Now.AddYears(-1);
    public DateTime EndTime { get; set; } = DateTime.Now.AddYears(1);
}
