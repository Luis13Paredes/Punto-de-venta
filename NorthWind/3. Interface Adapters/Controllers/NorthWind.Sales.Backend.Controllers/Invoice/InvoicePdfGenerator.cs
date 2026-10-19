using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using NorthWind.Sales.Backend.Controllers.Resources;

namespace NorthWind.Sales.Backend.Controllers.Invoice;

// Requirement 12: rebuild a saved order (header + detail)
// and render it as a PDF document for download and printing.
internal static class InvoicePdfGenerator
{
    const string HeaderBackground = "#DEE2E6";
    const string ZebraBackground = "#F5F7FA";

    static InvoicePdfGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static byte[] Generate(InvoiceDto invoice) =>
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(text => text.FontSize(10));

                page.Header().Column(header =>
                {
                    header.Item()
                        .Text(InvoicePdfMessages.CompanyName)
                        .FontSize(20).Bold();
                    header.Item()
                        .Text(InvoicePdfMessages.DocumentTitle)
                        .FontSize(16).SemiBold();
                    header.Item().PaddingTop(8).Row(row =>
                    {
                        row.RelativeItem().Text(
                            $"{InvoicePdfMessages.InvoiceNumberLabel} {invoice.OrderId}");
                        row.RelativeItem().AlignRight().Text(
                            $"{InvoicePdfMessages.DateLabel}: {invoice.OrderDate:yyyy-MM-dd HH:mm}");
                    });
                });

                page.Content().PaddingVertical(18).Column(content =>
                {
                    content.Item().Column(customer =>
                    {
                        customer.Item()
                            .Text(InvoicePdfMessages.CustomerLabel).SemiBold();
                        customer.Item().Text(
                            $"{invoice.CustomerId} - {invoice.CustomerName}");
                        customer.Item().PaddingTop(6)
                            .Text(InvoicePdfMessages.ShipToLabel).SemiBold();
                        customer.Item().Text(invoice.ShipAddress);
                        customer.Item().Text(
                            $"{invoice.ShipCity}, {invoice.ShipCountry}");
                        customer.Item().Text(
                            $"{InvoicePdfMessages.PostalCodeLabel}: {invoice.ShipPostalCode}");
                    });

                    content.Item().PaddingTop(16).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(5);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            HeaderCell(header.Cell(),
                                InvoicePdfMessages.ProductColumn, false);
                            HeaderCell(header.Cell(),
                                InvoicePdfMessages.QuantityColumn, true);
                            HeaderCell(header.Cell(),
                                InvoicePdfMessages.UnitPriceColumn, true);
                            HeaderCell(header.Cell(),
                                InvoicePdfMessages.LineTotalColumn, true);
                        });

                        bool Alternate = false;
                        foreach (var Detail in invoice.Details)
                        {
                            bool UseZebra = Alternate;
                            Alternate = !Alternate;

                            IContainer RowCell() => UseZebra
                                ? table.Cell().Background(ZebraBackground)
                                : table.Cell();

                            ContentCell(RowCell(),
                                Detail.ProductName, false);
                            ContentCell(RowCell(),
                                Detail.Quantity.ToString(), true);
                            ContentCell(RowCell(),
                                Detail.UnitPrice.ToString("0.00"), true);
                            ContentCell(RowCell(),
                                Detail.LineTotal.ToString("0.00"), true);
                        }
                    });

                    content.Item().AlignRight().PaddingTop(14)
                        .Width(240).Column(totals =>
                    {
                        TotalRow(totals.Item(),
                            InvoicePdfMessages.SubtotalLabel,
                            invoice.Subtotal, false);
                        TotalRow(totals.Item(),
                            InvoicePdfMessages.TaxLabel,
                            invoice.Tax, false);
                        TotalRow(totals.Item().BorderTop(1).PaddingTop(6),
                            InvoicePdfMessages.TotalLabel,
                            invoice.Total, true);
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf();

    static void HeaderCell(IContainer cell, string label, bool rightAlign)
    {
        IContainer Styled = cell.Background(HeaderBackground).Padding(6);
        (rightAlign ? Styled.AlignRight() : Styled.AlignLeft())
            .Text(label).Bold();
    }

    static void ContentCell(IContainer cell, string value, bool rightAlign)
    {
        IContainer Styled = cell.Padding(6);
        (rightAlign ? Styled.AlignRight() : Styled.AlignLeft())
            .Text(value);
    }

    static void TotalRow(IContainer container, string label,
        decimal value, bool bold)
    {
        container.PaddingTop(2).Row(row =>
        {
            var LabelSpan = row.RelativeItem().Text(label);
            var ValueSpan = row.ConstantItem(90).AlignRight()
                .Text(value.ToString("0.00"));

            if (bold)
            {
                LabelSpan.Bold();
                ValueSpan.Bold();
            }
        });
    }
}
