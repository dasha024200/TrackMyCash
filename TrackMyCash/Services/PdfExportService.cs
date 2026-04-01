using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TrackMyCash.Models;

namespace TrackMyCash.Services
{
    public class PdfExportService
    {
        public byte[] ExportTransactionsReceiptPdf(List<Transaction> transactions, string userName)
        {
            decimal totalIncome = transactions
                .Where(t => t.Type == "Income")
                .Sum(t => t.Amount);

            decimal totalExpense = transactions
                .Where(t => t.Type == "Expense")
                .Sum(t => t.Amount);

            decimal balance = totalIncome - totalExpense;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

                    page.Header().Element(Header);

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        column.Spacing(8);

                        column.Item().Text($"Користувач: {userName}").FontSize(12).SemiBold();
                        column.Item().Text($"Дата: {DateTime.UtcNow:dd.MM.yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Darken1);

                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Дата").SemiBold();
                            row.RelativeItem().Text("Категорія").SemiBold();
                            row.ConstantItem(70).AlignRight().Text("Дохід").SemiBold();
                            row.ConstantItem(70).AlignRight().Text("Витрата").SemiBold();
                        });

                        foreach (var transaction in transactions.OrderByDescending(t => t.DateCreated))
                        {
                            column.Item().Row(r =>
                            {
                                r.RelativeItem().Text(transaction.DateCreated.ToString("dd.MM.yyyy"));
                                r.RelativeItem().Text(transaction.Category?.Name ?? "Без категорії");
                                r.ConstantItem(70).AlignRight().Text(transaction.Type == "Income" ? transaction.Amount.ToString("0.00") : "").FontColor(Colors.Green.Darken1);
                                r.ConstantItem(70).AlignRight().Text(transaction.Type == "Expense" ? transaction.Amount.ToString("0.00") : "").FontColor(Colors.Red.Darken1);
                            });
                            if (!string.IsNullOrWhiteSpace(transaction.Comment))
                            {
                                column.Item().Text($"Коментар: {transaction.Comment}").FontSize(9).FontColor(Colors.Grey.Darken1);
                            }
                        }

                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        column.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Загальний дохід:").SemiBold();
                            r.ConstantItem(120).AlignRight().Text(totalIncome.ToString("0.00") + " грн").FontColor(Colors.Green.Darken1);
                        });

                        column.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Загальні витрати:").SemiBold();
                            r.ConstantItem(120).AlignRight().Text(totalExpense.ToString("0.00") + " грн").FontColor(Colors.Red.Darken1);
                        });

                        column.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Баланс:").SemiBold();
                            r.ConstantItem(120).AlignRight().Text(balance.ToString("0.00") + " грн").FontColor(balance >= 0 ? Colors.Green.Darken2 : Colors.Red.Darken2);
                        });
                    });

                    page.Footer().AlignCenter().Text("TrackMyCash — ваш стильний фінансовий чек").FontSize(9).FontColor(Colors.Grey.Darken1);
                });
            });

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            return ms.ToArray();
        }

        private static void Header(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(inner =>
                    {
                        inner.Item().Text("TrackMyCash").FontSize(24).Bold().FontColor(Colors.Pink.Medium);
                        inner.Item().Text("Фінансовий звіт (чек)").FontSize(10).FontColor(Colors.Grey.Darken1);
                    });
                    row.ConstantItem(50).AlignRight().Text("🧾").FontSize(28);
                });

                column.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            });
        }
    }
}

