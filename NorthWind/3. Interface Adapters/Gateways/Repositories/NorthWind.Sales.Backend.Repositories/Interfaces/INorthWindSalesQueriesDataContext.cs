namespace NorthWind.Sales.Backend.Repositories.Interfaces;

public interface INorthWindSalesQueriesDataContext
{
    IQueryable<Entities.Customer> Customers { get; }
    IQueryable<Entities.Product> Products { get; }
    IQueryable<BusinessObjects.POCOEntities.Order> Orders { get; }
    IQueryable<Entities.OrderDetail> OrderDetails { get; }
    Task<ReturnType> FirstOrDefaultAync<ReturnType>(
        IQueryable<ReturnType> queryable);
    Task<IEnumerable<ReturnType>> ToListAsync<ReturnType>(
        IQueryable<ReturnType> queryable);
    Task<int> CountAsync<ReturnType>(
        IQueryable<ReturnType> queryable);
}
