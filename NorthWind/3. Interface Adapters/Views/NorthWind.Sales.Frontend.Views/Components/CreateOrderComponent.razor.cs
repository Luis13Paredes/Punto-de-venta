using Microsoft.AspNetCore.Components;
using NorthWind.Membership.Frontend.RazorViews.Components;
using NorthWind.Sales.Entities.Dtos.Search;
using NorthWind.Sales.Frontend.Views.ViewModels.CreateOrder;

namespace NorthWind.Sales.Frontend.Views.Components;

public partial class CreateOrderComponent
{
    [Parameter]
    public CreateOrderViewModel Order { get; set; } = default!;

    SearchModal<CustomerResultDto>? CustomerSearchModal;
    SearchModal<ProductResultDto>? ProductSearchModal;
    ConfirmModal? ConfirmRemoveModal;

    // Product row waiting for the user's Sí/No confirmation (Requirement 10).
    CreateOrderDetailViewModel? PendingRemoveDetail;

    // Index of the detail row being replaced (Requirement 11);
    // -1 means the modal will add a new product (Requirement 8).
    int ProductChangeIndex = -1;

    // Discards stale lookups when the id is typed quickly.
    int ProductLookupVersion;

    async Task OpenCustomerSearch()
    {
        if (CustomerSearchModal is not null)
            await CustomerSearchModal.Open();
    }

    async Task OpenProductSearch()
    {
        ProductChangeIndex = -1;
        if (ProductSearchModal is not null)
            await ProductSearchModal.Open();
    }

    async Task ChangeProduct(int index)
    {
        ProductChangeIndex = index;
        if (ProductSearchModal is not null)
            await ProductSearchModal.Open();
    }

    void RemoveProduct(CreateOrderDetailViewModel detail)
    {
        PendingRemoveDetail = detail;
        ConfirmRemoveModal?.Open();
    }

    void ConfirmRemove()
    {
        if (PendingRemoveDetail is null)
            return;

        Order.RemoveOrderDetail(PendingRemoveDetail);
        PendingRemoveDetail = null;
    }

    void OnProductSelected(ProductResultDto product)
    {
        if (ProductChangeIndex >= 0 &&
            ProductChangeIndex < Order.OrderDetails.Count)
        {
            Order.ReplaceOrderDetailProduct(
                Order.OrderDetails[ProductChangeIndex], product);
        }
        else
        {
            Order.AddProductFromSearch(product);
        }
        ProductChangeIndex = -1;
    }

    // Fills the unit price automatically when the product id is typed by
    // hand (the modal is not the only way to get a price).
    async Task OnProductIdInput(ChangeEventArgs e, CreateOrderDetailViewModel detail)
    {
        if (!int.TryParse(e.Value?.ToString(), out int Parsed) ||
            Parsed <= 0 || Parsed == detail.ProductId)
            return;

        detail.ProductId = Parsed;
        await OnProductIdChanged(detail);
    }

    async Task OnProductIdChanged(CreateOrderDetailViewModel detail)
    {
        if (detail.ProductId <= 0)
        {
            detail.UnitPrice = 0;
            detail.UnitsInStock = 0;
            return;
        }

        int Version = ++ProductLookupVersion;
        try
        {
            // The backend search matches by id or name; exact id wins below.
            var Result = await Order.SearchProducts(
                detail.ProductId.ToString(), "id", 1, 5);

            if (Version != ProductLookupVersion)
                return;

            var Match = Result.Items
                .FirstOrDefault(p => p.Id == detail.ProductId);
            detail.UnitPrice = Match?.UnitPrice ?? 0;
            detail.UnitsInStock = Match?.UnitsInStock ?? 0;
        }
        catch (HttpRequestException)
        {
            // API unavailable: keep the current price.
        }
    }

    void SanitizeShipAddress() =>
        Order.ShipAddress = InputFilters.FilterAddress(Order.ShipAddress);

    void SanitizeShipCity() =>
        Order.ShipCity = InputFilters.FilterName(Order.ShipCity);

    void SanitizeShipCountry() =>
        Order.ShipCountry = InputFilters.FilterName(Order.ShipCountry);

    void SanitizeShipPostalCode() =>
        Order.ShipPostalCode =
            InputFilters.FilterDigits(Order.ShipPostalCode);
}
