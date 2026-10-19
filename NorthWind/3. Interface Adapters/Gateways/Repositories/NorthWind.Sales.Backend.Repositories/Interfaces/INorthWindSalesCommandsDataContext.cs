namespace NorthWind.Sales.Backend.Repositories.Interfaces;

public interface INorthWindSalesCommandsDataContext
{
    Task AddOrderAsync(Order order);
    Task AddOrderDetailsAsync(IEnumerable<Entities.OrderDetail> orderDetails);

    // Requirement 7: automatic inventory decrease when an order is saved.
    Task DecreaseProductStockAsync(int productId, short quantity);

    Task SaveChangesAsync();
}
