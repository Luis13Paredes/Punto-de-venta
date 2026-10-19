using Microsoft.AspNetCore.Components;

namespace NorthWind.Sales.Frontend.Views.Components;

public partial class ConfirmModal
{
    [Parameter]
    public string Title { get; set; } = string.Empty;
    [Parameter]
    public string Message { get; set; } = string.Empty;
    [Parameter]
    public string ConfirmButtonText { get; set; } = string.Empty;
    [Parameter]
    public string CancelButtonText { get; set; } = string.Empty;
    [Parameter]
    public EventCallback OnConfirm { get; set; }

    public bool IsOpen { get; private set; }

    // StateHasChanged is required: Open() runs in the parent's context and
    // this component has no RenderFragments, so the parent's diff may skip
    // re-rendering it (all parameters look unchanged).
    public void Open()
    {
        IsOpen = true;
        StateHasChanged();
    }

    public void Close() => IsOpen = false;

    async Task ConfirmAsync()
    {
        await OnConfirm.InvokeAsync();
        Close();
    }
}
