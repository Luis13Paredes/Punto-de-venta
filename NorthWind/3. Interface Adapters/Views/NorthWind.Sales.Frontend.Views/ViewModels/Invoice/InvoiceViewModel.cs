namespace NorthWind.Sales.Frontend.Views.ViewModels.Invoice;

// Requirement 12: rebuild and show a saved invoice (header + detail).
public class InvoiceViewModel(IInvoiceGateway gateway)
{
    public InvoiceDto? Invoice { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public bool IsLoaded => Invoice is not null;

    public async Task LoadAsync(int orderId)
    {
        ErrorMessage = string.Empty;
        try
        {
            Invoice = await gateway.GetInvoiceAsync(orderId);

            if (Invoice is null)
            {
                ErrorMessage = string.Format(
                    InvoiceMessages.InvoiceNotFoundMessage, orderId);
            }
        }
        catch (HttpRequestException)
        {
            Invoice = null;
            ErrorMessage = InvoiceMessages.LoadFailedMessage;
        }
    }

    public async Task<byte[]?> GetPdfAsync()
    {
        if (Invoice is null)
            return null;

        ErrorMessage = string.Empty;
        try
        {
            return await gateway.GetInvoicePdfAsync(Invoice.OrderId);
        }
        catch (HttpRequestException)
        {
            ErrorMessage = InvoiceMessages.PdfErrorMessage;
            return null;
        }
    }
}
