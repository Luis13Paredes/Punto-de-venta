namespace NorthWind.Sales.Backend.Repositories.Entities;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public short UnitsInStock { get; set; }
    public decimal UnitPrice { get; set; }

    // Optimistic concurrency token (requirement 17): protects the
    // automatic stock decrease when several orders are saved at once.
    public byte[]? RowVersion { get; set; }
}
