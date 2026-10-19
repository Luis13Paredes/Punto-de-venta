using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NorthWind.Sales.Frontend.Views.ViewModels.Invoice;

namespace NorthWind.Sales.Frontend.Views.Pages;

public partial class Invoices
{
    [Inject]
    SearchInvoicesViewModel ViewModel { get; set; } = default!;

    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    bool IsLoading;

    async Task SearchAsync()
    {
        IsLoading = true;
        StateHasChanged();
        await ViewModel.SearchAsync();
        IsLoading = false;
    }

    async Task NextAsync()
    {
        IsLoading = true;
        StateHasChanged();
        await ViewModel.NextAsync();
        IsLoading = false;
    }

    async Task PreviousAsync()
    {
        IsLoading = true;
        StateHasChanged();
        await ViewModel.PreviousAsync();
        IsLoading = false;
    }

    async Task HandleKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
            await SearchAsync();
    }

    void ViewInvoice(int orderId) =>
        Navigation.NavigateTo($"/invoice/{orderId}");
}
