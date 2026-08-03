using FreelanceFlow.Backend.Models.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FreelanceFlow.Backend.ExternalServices;

public class InvoicePdfService : IInvoicePdfService
{
    public byte[] GeneratePdf(Invoice invoice)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("INVOICE").FontSize(24).Bold();
                        col.Item().Text(invoice.InvoiceNumber).FontSize(12).FontColor(Colors.Grey.Darken1);
                    });

                    row.ConstantItem(180).Column(col =>
                    {
                        col.Item().AlignRight().Text($"Issue date: {invoice.IssueDate:yyyy-MM-dd}");
                        col.Item().AlignRight().Text($"Due date: {invoice.DueDate:yyyy-MM-dd}");
                        col.Item().AlignRight().Text($"Status: {invoice.Status}").Bold();
                    });
                });

                page.Content().PaddingVertical(20).Column(column =>
                {
                    column.Spacing(15);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("From").Bold().FontColor(Colors.Grey.Darken2);
                            col.Item().Text(invoice.Freelancer.FullName);
                            col.Item().Text(invoice.Freelancer.Email ?? string.Empty);
                        });

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Bill To").Bold().FontColor(Colors.Grey.Darken2);
                            col.Item().Text(invoice.Client.Name);
                            if (!string.IsNullOrEmpty(invoice.Client.Company))
                            {
                                col.Item().Text(invoice.Client.Company!);
                            }
                            col.Item().Text(invoice.Client.Email);
                        });
                    });

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Description").Bold();
                            header.Cell().AlignRight().Text("Qty").Bold();
                            header.Cell().AlignRight().Text("Unit Price").Bold();
                            header.Cell().AlignRight().Text("Total").Bold();
                            header.Cell().ColumnSpan(4).PaddingTop(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1);
                        });

                        foreach (var item in invoice.LineItems)
                        {
                            table.Cell().PaddingVertical(4).Text(item.Description);
                            table.Cell().PaddingVertical(4).AlignRight().Text(item.Quantity.ToString("0.##"));
                            table.Cell().PaddingVertical(4).AlignRight().Text(item.UnitPrice.ToString("0.00"));
                            table.Cell().PaddingVertical(4).AlignRight().Text(item.Total.ToString("0.00"));
                        }
                    });

                    column.Item().AlignRight().Column(col =>
                    {
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().AlignRight().Text("Subtotal:");
                            r.ConstantItem(100).AlignRight().Text($"{invoice.SubTotal:0.00} {invoice.Freelancer.Currency}");
                        });
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().AlignRight().Text($"Tax ({invoice.TaxPercent:0.##}%):");
                            r.ConstantItem(100).AlignRight().Text($"{invoice.TaxAmount:0.00} {invoice.Freelancer.Currency}");
                        });
                        col.Item().PaddingTop(4).BorderTop(1).BorderColor(Colors.Grey.Darken1).Row(r =>
                        {
                            r.RelativeItem().AlignRight().Text("Total:").Bold();
                            r.ConstantItem(100).AlignRight().Text($"{invoice.TotalAmount:0.00} {invoice.Freelancer.Currency}").Bold();
                        });
                    });

                    if (!string.IsNullOrEmpty(invoice.Notes))
                    {
                        column.Item().PaddingTop(10).Column(col =>
                        {
                            col.Item().Text("Notes").Bold().FontColor(Colors.Grey.Darken2);
                            col.Item().Text(invoice.Notes!);
                        });
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generated by FreelanceFlow on ").FontColor(Colors.Grey.Darken1);
                    text.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm 'UTC'")).FontColor(Colors.Grey.Darken1);
                });
            });
        });

        return document.GeneratePdf();
    }
}