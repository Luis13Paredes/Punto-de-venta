#nullable enable
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace NorthWind.Sales.Backend.Controllers.Resources;

public static class InvoicePdfMessages
{
    static readonly ResourceManager ResourceManager = new(
        "NorthWind.Sales.Backend.Controllers.Resources.InvoicePdfMessages",
        typeof(InvoicePdfMessages).GetTypeInfo().Assembly);

    public static CultureInfo? Culture { get; set; }

    public static string CompanyName =>
        ResourceManager.GetString(nameof(CompanyName), Culture)!;
    public static string DocumentTitle =>
        ResourceManager.GetString(nameof(DocumentTitle), Culture)!;
    public static string InvoiceNumberLabel =>
        ResourceManager.GetString(nameof(InvoiceNumberLabel), Culture)!;
    public static string DateLabel =>
        ResourceManager.GetString(nameof(DateLabel), Culture)!;
    public static string CustomerLabel =>
        ResourceManager.GetString(nameof(CustomerLabel), Culture)!;
    public static string ShipToLabel =>
        ResourceManager.GetString(nameof(ShipToLabel), Culture)!;
    public static string AddressLabel =>
        ResourceManager.GetString(nameof(AddressLabel), Culture)!;
    public static string CityLabel =>
        ResourceManager.GetString(nameof(CityLabel), Culture)!;
    public static string CountryLabel =>
        ResourceManager.GetString(nameof(CountryLabel), Culture)!;
    public static string PostalCodeLabel =>
        ResourceManager.GetString(nameof(PostalCodeLabel), Culture)!;
    public static string ProductColumn =>
        ResourceManager.GetString(nameof(ProductColumn), Culture)!;
    public static string QuantityColumn =>
        ResourceManager.GetString(nameof(QuantityColumn), Culture)!;
    public static string UnitPriceColumn =>
        ResourceManager.GetString(nameof(UnitPriceColumn), Culture)!;
    public static string LineTotalColumn =>
        ResourceManager.GetString(nameof(LineTotalColumn), Culture)!;
    public static string SubtotalLabel =>
        ResourceManager.GetString(nameof(SubtotalLabel), Culture)!;
    public static string TaxLabel =>
        ResourceManager.GetString(nameof(TaxLabel), Culture)!;
    public static string TotalLabel =>
        ResourceManager.GetString(nameof(TotalLabel), Culture)!;
    public static string PageInfoTemplate =>
        ResourceManager.GetString(nameof(PageInfoTemplate), Culture)!;
}
