using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using InstantProforms.Application.Features.Proforms.GetProformById;

namespace InstantProforms.Infrastructure.Services.Pdf;

/// <summary>
/// Represents a branded proform PDF document.
/// </summary>
public sealed class ProformPdfDocument : IDocument
{
    private readonly GetProformByIdResponse _data;
    private readonly string _logoPath;

    private const string PastelPink = "#e6c7f0";
    private const string PastelBlue = "#dbe2ff";
    private const string PastelPurple = "#decbf2";
    private const string PastelOverlay = "#eacbf2";
    private const string DarkText = "#1B2D5A";
    private const string BlackText = "#111111";
    private const string DividerColor = "#222222";

    /// <summary>
    /// Initializes a new instance of the <see cref="ProformPdfDocument"/> class.
    /// </summary>
    /// <param name="data">The proform data.</param>
    /// <param name="logoPath">The company logo path.</param>
    public ProformPdfDocument(GetProformByIdResponse data, string logoPath)
    {
        _data = data;
        _logoPath = logoPath;
    }

    /// <inheritdoc />
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    /// <inheritdoc />
    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(0);
            page.DefaultTextStyle(x => x.FontSize(12.5f).FontColor(BlackText));

            page.Content().Layers(layers =>
            {
                layers.Layer().Element(ComposeBackground);
                layers.PrimaryLayer().PaddingHorizontal(44).PaddingVertical(30).Element(ComposeContent);
            });
        });
    }

    private void ComposeBackground(IContainer container)
    {
        var backgroundPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Background.png");

        if (File.Exists(backgroundPath))
        {
            container.Image(backgroundPath).FitArea();
        }
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Element(ComposeHeader);
            column.Item().PaddingTop(34).Element(ComposeMetaSection);
            column.Item().PaddingTop(34).Element(ComposeItemsTable);
            column.Item().PaddingTop(18).Element(ComposeTotalsSection);
            column.Item().PaddingTop(20).Element(ComposeConditionsSection);
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem();

            row.ConstantItem(340).Row(innerRow =>
            {
                innerRow.RelativeItem()
                    .AlignMiddle()
                    .Column(column =>
                    {
                        column.Item().AlignRight().Text("8544-0393").FontSize(12);
                        column.Item().AlignRight().Text("www.ecotechcr.net").FontSize(12);
                        column.Item().AlignRight().Text("San José, Curridabat").FontSize(12);
                    });

                innerRow.ConstantItem(16)
                    .AlignMiddle()
                    .PaddingHorizontal(8)
                    .Height(54)
                    .BorderLeft(1)
                    .BorderColor(DividerColor);

                innerRow.ConstantItem(120)
                    .AlignMiddle()
                    .AlignRight()
                    .Element(ComposeLogo);
            });
        });
    }

    private void ComposeLogo(IContainer container)
    {
        if (File.Exists(_logoPath))
        {
            container.Image(_logoPath).FitArea();
        }
        else
        {
            container.AlignCenter().AlignMiddle().Text("LOGO").Bold();
        }
    }

    private void ComposeMetaSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Text(text =>
            {
                text.Span(_data.IssuedAtUtc.ToString("dd MMMM yyyy").ToUpperInvariant())
                    .FontSize(10)
                    .SemiBold();
            });

            column.Item().PaddingTop(48).Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(11));
                        text.Span("CLIENTE: ").ExtraBold();
                        text.Span(_data.ClientName.ToUpperInvariant());
                    });

                    if (!string.IsNullOrWhiteSpace(_data.ClientPhone))
                    {
                        left.Item().PaddingTop(2).Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(11));
                            text.Span("TELÉFONO: ").ExtraBold();
                            text.Span(_data.ClientPhone!.ToUpperInvariant());
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(_data.ClientEmail))
                    {
                        left.Item().PaddingTop(2).Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(11));
                            text.Span(_data.ClientEmail!.ToUpperInvariant());
                        });
                    }
                });

                row.ConstantItem(260)
                    .AlignBottom()
                    .AlignRight()
                    .Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(11));
                        text.Span("COTIZACIÓN: ").ExtraBold();
                        text.Span(_data.Number).Bold();
                    });
            });
        });
    }

    private void ComposeItemsTable(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor(DividerColor);

            column.Item().PaddingTop(10).PaddingBottom(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(6);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().PaddingLeft(14).Text("DESCRIPCIÓN").FontSize(10).SemiBold();
                    header.Cell().AlignCenter().Text("CANTIDAD").FontSize(10).SemiBold();
                    header.Cell().AlignRight().PaddingRight(14).Text("TOTAL").FontSize(10).SemiBold();
                });
            });

            column.Item().LineHorizontal(1).LineColor(DividerColor);

            column.Item().PaddingTop(18).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(6);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                });

                foreach (var item in _data.Items.OrderBy(x => x.SortOrder))
                {
                    table.Cell().PaddingLeft(14).PaddingBottom(12).Text(item.Description);
                    table.Cell().AlignCenter().PaddingBottom(12).Text(item.Quantity.ToString("0.##"));
                    table.Cell().AlignRight().PaddingRight(14).PaddingBottom(12).Text(FormatCurrency(item.Total));
                }
            });

            column.Item().MinHeight(80);
        });
    }

    private void ComposeTotalsSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor(DividerColor);

            column.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem();

                row.ConstantItem(300).Column(inner =>
                {
                    inner.Item().Row(r =>
                    {
                        r.RelativeItem()
                            .AlignRight()
                            .Text("Sub-Total")
                            .Italic()
                            .ExtraBold()
                            .FontSize(14);

                        r.ConstantItem(110)
                            .AlignRight()
                            .Text(FormatCurrency(_data.Subtotal))
                            .Bold();
                    });

                    inner.Item().PaddingTop(2).LineHorizontal(1).LineColor(DividerColor);

                    inner.Item().PaddingTop(10).Row(r =>
                    {
                        r.RelativeItem()
                            .AlignRight()
                            .Text("Total IVAI")
                            .Italic()
                            .ExtraBold()
                            .FontSize(14);

                        r.ConstantItem(110)
                            .AlignRight()
                            .Text(FormatCurrency(_data.Total))
                            .Bold();
                    });

                    inner.Item().PaddingTop(6).LineHorizontal(1).LineColor(DividerColor);
                });
            });
        });
    }

    private void ComposeConditionsSection(IContainer container)
{
    var conditions = GetFixedConditions();

    container.Row(row =>
    {
        row.ConstantItem(380).Column(column =>
        {
            column.Item().Text("CONDICIONES")
                .FontColor(DarkText)
                .FontSize(16)
                .ExtraBold();

            column.Item().PaddingTop(8).Column(list =>
            {
                foreach (var condition in conditions)
                {
                    list.Item().PaddingBottom(3).Row(itemRow =>
                    {
                        itemRow.ConstantItem(12)
                            .Text("•")
                            .FontColor(DarkText)
                            .FontSize(12);

                        itemRow.RelativeItem()
                            .Text(condition)
                            .FontSize(9.5f)
                            .LineHeight(1.2f);
                    });
                }
            });
        });
    });
}

    private static string FormatCurrency(decimal amount)
    {
        return $"₡{amount:N0}";
    }

    private static IReadOnlyList<string> GetFixedConditions()
    {
        return new List<string>
        {
            "La garantía no cubre daños, fallas o modificaciones ocasionadas por manipulación,\r\nintervención o alteraciones realizadas por terceros ajenos a Ecotech CR.",
            "Cualquier falla, anomalía o inconveniente relacionado con la instalación eléctrica deberá\r\nser reportada directamente a Ecotech CR, quien realizará una valoración técnica previa\r\nantes de cualquier reparación o intervención.",
            "En caso de que el cliente autorice reparaciones o manipulaciones por parte de terceros\r\nsin previa valoración de Ecotech CR, la garantía quedará automáticamente anulada.",
            "Ecotech CR no se responsabiliza por daños ocasionados por sobrecargas, equipos\r\ndefectuosos, conexiones no autorizadas o uso indebido de la instalación."
        };
    }
}