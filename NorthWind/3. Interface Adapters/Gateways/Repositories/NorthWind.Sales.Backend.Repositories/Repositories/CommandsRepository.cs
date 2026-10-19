namespace NorthWind.Sales.Backend.Repositories.Repositories;

internal class CommandsRepository(INorthWindSalesCommandsDataContext context) :
    ICommandsRepository
{
    public async Task CreateOrder(OrderAggregate order)
    {
        var OrderDetails = order.OrderDetails.ToArray();

        await context.AddOrderAsync(order);
        await context.AddOrderDetailsAsync(
            OrderDetails
            .Select(d => new Entities.OrderDetail
            {
                Order = order,
                ProductId = d.ProductId,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice

            }).ToArray());

        // Requirement 7: automatic inventory decrease, executed inside
        // the same transaction with optimistic concurrency (RowVersion).
        foreach (var Detail in OrderDetails)
        {
            await context.DecreaseProductStockAsync(
                Detail.ProductId, Detail.Quantity);
        }
    }

    public async Task SaveChanges() =>
        await context.SaveChangesAsync();
}
