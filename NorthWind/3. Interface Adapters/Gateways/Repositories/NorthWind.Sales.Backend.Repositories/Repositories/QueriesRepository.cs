namespace NorthWind.Sales.Backend.Repositories.Repositories;

internal class QueriesRepository(INorthWindSalesQueriesDataContext context) :
    IQueriesRepository
{
    public async Task<decimal?> GetCustomerCurrentBalance(
        string customerId)
    {
        var Queryable = context.Customers
            .Where(c => c.Id == customerId)
            .Select(c => new { c.CurrentBalance });

        var Result = await context.FirstOrDefaultAync(Queryable);

        return Result?.CurrentBalance;
    }

    public async Task<IEnumerable<ProductUnitsInStock>>
        GetProductsUnitsInStock(IEnumerable<int> productIds)
    {
        var Queryable = context.Products
            .Where(p => productIds.Contains(p.Id))
            .Select(p => new ProductUnitsInStock(
                p.Id, p.UnitsInStock));

        return await context.ToListAsync(Queryable);
    }

    // Requirement 15: search customers by id or name with pagination.
    public async Task<PagedResultDto<CustomerResultDto>> SearchCustomers(
        string? searchTerm, int page, int pageSize)
    {
        (page, pageSize) = NormalizePaging(page, pageSize);

        var Queryable = context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string Term = searchTerm.Trim();
            Queryable = Queryable.Where(c =>
                c.Id.Contains(Term) || c.Name.Contains(Term));
        }

        int TotalCount = await context.CountAsync(Queryable);

        var Items = await context.ToListAsync(
            Queryable
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerResultDto(
                    c.Id, c.Name, c.CurrentBalance)));

        return new PagedResultDto<CustomerResultDto>(
            Items.ToList(), page, pageSize, TotalCount);
    }

    // Requirement 15 + 8: search products by id or name with
    // pagination, showing only products with stock greater than zero.
    public async Task<PagedResultDto<ProductResultDto>> SearchProducts(
        string? searchTerm, int page, int pageSize)
    {
        (page, pageSize) = NormalizePaging(page, pageSize);

        var Queryable = context.Products
            .Where(p => p.UnitsInStock > 0);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string Term = searchTerm.Trim();
            Queryable = Queryable.Where(p =>
                p.Name.Contains(Term) ||
                p.Id.ToString().Contains(Term));
        }

        int TotalCount = await context.CountAsync(Queryable);

        var Items = await context.ToListAsync(
            Queryable
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductResultDto(
                    p.Id, p.Name, p.UnitPrice, p.UnitsInStock)));

        return new PagedResultDto<ProductResultDto>(
            Items.ToList(), page, pageSize, TotalCount);
    }

    // Requirement 12: rebuild a saved invoice from its header
    // and detail rows. Returns null when the order does not exist.
    public async Task<InvoiceDto?> GetInvoice(int orderId)
    {
        var OrderQuery = context.Orders
            .Where(o => o.Id == orderId)
            .Select(o => new
            {
                o.Id, o.OrderDate, o.CustomerId, o.ShipAddress,
                o.ShipCity, o.ShipCountry, o.ShipPostalCode,
                o.Subtotal, o.Tax, o.Total
            });

        var Order = await context.FirstOrDefaultAync(OrderQuery);

        if (Order is null)
            return null;

        var CustomerQuery = context.Customers
            .Where(c => c.Id == Order.CustomerId)
            .Select(c => new { c.Name });

        var Customer = await context.FirstOrDefaultAync(CustomerQuery);

        var DetailQuery =
            from d in context.OrderDetails
            join p in context.Products on d.ProductId equals p.Id
            where d.OrderId == orderId
            orderby p.Id
            select new InvoiceDetailDto(
                d.ProductId, p.Name, d.UnitPrice, d.Quantity);

        var Details = await context.ToListAsync(DetailQuery);

        return new InvoiceDto(
            Order.Id, Order.OrderDate, Order.CustomerId,
            Customer?.Name ?? string.Empty,
            Order.ShipAddress, Order.ShipCity, Order.ShipCountry,
            Order.ShipPostalCode, Order.Subtotal, Order.Tax,
            Order.Total, Details.ToList());
    }

    // Requirement 15 (window 4): search invoices by order number
    // or by customer (id or name) with pagination.
    public async Task<PagedResultDto<InvoiceSummaryDto>> SearchInvoices(
        string? searchTerm, int page, int pageSize)
    {
        (page, pageSize) = NormalizePaging(page, pageSize);

        var Queryable =
            from o in context.Orders
            join c in context.Customers on o.CustomerId equals c.Id
            select new { Order = o, Customer = c };

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string Term = searchTerm.Trim();
            Queryable = Queryable.Where(x =>
                x.Order.Id.ToString().Contains(Term) ||
                x.Customer.Id.Contains(Term) ||
                x.Customer.Name.Contains(Term));
        }

        int TotalCount = await context.CountAsync(Queryable);

        var Items = await context.ToListAsync(
            Queryable
                .OrderByDescending(x => x.Order.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new InvoiceSummaryDto(
                    x.Order.Id, x.Order.OrderDate,
                    x.Customer.Id, x.Customer.Name, x.Order.Total)));

        return new PagedResultDto<InvoiceSummaryDto>(
            Items.ToList(), page, pageSize, TotalCount);
    }

    static (int Page, int PageSize) NormalizePaging(
        int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 5 : Math.Min(pageSize, 50);
        return (page, pageSize);
    }
}
