using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NorthWind.Sales.Frontend.Views.ViewModels.Invoice;

namespace NorthWind.Sales.Frontend.Views.Pages;

public partial class Invoice
{
    // Company name shown in the invoice header (UI display only).
    const string InvoicePdfCompany = "NorthWind";

    [Parameter]
    public int OrderId { get; set; }

    [Inject]
    InvoiceViewModel ViewModel { get; set; } = default!;

    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    [Inject]
    IJSRuntime JS { get; set; } = default!;

    bool IsPdfBusy;

    protected override async Task OnParametersSetAsync() =>
        await ViewModel.LoadAsync(OrderId);

    async Task ViewPdfAsync()
    {
        IsPdfBusy = true;
        StateHasChanged();
        var Pdf = await ViewModel.GetPdfAsync();
        if (Pdf is not null)
        {
            await JS.InvokeVoidAsync(
                "northwind.openPdf", $"Invoice-{OrderId}.pdf", Pdf);
        }
        IsPdfBusy = false;
    }

    async Task DownloadPdfAsync()
    {
        IsPdfBusy = true;
        StateHasChanged();
        var Pdf = await ViewModel.GetPdfAsync();
        if (Pdf is not null)
        {
            await JS.InvokeVoidAsync(
                "northwind.downloadPdf", $"Invoice-{OrderId}.pdf", Pdf);
        }
        IsPdfBusy = false;
    }

    void BackToSearch() => Navigation.NavigateTo("/invoices");
}
