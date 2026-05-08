using InstantProforms.Infrastructure.Services.Pdf;
using Xunit;

namespace InstantProforms.Tests.Infrastructure.Services.Pdf;

public sealed class ProformPdfRichTextParserTests
{
    [Fact]
    public void Parse_WhenTextContainsParagraphsAndBullets_ReturnsStructuredBlocks()
    {
        const string content = """
            Primera linea del parrafo
            continua en la misma idea

            - Primer punto
            con detalle adicional

            2. Segundo punto

            Cierre del documento.
            """;

        var blocks = ProformPdfRichTextParser.Parse(content);

        Assert.Collection(
            blocks,
            block =>
            {
                Assert.Equal(ProformPdfTextBlockKind.Paragraph, block.Kind);
                Assert.Equal("Primera linea del parrafo continua en la misma idea", block.Text);
            },
            block =>
            {
                Assert.Equal(ProformPdfTextBlockKind.Bullet, block.Kind);
                Assert.Equal("Primer punto con detalle adicional", block.Text);
            },
            block =>
            {
                Assert.Equal(ProformPdfTextBlockKind.Bullet, block.Kind);
                Assert.Equal("Segundo punto", block.Text);
            },
            block =>
            {
                Assert.Equal(ProformPdfTextBlockKind.Paragraph, block.Kind);
                Assert.Equal("Cierre del documento.", block.Text);
            });
    }

    [Fact]
    public void Parse_WhenValueContainsEscapedNewLines_NormalizesThem()
    {
        const string content = "Parrafo inicial\\n\\n* Punto importante";

        var blocks = ProformPdfRichTextParser.Parse(content);

        Assert.Collection(
            blocks,
            block =>
            {
                Assert.Equal(ProformPdfTextBlockKind.Paragraph, block.Kind);
                Assert.Equal("Parrafo inicial", block.Text);
            },
            block =>
            {
                Assert.Equal(ProformPdfTextBlockKind.Bullet, block.Kind);
                Assert.Equal("Punto importante", block.Text);
            });
    }

    [Fact]
    public void ParseConditions_WhenTextContainsBlankLines_ReturnsOneConditionPerBlock()
    {
        const string content = """
            La garantia no cubre danos o modificaciones por terceros.

            Cualquier inconveniente debe reportarse a la empresa antes de intervenir.

            Si se autoriza reparacion por terceros, la garantia queda anulada.
            """;

        var blocks = ProformPdfRichTextParser.ParseConditions(content);

        Assert.Collection(
            blocks,
            block =>
            {
                Assert.Equal(ProformPdfTextBlockKind.Bullet, block.Kind);
                Assert.Equal("La garantia no cubre danos o modificaciones por terceros.", block.Text);
            },
            block =>
            {
                Assert.Equal(ProformPdfTextBlockKind.Bullet, block.Kind);
                Assert.Equal("Cualquier inconveniente debe reportarse a la empresa antes de intervenir.", block.Text);
            },
            block =>
            {
                Assert.Equal(ProformPdfTextBlockKind.Bullet, block.Kind);
                Assert.Equal("Si se autoriza reparacion por terceros, la garantia queda anulada.", block.Text);
            });
    }

    [Fact]
    public void ParseConditions_WhenTextArrivesAsSingleParagraph_SplitsSentencesIntoRows()
    {
        const string content =
            "La garantia no cubre danos por terceros. Cualquier inconveniente debe reportarse antes de intervenir. Si se autoriza reparacion por terceros, la garantia queda anulada.";

        var blocks = ProformPdfRichTextParser.ParseConditions(content);

        Assert.Equal(3, blocks.Count);
        Assert.All(blocks, block => Assert.Equal(ProformPdfTextBlockKind.Bullet, block.Kind));
    }

    [Fact]
    public void ParseServiceConditions_PreservesUserFormattingAndLegacyCompanyRows()
    {
        const string userContent = """
            Introduccion libre del usuario.

            - Punto personalizado
            """;

        const string companyDefaults =
            "Primera condicion heredada. Segunda condicion heredada.";

        var blocks = ProformPdfRichTextParser.ParseServiceConditions(userContent, companyDefaults);

        Assert.Collection(
            blocks,
            block =>
            {
                Assert.Equal(ProformPdfTextBlockKind.Paragraph, block.Kind);
                Assert.Equal("Introduccion libre del usuario.", block.Text);
            },
            block =>
            {
                Assert.Equal(ProformPdfTextBlockKind.Bullet, block.Kind);
                Assert.Equal("Punto personalizado", block.Text);
            },
            block =>
            {
                Assert.Equal(ProformPdfTextBlockKind.Bullet, block.Kind);
                Assert.Equal("Primera condicion heredada.", block.Text);
            },
            block =>
            {
                Assert.Equal(ProformPdfTextBlockKind.Bullet, block.Kind);
                Assert.Equal("Segunda condicion heredada.", block.Text);
            });
    }
}
