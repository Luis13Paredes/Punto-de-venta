namespace NorthWind.Sales.Backend.DataContexts.EFCore.Services;

internal class NorthWindSalesCommandsDataContext(
    IOptions<DBOptions> dbOptions) :
    NorthWindSalesContext(dbOptions),
    INorthWindSalesCommandsDataContext
{
    public async Task AddOrderAsync(Order order) =>
        await AddAsync(order);

    public async Task AddOrderDetailsAsync(
        IEnumerable<Repositories.Entities.OrderDetail> orderDetails) =>
        await AddRangeAsync(orderDetails);

    // Requirement 7: decreases the product stock in memory; the change is
    // persisted with the rest of the order inside the same transaction.
    public async Task DecreaseProductStockAsync(
        int productId, short quantity)
    {
        var Product = await Products
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (Product == null)
            throw new UpdateException(
                $"Product with id {productId} does not exist.");

        if (Product.UnitsInStock < quantity)
            throw new UpdateException(
                $"Insufficient stock for product {productId}. " +
                $"Available: {Product.UnitsInStock}, requested: {quantity}.");

        Product.UnitsInStock -= quantity;
    }

    public async Task SaveChangesAsync() =>
        await GuardDBContext.AgainstSaveChangesErrorAsync(this);
}
