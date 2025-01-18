using Clothers.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace Clothers.Services
{
    public class OrderPdfGenerator
    {
        public byte[] GeneratePdf(Order order)
        {
            var content = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text($"Potwierdzenie Zamówienia #{order.Id}")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(10)
                        .Column(column =>
                        {
                            column.Spacing(10);
                            column.Item().Text($"Data Zamówienia: {order.OrderDate:dd-MM-yyyy HH:mm}");
                            column.Item().Text($"Klient: {order.FirstName} {order.LastName}");
                            column.Item().Text($"Adres Dostawy: {order.DeliveryAddress}");
                            column.Item().Text($"Sposób Płatności: {order.PaymentMethod}");
                            column.Item().Text($"Sposób Dostawy: {order.DeliveryMethod}");
                            column.Item().Text("Produkty:");

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Nazwa Produktu");
                                    header.Cell().Element(CellStyle).Text("Ilość");
                                    header.Cell().Element(CellStyle).Text("Cena Jednostkowa");
                                    header.Cell().Element(CellStyle).Text("Razem");
                                });

                                foreach (var item in order.OrderItems)
                                {
                                    table.Cell().Element(CellStyle).Text(item.Product.Name);
                                    table.Cell().Element(CellStyle).Text(item.Quantity.ToString());
                                    table.Cell().Element(CellStyle).Text($"{item.UnitPrice:C}");
                                    table.Cell().Element(CellStyle).Text($"{item.TotalPrice:C}");
                                }

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container
                                        .BorderBottom(1)
                                        .PaddingVertical(5)
                                        .PaddingHorizontal(5);
                                }
                            });

                            column.Item().AlignRight().Text($"Suma Totalna: {order.OrderItems.Sum(i => i.TotalPrice):C}");
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Dziękujemy za zakupy w Clothers!");
                        });
                });
            });

            return content.GeneratePdf();
        }
    }
}
