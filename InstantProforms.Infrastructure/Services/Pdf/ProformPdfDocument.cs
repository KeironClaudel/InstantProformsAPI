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
    private string SecondaryColor => string.IsNullOrWhiteSpace(_data.SecondaryColor) ? "#e6c7f0" : _data.SecondaryColor;
    private string AccentColor => string.IsNullOrWhiteSpace(_data.AccentColor) ? "#dbe2ff" : _data.AccentColor;

    private const string BlackText = "#111111";
    private const string DividerColor = "#222222";

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
            column.Item().PaddingTop(15).Element(ComposeItemsTable);
            column.Item().PaddingTop(18).Element(ComposeTotalsSection);
            column.Item().PaddingTop(20).Element(ComposeConditionsSection);
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
                                            icon.Image(phoneIconPath).FitArea();
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
                                            icon.Image(globeIconPath).FitArea();
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
                                            icon.Image(locationIconPath).FitArea();
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
                text.Span(_data.IssuedAtUtc.ToString("dd MMMM yyyy", new CultureInfo("es-ES")).ToUpperInvariant())
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
                        text.Span("CLIENTE: ").ExtraBold(); ;
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

                    if (!string.IsNullOrWhiteSpace(_data.Notes))
                    {
                        left.Item().PaddingTop(2).Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(11));
                            text.Span(_data.Notes!.ToUpperInvariant());
                        });
                    }
                });

                row.ConstantItem(260)
                    .AlignBottom()
                    .AlignRight()
                    .Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(11));
                        text.Span("COTIZACIÓN: ").ExtraBold().FontColor(PrimaryColor);
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
                            .Text(_data.TaxLabel)
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
        var conditions = GetConditions();

        if (conditions.Count == 0)
        {
            return;
        }

        container.Row(row =>
        {
            row.ConstantItem(380).Column(column =>
            {
                column.Item().Text("CONDICIONES")
                    .FontColor(PrimaryColor)
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
                                .FontColor(PrimaryColor)
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

    private string FormatCurrency(decimal amount)
    {
        return $"{_data.CurrencySymbol}{amount:N0}";
    }

    private IReadOnlyList<string> GetConditions()
    {
        if (string.IsNullOrWhiteSpace(_data.TermsAndConditions))
        {
            return Array.Empty<string>();
        }

        var normalized = _data.TermsAndConditions
            .Replace("\\n", "\n")
            .Replace("\r", string.Empty)
            .Trim();

        // Preferred format:
        // one bullet per paragraph, separated by a blank line
        if (normalized.Contains("\n\n"))
        {
            return normalized
                .Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(NormalizeConditionText)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }

        // Secondary format:
        // line-based content. If all lines look like continuation lines, merge them.
        if (normalized.Contains('\n'))
        {
            var lines = normalized
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            // Heuristic:
            // if every line ends with punctuation, treat each line as a bullet
            var everyLineLooksIndependent = lines.All(x =>
                x.EndsWith('.') || x.EndsWith(';') || x.EndsWith(':'));

            if (everyLineLooksIndependent)
            {
                return lines
                    .Select(NormalizeConditionText)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();
            }

            // Otherwise, treat the full text as paragraphs with internal wraps
            return new List<string>
        {
            NormalizeConditionText(string.Join(" ", lines))
        };
        }

        // Fallback:
        // split by sentence endings followed by a space
        var sentenceBased = System.Text.RegularExpressions.Regex
            .Split(normalized, @"(?<=\.)\s+(?=[A-ZÁÉÍÓÚÑ])")
            .Select(NormalizeConditionText)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        return sentenceBased.Count > 0
            ? sentenceBased
            : new List<string> { NormalizeConditionText(normalized) };
    }

    private static string NormalizeConditionText(string value)
    {
        return System.Text.RegularExpressions.Regex
            .Replace(value, @"\s+", " ")
            .Trim();
    }
}
