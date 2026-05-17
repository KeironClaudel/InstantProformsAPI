using InstantProforms.Application.Features.Proforms.Common;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace InstantProforms.Infrastructure.Services.Pdf;

/// <summary>
/// Represents a branded proform PDF document.
/// </summary>
public sealed class ProformPdfDocument : IDocument
{
    private readonly byte[]? _logoBytes;
    private readonly ProformPdfModel _data;

    private string PrimaryColor => string.IsNullOrWhiteSpace(_data.PrimaryColor) ? "#1B2D5A" : _data.PrimaryColor;
    private string SecondaryColor => string.IsNullOrWhiteSpace(_data.SecondaryColor) ? "#E9EEF9" : _data.SecondaryColor;
    private string AccentColor => string.IsNullOrWhiteSpace(_data.AccentColor) ? "#DCE6FF" : _data.AccentColor;

    private const string BlackText = "#111111";
    private const string DividerColor = "#222222";
    private const string MutedText = "#52607A";
    private const string LightBorder = "#CAD5E6";

    /// <summary>
    /// Initializes a new instance of the <see cref="ProformPdfDocument"/> class.
    /// </summary>
    /// <param name="data">The proform data.</param>
    /// <param name="logoBytes">The company logo content.</param>
    public ProformPdfDocument(ProformPdfModel data, byte[]? logoBytes)
    {
        _data = data;
        _logoBytes = logoBytes;
    }

    /// <inheritdoc />
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    /// <inheritdoc />
    public void Compose(IDocumentContainer container)
    {
        var editorialSections = GetEditorialSections();

        if (editorialSections.Count > 0)
        {
            container.Page(page =>
            {
                ConfigurePage(page);
                page.Content().Layers(layers =>
                {
                    layers.Layer().Element(ComposeBackground);
                    layers.PrimaryLayer()
                        .PaddingHorizontal(44)
                        .PaddingVertical(30)
                        .Element(content => ComposeLeadingEditorialContent(content, editorialSections[0]));
                });
            });

            foreach (var section in editorialSections.Skip(1))
            {
                container.Page(page =>
                {
                    ConfigurePage(page);
                    page.Content().Layers(layers =>
                    {
                        layers.Layer().Element(ComposeBackground);
                        layers.PrimaryLayer()
                            .PaddingLeft(68)
                            .PaddingRight(54)
                            .PaddingTop(118)
                            .PaddingBottom(48)
                            .Element(content => ComposeEditorialSection(content, section));
                    });
                });
            }
        }

        container.Page(page =>
        {
            ConfigurePage(page);
            page.Content().Layers(layers =>
            {
                layers.Layer().Element(ComposeBackground);
                layers.PrimaryLayer()
                    .PaddingHorizontal(44)
                    .PaddingVertical(30)
                    .Element(ComposeProformContent);
            });
        });
    }

    private static void ConfigurePage(PageDescriptor page)
    {
        page.Size(PageSizes.A4);
        page.Margin(0);
        page.DefaultTextStyle(x => x.FontSize(12.5f).FontColor(BlackText));
    }

    private void ComposeBackground(IContainer container)
    {
        var backgroundPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Background.png");

        if (File.Exists(backgroundPath))
        {
            container.Image(backgroundPath).FitArea();
        }
    }

    private void ComposeLeadingEditorialContent(IContainer container, EditorialSection firstSection)
    {
        container.Column(column =>
        {
            column.Item().Element(ComposeHeader);
            column.Item().PaddingTop(34).Element(ComposeMetaSection);
            column.Item().PaddingTop(52).Element(content => ComposeEditorialSection(content, firstSection));
        });
    }

    private void ComposeProformContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Element(ComposeHeader);
            column.Item().PaddingTop(34).Element(ComposeMetaSection);
            column.Item().PaddingTop(15).Element(ComposeItemsTable);
            column.Item().PaddingTop(18).Element(ComposeTotalsSection);
        });
    }

    private void ComposeEditorialSection(IContainer container, EditorialSection section)
    {
        container.Column(column =>
        {
            column.Item().Text(section.Title)
                .FontColor(PrimaryColor)
                .FontSize(25)
                .ExtraBold();

            column.Item().PaddingTop(8).Text(_data.Number)
                .FontColor(MutedText)
                .FontSize(10.5f)
                .SemiBold();

            column.Item().PaddingTop(18).LineHorizontal(1).LineColor(PrimaryColor);

            column.Item().PaddingTop(24)
                .Element(content => ComposeBlockList(content, section.Blocks, 11.5f, 1.65f, PrimaryColor));
        });
    }

    private void ComposeHeader(IContainer container)
    {
        var phoneIconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "phone.png");
        var globeIconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "globe.png");
        var locationIconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "location.png");

        container.Row(row =>
        {
            row.RelativeItem();

            row.ConstantItem(340).Row(innerRow =>
            {
                innerRow.RelativeItem()
                    .AlignMiddle()
                    .Column(column =>
                    {
                        if (!string.IsNullOrWhiteSpace(_data.Phone))
                        {
                            column.Item().AlignRight().Row(iconRow =>
                            {
                                iconRow.RelativeItem().AlignRight().Text(_data.Phone).FontSize(12);
                                iconRow.ConstantItem(18)
                                    .PaddingLeft(4)
                                    .Height(14)
                                    .AlignMiddle()
                                    .AlignRight()
                                    .Element(icon =>
                                    {
                                        if (File.Exists(phoneIconPath))
                                        {
                                            icon.Image(phoneIconPath).FitArea();
                                        }
                                    });
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(_data.Website))
                        {
                            column.Item().AlignRight().Row(iconRow =>
                            {
                                iconRow.RelativeItem().AlignRight().Text(_data.Website).FontSize(12);
                                iconRow.ConstantItem(18)
                                    .PaddingLeft(4)
                                    .Height(14)
                                    .AlignMiddle()
                                    .AlignRight()
                                    .Element(icon =>
                                    {
                                        if (File.Exists(globeIconPath))
                                        {
                                            icon.Image(globeIconPath).FitArea();
                                        }
                                    });
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(_data.Address))
                        {
                            column.Item().AlignRight().Row(iconRow =>
                            {
                                iconRow.RelativeItem()
                                    .AlignRight()
                                    .Text(_data.Address)
                                    .FontSize(12);

                                iconRow.ConstantItem(20)
                                    .PaddingLeft(6)
                                    .Height(14)
                                    .AlignMiddle()
                                    .AlignRight()
                                    .Element(icon =>
                                    {
                                        if (File.Exists(locationIconPath))
                                        {
                                            icon.Image(locationIconPath).FitArea();
                                        }
                                    });
                            });
                        }
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
        if (_logoBytes is { Length: > 0 })
        {
            container.Image(_logoBytes).FitArea();
        }
    }

    private void ComposeMetaSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Text(_data.IssuedAtUtc.ToString("dd MMMM yyyy", new CultureInfo("es-ES")).ToUpperInvariant())
                .FontSize(10)
                .SemiBold();

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
                            text.Span("TELEFONO: ").ExtraBold();
                            text.Span(_data.ClientPhone!.ToUpperInvariant());
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(_data.ClientIdentificationNumber))
                    {
                        left.Item().PaddingTop(2).Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(11));
                            text.Span($"{GetIdentificationLabel()}: ").ExtraBold();
                            text.Span(_data.ClientIdentificationNumber!.ToUpperInvariant());
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(_data.Location))
                    {
                        left.Item().PaddingTop(2).Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(11));
                            text.Span(_data.Location!.ToUpperInvariant());
                        });
                    }
                });

                row.ConstantItem(260)
                    .AlignBottom()
                    .AlignRight()
                    .Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(11));
                        text.Span("COTIZACION: ").ExtraBold().FontColor(PrimaryColor);
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
                    header.Cell().PaddingLeft(14).Text("DESCRIPCION").FontSize(10).SemiBold();
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

            column.Item().PaddingTop(12).Row(row =>
            {
                row.RelativeItem();

                row.ConstantItem(320).Column(inner =>
                {
                    inner.Item().Element(rowContainer =>
                        ComposeSummaryLine(rowContainer, "Sub-Total", FormatCurrency(_data.Subtotal), false));

                    inner.Item().PaddingTop(8).Element(rowContainer =>
                        ComposeSummaryLine(rowContainer, _data.TaxLabel, FormatCurrency(_data.TaxAmount), false));

                    inner.Item().PaddingTop(8).LineHorizontal(1).LineColor(DividerColor);

                    inner.Item().PaddingTop(10).Element(rowContainer =>
                        ComposeSummaryLine(rowContainer, "Total", FormatCurrency(_data.Total), true));
                });
            });
        });
    }

    private void ComposeBlockList(
        IContainer container,
        IReadOnlyList<ProformPdfTextBlock> blocks,
        float fontSize,
        float lineHeight,
        string bulletColor)
    {
        container.Column(column =>
        {
            foreach (var block in blocks)
            {
                if (block.Kind == ProformPdfTextBlockKind.Bullet)
                {
                    column.Item().PaddingBottom(10).Row(row =>
                    {
                        row.ConstantItem(14)
                            .AlignTop()
                            .Text("•")
                            .FontColor(bulletColor)
                            .FontSize(fontSize + 1);

                        row.RelativeItem()
                            .Text(block.Text)
                            .Justify()
                            .FontSize(fontSize)
                            .LineHeight(lineHeight);
                    });

                    continue;
                }

                column.Item()
                    .PaddingBottom(12)
                    .Text(block.Text)
                    .Justify()
                    .FontSize(fontSize)
                    .LineHeight(lineHeight);
            }
        });
    }

    private void ComposeSummaryLine(IContainer container, string label, string value, bool emphasize)
    {
        container.Row(row =>
        {
            row.RelativeItem()
                .Text(label)
                .FontSize(emphasize ? 14 : 12)
                .FontColor(emphasize ? BlackText : MutedText)
                .ExtraBold();

            row.ConstantItem(120)
                .AlignRight()
                .Text(value)
                .FontSize(emphasize ? 14 : 12)
                .Bold();
        });
    }

    private IReadOnlyList<EditorialSection> GetEditorialSections()
    {
        var sections = new List<EditorialSection>();

        TryAddSection(sections, "Descripción del servicio", _data.ServiceDescription);
        TryAddSection(sections, "Alcances del trabajo", _data.ScopeOfWork);
        TryAddSection(
            sections,
            "Condiciones del servicio",
            ProformPdfRichTextParser.ParseServiceConditions(_data.ServiceConditions, _data.TermsAndConditions));
        TryAddSection(sections, "Condiciones de pago", _data.PaymentConditions);

        return sections;
    }

    private static void TryAddSection(ICollection<EditorialSection> sections, string title, string? content)
    {
        var blocks = ProformPdfRichTextParser.Parse(content);

        if (blocks.Count > 0)
        {
            sections.Add(new EditorialSection(title, blocks));
        }
    }

    private static void TryAddSection(
        ICollection<EditorialSection> sections,
        string title,
        IReadOnlyList<ProformPdfTextBlock> blocks)
    {
        if (blocks.Count > 0)
        {
            sections.Add(new EditorialSection(title, blocks));
        }
    }

    private string GetIdentificationLabel()
    {
        return _data.ClientIdentificationType switch
        {
            "PhysicalId" => "CEDULA FISICA",
            "LegalEntityId" => "CEDULA JURIDICA",
            _ => "IDENTIFICACION"
        };
    }

    private string FormatCurrency(decimal amount)
    {
        return $"{_data.CurrencySymbol}{amount:N2}";
    }

    private sealed record EditorialSection(string Title, IReadOnlyList<ProformPdfTextBlock> Blocks);
}
