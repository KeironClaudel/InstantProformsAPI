using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using InstantProforms.Application.Features.Proforms.GetProformById;

namespace InstantProforms.Infrastructure.Services.Pdf;

/// <summary>
/// Represents the professional proform PDF document.
/// </summary>
public sealed class ProformPdfDocument : IDocument
{
    private readonly GetProformByIdResponse _data;

    public ProformPdfDocument(GetProformByIdResponse data)
    {
        _data = data;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);

            page.Content().Column(column =>
            {
                column.Item().Element(ComposeHeader);

                column.Item().PaddingTop(20).Element(ComposeClientSection);

                column.Item().PaddingTop(20).Element(ComposeTable);

                column.Item().PaddingTop(20).Element(ComposeTotals);

                column.Item().PaddingTop(30).Element(ComposeConditions);
            });
        });
    }

    // ================= HEADER =================

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("EcoTech")
                    .FontSize(18)
                    .Bold();

                col.Item().Text("All Technology Solutions")
                    .FontSize(10);

                col.Item().Text("San José, Costa Rica")
                    .FontSize(10);

                col.Item().Text("www.ecotechcr.net")
                    .FontSize(10);
            });

            var logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Logo2020.png");
            Console.WriteLine(logoPath);
            row.ConstantItem(120).Height(80).Image(Path.Combine(logoPath));
        });
    }

    // ================= CLIENT =================

    private void ComposeClientSection(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text($"DATE: {_data.IssuedAtUtc:dd MMMM yyyy}");
                col.Item().Text($"CLIENT: {_data.ClientName}");
                col.Item().Text($"EMAIL: {_data.ClientEmail}");
                col.Item().Text($"PHONE: {_data.ClientPhone}");
            });

            row.ConstantItem(200).AlignRight().Text($"PROFORM: {_data.Number}")
                .Bold()
                .FontSize(14);
        });
    }

    // ================= TABLE =================

    private void ComposeTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(5);
                columns.RelativeColumn(2);
                columns.RelativeColumn(2);
            });

            table.Header(header =>
            {
                header.Cell().Text("DESCRIPTION").Bold();
                header.Cell().AlignCenter().Text("QTY").Bold();
                header.Cell().AlignRight().Text("TOTAL").Bold();
            });

            foreach (var item in _data.Items)
            {
                table.Cell().Text(item.Description);
                table.Cell().AlignCenter().Text(item.Quantity.ToString());
                table.Cell().AlignRight().Text(item.Total.ToString("C"));
            }
        });
    }

    // ================= TOTALS =================

    private void ComposeTotals(IContainer container)
    {
        container.AlignRight().Column(col =>
        {
            col.Item().Text($"Subtotal: {_data.Subtotal:C}");
            col.Item().Text($"Total: {_data.Total:C}")
                .Bold()
                .FontSize(14);
        });
    }

    // ================= CONDITIONS =================

    private void ComposeConditions(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text("CONDITIONS")
                .Bold()
                .FontSize(12);

            col.Item().Text("• Warranty does not cover damages caused by improper use.");
            col.Item().Text("• Any issue must be reported before repair.");
            col.Item().Text("• Unauthorized modifications void warranty.");
            col.Item().Text("• Company is not responsible for misuse.");
        });
    }
}