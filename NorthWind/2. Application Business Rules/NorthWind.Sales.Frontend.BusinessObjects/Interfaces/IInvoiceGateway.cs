namespace NorthWind.Sales.Frontend.BusinessObjects.Interfaces;

public interface IInvoiceGateway
{
    Task<InvoiceDto?> GetInvoiceAsync(int orderId);
    Task<PagedResultDto<InvoiceSummaryDto>> SearchInvoicesAsync(
        string? searchTerm, int page, int pageSize);
    Task<byte[]> GetInvoicePdfAsync(int orderId);
}
