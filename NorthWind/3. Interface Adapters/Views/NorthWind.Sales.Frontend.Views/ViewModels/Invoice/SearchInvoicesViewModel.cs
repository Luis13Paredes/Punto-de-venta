namespace NorthWind.Sales.Frontend.Views.ViewModels.Invoice;

// Requirement 15 (window 4): smart search of saved invoices
// by order number or customer, with pagination.
public class SearchInvoicesViewModel(IInvoiceGateway gateway)
{
    const int PageSize = 5;

    public string SearchTerm { get; set; } = string.Empty;
    public int Page { get; private set; } = 1;
    public PagedResultDto<InvoiceSummaryDto>? Result { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    public bool CanGoPrevious => Result is not null && Result.Page > 1;
    public bool CanGoNext =>
        Result is not null && Result.Page < Result.TotalPages;

    public async Task SearchAsync()
    {
        Page = 1;
        await LoadAsync();
    }

    public async Task NextAsync()
    {
        if (!CanGoNext) return;
        Page++;
        await LoadAsync();
    }

    public async Task PreviousAsync()
    {
        if (!CanGoPrevious) return;
        Page--;
        await LoadAsync();
    }

    async Task LoadAsync()
    {
        ErrorMessage = string.Empty;
        try
        {
            Result = await gateway.SearchInvoicesAsync(
                string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm.Trim(),
                Page, PageSize);
        }
        catch (HttpRequestException)
        {
            Result = null;
            ErrorMessage = InvoiceMessages.SearchFailedMessage;
        }
    }
}
