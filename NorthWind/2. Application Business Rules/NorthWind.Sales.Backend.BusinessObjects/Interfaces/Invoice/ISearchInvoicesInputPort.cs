namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Invoice;

public interface ISearchInvoicesInputPort
{
    Task<PagedResultDto<InvoiceSummaryDto>> Handle(
        string? searchTerm, int page, int pageSize);
}
