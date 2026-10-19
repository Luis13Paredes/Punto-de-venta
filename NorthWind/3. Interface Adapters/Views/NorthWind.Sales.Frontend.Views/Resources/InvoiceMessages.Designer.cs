#nullable enable
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace NorthWind.Sales.Frontend.Views.Resources;

public static class InvoiceMessages
{
    static readonly ResourceManager ResourceManager = new(
        "NorthWind.Sales.Frontend.Views.Resources.InvoiceMessages",
        typeof(InvoiceMessages).GetTypeInfo().Assembly);

    public static CultureInfo? Culture { get; set; }

    public static string InvoicesMenuItemTitle =>
        ResourceManager.GetString(nameof(InvoicesMenuItemTitle), Culture)!;
    public static string SearchInvoicesPageTitle =>
        ResourceManager.GetString(nameof(SearchInvoicesPageTitle), Culture)!;
    public static string SearchInvoicesHeading =>
        ResourceManager.GetString(nameof(SearchInvoicesHeading), Culture)!;
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
    public static string InvoiceNumberColumn =>
        ResourceManager.GetString(nameof(InvoiceNumberColumn), Culture)!;
    public static string DateColumn =>
        ResourceManager.GetString(nameof(DateColumn), Culture)!;
    public static string CustomerColumn =>
        ResourceManager.GetString(nameof(CustomerColumn), Culture)!;
    public static string TotalColumn =>
        ResourceManager.GetString(nameof(TotalColumn), Culture)!;
    public static string ViewButtonText =>
        ResourceManager.GetString(nameof(ViewButtonText), Culture)!;
    public static string InvoicePageTitle =>
        ResourceManager.GetString(nameof(InvoicePageTitle), Culture)!;
    public static string InvoiceNumberLabel =>
        ResourceManager.GetString(nameof(InvoiceNumberLabel), Culture)!;
    public static string OrderDateLabel =>
        ResourceManager.GetString(nameof(OrderDateLabel), Culture)!;
    public static string CustomerLabel =>
        ResourceManager.GetString(nameof(CustomerLabel), Culture)!;
    public static string ShipToLabel =>
        ResourceManager.GetString(nameof(ShipToLabel), Culture)!;
    public static string ProductColumn =>
        ResourceManager.GetString(nameof(ProductColumn), Culture)!;
    public static string QuantityColumn =>
        ResourceManager.GetString(nameof(QuantityColumn), Culture)!;
    public static string UnitPriceColumn =>
        ResourceManager.GetString(nameof(UnitPriceColumn), Culture)!;
    public static string LineTotalColumn =>
        ResourceManager.GetString(nameof(LineTotalColumn), Culture)!;
    public static string ViewPdfButtonText =>
        ResourceManager.GetString(nameof(ViewPdfButtonText), Culture)!;
    public static string DownloadPdfButtonText =>
        ResourceManager.GetString(nameof(DownloadPdfButtonText), Culture)!;
    public static string InvoiceNotFoundMessage =>
        ResourceManager.GetString(nameof(InvoiceNotFoundMessage), Culture)!;
    public static string SearchFailedMessage =>
        ResourceManager.GetString(nameof(SearchFailedMessage), Culture)!;
    public static string PdfErrorMessage =>
        ResourceManager.GetString(nameof(PdfErrorMessage), Culture)!;
    public static string BackToSearchText =>
        ResourceManager.GetString(nameof(BackToSearchText), Culture)!;
    public static string LoadFailedMessage =>
        ResourceManager.GetString(nameof(LoadFailedMessage), Culture)!;
}
