namespace NorthWind.Sales.Entities.ValueObjects;

public class Endpoints
{
    public const string CreateOrder = $"/{nameof(CreateOrder)}";
    public const string SearchCustomers = $"/{nameof(SearchCustomers)}";
    public const string SearchProducts = $"/{nameof(SearchProducts)}";
    public const string GetInvoice = $"/{nameof(GetInvoice)}";
    public const string SearchInvoices = $"/{nameof(SearchInvoices)}";
    public const string GetInvoicePdf = $"/{nameof(GetInvoicePdf)}";
}
