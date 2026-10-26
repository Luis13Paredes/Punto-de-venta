using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NorthWind.Sales.Entities.Dtos.Search;

namespace NorthWind.Sales.Frontend.Views.Components;

public partial class SearchModal<TItem>
{
    // Requirement 15: smart search modal with pagination.
    const int PageSize = 5;

    [Parameter]
    public string Title { get; set; } = string.Empty;
    [Parameter]
    public string SearchPlaceholder { get; set; } = string.Empty;
    [Parameter]
    public string SearchButtonText { get; set; } = string.Empty;
    [Parameter]
    public string NoResultsText { get; set; } = string.Empty;
    [Parameter]
    public string PreviousPageText { get; set; } = string.Empty;
    [Parameter]
    public string NextPageText { get; set; } = string.Empty;
    [Parameter]
    public string PageInfoTemplate { get; set; } = "{0}/{1}";
    [Parameter]
    public string SelectButtonText { get; set; } = string.Empty;
    [Parameter]
    public bool ShowFilter { get; set; }
    [Parameter]
    public string FilterAllText { get; set; } = "Todos";
    [Parameter]
    public string FilterIdText { get; set; } = "ID";
    [Parameter]
    public string FilterNameText { get; set; } = "Nombre";
    [Parameter]
    public Func<string?, string?, int, int, Task<PagedResultDto<TItem>>> OnSearch { get; set; }
        = default!;
    [Parameter]
    public EventCallback<TItem> OnSelect { get; set; }
    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }
    [Parameter]
    public RenderFragment<TItem>? RowTemplate { get; set; }
    // Optional rule to grey out and block items that cannot be
    // selected (e.g. a product that is already in the order).
    [Parameter]
    public Func<TItem, bool>? ItemBlocked { get; set; }
    [Parameter]
    public string BlockedItemTitle { get; set; } = "Ya agregado";
    [Parameter]
    public string BlockedItemMark { get; set; } = "—";

    public bool IsOpen { get; private set; }

    string SearchTerm = string.Empty;
    string FilterBy = "all";
    int Page = 1;
    bool IsLoading;
    PagedResultDto<TItem>? Result;

    // Discards superseded searches (fast typing / pagination races).
    int SearchVersion;

    bool CanGoPrevious =>
        !IsLoading && Result is not null && Result.Page > 1;
    bool CanGoNext =>
        !IsLoading && Result is not null && Result.Page < Result.TotalPages;

    public async Task Open()
    {
        IsOpen = true;
        SearchTerm = string.Empty;
        FilterBy = "all";
        Page = 1;
        StateHasChanged();
        await SearchAsync();
    }

    public void Close() => IsOpen = false;

    async Task OnFilterChanged()
    {
        Page = 1;
        await SearchAsync();
    }

    async Task SearchAsync()
    {
        int Version = ++SearchVersion;
        IsLoading = true;
        StateHasChanged();
        try
        {
            var NewResult = await OnSearch(
                string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm,
                FilterBy, Page, PageSize);
            if (Version != SearchVersion) return;
            Result = NewResult;
        }
        catch (HttpRequestException)
        {
            if (Version == SearchVersion)
                Result = null;
        }
        finally
        {
            if (Version == SearchVersion)
            {
                IsLoading = false;
                StateHasChanged();
            }
        }
    }

    async Task NextAsync()
    {
        if (!CanGoNext) return;
        Page++;
        await SearchAsync();
    }

    async Task PreviousAsync()
    {
        if (!CanGoPrevious) return;
        Page--;
        await SearchAsync();
    }

    // Live search: filters as the user types (debounced), without
    // needing the search button. Button and Enter still work.
    async Task HandleSearchInput(ChangeEventArgs e)
    {
        SearchTerm = e.Value?.ToString() ?? string.Empty;
        Page = 1;
        int Version = ++SearchVersion;
        await Task.Delay(300);
        if (Version != SearchVersion || !IsOpen) return;
        await SearchAsync();
    }

    async Task HandleKeyUp(KeyboardEventArgs e)
    {
        if (e.Key != "Enter") return;
        Page = 1;
        await SearchAsync();
    }

    public bool IsBlocked(TItem item) =>
        ItemBlocked is not null && ItemBlocked(item);

    async Task SelectAsync(TItem item)
    {
        if (IsBlocked(item)) return;
        await OnSelect.InvokeAsync(item);
        Close();
    }
}
