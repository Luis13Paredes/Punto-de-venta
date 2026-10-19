namespace NorthWind.Sales.Entities.Dtos.Search;

public class ProductResultDto(
    int id, string name, decimal unitPrice, short unitsInStock)
{
    public int Id => id;
    public string Name => name;
    public decimal UnitPrice => unitPrice;
    public short UnitsInStock => unitsInStock;
}
