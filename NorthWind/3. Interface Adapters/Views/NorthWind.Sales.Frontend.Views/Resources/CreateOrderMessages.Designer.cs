#nullable enable
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace NorthWind.Sales.Frontend.Views.Resources;

public static class CreateOrderMessages
{
    static readonly ResourceManager ResourceManager = new(
        "NorthWind.Sales.Frontend.Views.Resources.CreateOrderMessages",
        typeof(CreateOrderMessages).GetTypeInfo().Assembly);

    public static CultureInfo? Culture { get; set; }

    public static string CustomerIdLabel =>
        ResourceManager.GetString(nameof(CustomerIdLabel), Culture)!;
    public static string ShipAddressLabel =>
        ResourceManager.GetString(nameof(ShipAddressLabel), Culture)!;
    public static string ShipCityLabel =>
        ResourceManager.GetString(nameof(ShipCityLabel), Culture)!;
    public static string ShipCountryLabel =>
        ResourceManager.GetString(nameof(ShipCountryLabel), Culture)!;
    public static string ShipPostalCodeLabel =>
        ResourceManager.GetString(nameof(ShipPostalCodeLabel), Culture)!;
    public static string ProductIdLabel =>
        ResourceManager.GetString(nameof(ProductIdLabel), Culture)!;
    public static string UnitPriceLabel =>
        ResourceManager.GetString(nameof(UnitPriceLabel), Culture)!;
    public static string QuantityLabel =>
        ResourceManager.GetString(nameof(QuantityLabel), Culture)!;
    public static string AddProductButtonText =>
        ResourceManager.GetString(nameof(AddProductButtonText), Culture)!;
    public static string SendButtonText =>
        ResourceManager.GetString(nameof(SendButtonText), Culture)!;
    public static string CreatedOrderTemplate =>
        ResourceManager.GetString(nameof(CreatedOrderTemplate), Culture)!;
    public static string TransactionCompletedMessage =>
        ResourceManager.GetString(nameof(TransactionCompletedMessage), Culture)!;
    public static string TransactionFailedMessage =>
        ResourceManager.GetString(nameof(TransactionFailedMessage), Culture)!;
    public static string SubtotalLabel =>
        ResourceManager.GetString(nameof(SubtotalLabel), Culture)!;
    public static string TaxLabel =>
        ResourceManager.GetString(nameof(TaxLabel), Culture)!;
    public static string TotalLabel =>
        ResourceManager.GetString(nameof(TotalLabel), Culture)!;
    public static string CreateOrderMenuItemTitle =>
        ResourceManager.GetString(nameof(CreateOrderMenuItemTitle), Culture)!;
    public static string CreateOrderPageTitle =>
        ResourceManager.GetString(nameof(CreateOrderPageTitle), Culture)!;
    public static string SearchCustomersTitle =>
        ResourceManager.GetString(nameof(SearchCustomersTitle), Culture)!;
    public static string SearchProductsTitle =>
        ResourceManager.GetString(nameof(SearchProductsTitle), Culture)!;
    public static string SearchPlaceholder =>
        ResourceManager.GetString(nameof(SearchPlaceholder), Culture)!;
    public static string SearchButtonText =>
        ResourceManager.GetString(nameof(SearchButtonText), Culture)!;
    public static string NoResultsText =>
        ResourceManager.GetString(nameof(NoResultsText), Culture)!;
    public static string PreviousPageText =>
        ResourceManager.GetString(nameof(PreviousPageText), Culture)!;
    public static string NextPageText =>
        ResourceManager.GetString(nameof(NextPageText), Culture)!;
    public static string PageInfoTemplate =>
        ResourceManager.GetString(nameof(PageInfoTemplate), Culture)!;
    public static string ChangeProductButtonText =>
        ResourceManager.GetString(nameof(ChangeProductButtonText), Culture)!;
    public static string RemoveProductButtonText =>
        ResourceManager.GetString(nameof(RemoveProductButtonText), Culture)!;
    public static string SelectButtonText =>
        ResourceManager.GetString(nameof(SelectButtonText), Culture)!;
    public static string ProductAlreadyInOrderMessage =>
        ResourceManager.GetString(nameof(ProductAlreadyInOrderMessage), Culture)!;
    public static string CustomerNameColumn =>
        ResourceManager.GetString(nameof(CustomerNameColumn), Culture)!;
    public static string CustomerBalanceColumn =>
        ResourceManager.GetString(nameof(CustomerBalanceColumn), Culture)!;
    public static string ProductIdColumn =>
        ResourceManager.GetString(nameof(ProductIdColumn), Culture)!;
    public static string ProductNameColumn =>
        ResourceManager.GetString(nameof(ProductNameColumn), Culture)!;
    public static string PriceColumn =>
        ResourceManager.GetString(nameof(PriceColumn), Culture)!;
    public static string StockColumn =>
        ResourceManager.GetString(nameof(StockColumn), Culture)!;
    public static string SearchCustomerButtonText =>
        ResourceManager.GetString(nameof(SearchCustomerButtonText), Culture)!;
    public static string ActionsColumn =>
        ResourceManager.GetString(nameof(ActionsColumn), Culture)!;
    public static string SearchProductButtonText =>
        ResourceManager.GetString(nameof(SearchProductButtonText), Culture)!;
    public static string CustomerIdColumn =>
        ResourceManager.GetString(nameof(CustomerIdColumn), Culture)!;
    public static string ConfirmRemoveTitle =>
        ResourceManager.GetString(nameof(ConfirmRemoveTitle), Culture)!;
    public static string ConfirmRemoveMessage =>
        ResourceManager.GetString(nameof(ConfirmRemoveMessage), Culture)!;
    public static string ConfirmYesButtonText =>
        ResourceManager.GetString(nameof(ConfirmYesButtonText), Culture)!;
    public static string ConfirmNoButtonText =>
        ResourceManager.GetString(nameof(ConfirmNoButtonText), Culture)!;
    public static string DuplicateProductsMessage =>
        ResourceManager.GetString(nameof(DuplicateProductsMessage), Culture)!;
    public static string ShippingSectionTitle =>
        ResourceManager.GetString(nameof(ShippingSectionTitle), Culture)!;
    public static string OrderDetailSectionTitle =>
        ResourceManager.GetString(nameof(OrderDetailSectionTitle), Culture)!;
    public static string OrderEmptyHintText =>
        ResourceManager.GetString(nameof(OrderEmptyHintText), Culture)!;
}
