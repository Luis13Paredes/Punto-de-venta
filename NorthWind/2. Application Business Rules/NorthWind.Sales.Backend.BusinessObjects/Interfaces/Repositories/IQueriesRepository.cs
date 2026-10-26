namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;

public interface IQueriesRepository
{
    Task<decimal?> GetCustomerCurrentBalance(string customerId);
    Task<IEnumerable<ProductUnitsInStock>>
        GetProductsUnitsInStock(IEnumerable<int> productIds);

    // Requirement 15: smart search with pagination.
    // filterBy: "all" (id or name), "id" or "name".
    Task<PagedResultDto<CustomerResultDto>> SearchCustomers(
        string? searchTerm, int page, int pageSize,
        string? filterBy);

    // Requirement 15 (search) + requirement 8 (only products
    // with stock greater than zero).
    Task<PagedResultDto<ProductResultDto>> SearchProducts(
        string? searchTerm, int page, int pageSize,
        string? filterBy);

    // Requirement 12: rebuild a saved invoice (header + detail).
    Task<InvoiceDto?> GetInvoice(int orderId);

    // Requirement 15 / window 4: search invoices by number or customer.
    Task<PagedResultDto<InvoiceSummaryDto>> SearchInvoices(
        string? searchTerm, int page, int pageSize);
}
