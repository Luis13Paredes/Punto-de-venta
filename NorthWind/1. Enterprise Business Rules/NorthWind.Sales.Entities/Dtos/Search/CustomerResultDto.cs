namespace NorthWind.Sales.Entities.Dtos.Search;

public class CustomerResultDto(
    string id, string name, decimal currentBalance)
{
    public string Id => id;
    public string Name => name;
    public decimal CurrentBalance => currentBalance;
}
