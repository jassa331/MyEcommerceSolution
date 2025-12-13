using Admin.API.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

public class InvoiceDocument : IDocument
{
    private readonly manageorders _order;
    private readonly List<OrderItem> _items;
    private readonly OrderAddress _address;

    public InvoiceDocument(manageorders order,
                           List<OrderItem> items,
                           OrderAddress address)
    {
        _order = order;
        _items = items;
        _address = address;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(30);
            page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(11));

            // ===== Header with Branding =====
            page.Header().BorderBottom(2).PaddingBottom(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("𝓓𝓱𝓪𝓶𝓶𝓲's 𝓔-𝓼𝓱𝓸𝓹𝓮").FontSize(20).Bold();
                    col.Item().Text("www.dhammieshop.com").FontSize(10).FontColor(Colors.Grey.Darken1);
                });

                row.ConstantItem(200).AlignRight().Column(col =>
                {
                    col.Item().Text("INVOICE").FontSize(18).Bold();
                    col.Item().Text($"Order No: {_order.OrderNumber}");
                    col.Item().Text($"Date: {_order.CreatedAt:dd MMM yyyy}");
                });
            });

            // ===== Content =====
            page.Content().PaddingTop(10).Column(col =>
            {
                // Address Box
                col.Item().Background(Colors.Grey.Lighten4).Padding(10).Column(x =>
                {
                    x.Item().Text("BILL TO").Bold();
                    x.Item().Text(_address?.FullName);
                    x.Item().Text(_address?.Line1);
                    if (!string.IsNullOrEmpty(_address?.Line2))
                        x.Item().Text(_address.Line2);

                    x.Item().Text($"{_address?.City}, {_address?.State} - {_address?.PostalCode}");
                    x.Item().Text(_address?.Country ?? "India");
                    x.Item().Text($"Phone: {_address?.Phone}");
                });

                // Product Table
                col.Item().PaddingTop(15).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(4);
                        c.RelativeColumn(1);
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                    });

                    table.Header(h =>
                    {
                        h.Cell().Background(Colors.Blue.Medium).Padding(6).Text("Product").Bold().FontColor(Colors.White);
                        h.Cell().Background(Colors.Blue.Medium).Padding(6).Text("Qty").Bold().FontColor(Colors.White);
                        h.Cell().Background(Colors.Blue.Medium).Padding(6).Text("Price").Bold().FontColor(Colors.White);
                        h.Cell().Background(Colors.Blue.Medium).Padding(6).Text("Total").Bold().FontColor(Colors.White);
                    });

                    foreach (var item in _items)
                    {
                        decimal price = item.UnitPrice ?? 0;
                        int qty = item.Quantity ?? 0;
                        decimal rowTotal = price * qty;

                        table.Cell().Padding(6).Text(item.ProductName);
                        table.Cell().Padding(6).Text(qty.ToString());
                        table.Cell().Padding(6).Text($"₹{price}");
                        table.Cell().Padding(6).Text($"₹{rowTotal}");
                    }
                });

                // Totals Card
                col.Item().PaddingTop(15).AlignRight().Background(Colors.Grey.Lighten4).Padding(10).Column(t =>
                {
                    t.Item().Text($"Sub Total: ₹{_order.SubTotalAmount}");
                    t.Item().Text($"Shipping: ₹{_order.ShippingAmount}");
                    t.Item().Text($"Tax: ₹{_order.TaxAmount}");
                    t.Item().Text($"Total: ₹{_order.TotalAmount}")
                        .Bold().FontSize(14);
                });

                // Footer
                col.Item().PaddingTop(20).AlignCenter()
                    .Text("Thank you for shopping with Dhammi's E-shop! ❤️")
                    .FontSize(12).Bold();
            });
        });
    }
}
