using InvoiceSystem.Shared.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InvoiceSystem.Web.Services;

public sealed class PdfService
{
    private const string SellerName    = "Aimbys Solutions Private Limited";
    private const string SellerLine1   = "a-612 Titanium City Centre, Bh. Aaykarbhavan";
    private const string SellerLine2   = "Anandnagar Road, Satellite, Ahmedabad – 380015";
    private const string SellerCIN     = "CIN: U72200GJ2014PTC080124";
    private const string SellerGST     = "GST: 24AAMCA8116G1ZR";
    private const string SellerEmail   = "info@aimbys.com";
    private const string SellerWeb     = "www.aimbys.com";

    private const string Navy   = "#0D2B55";
    private const string Accent = "#1A6EC2";
    private const string Light  = "#EEF4FB";
    private const string Muted  = "#6B7280";
    private const string Border = "#D1DCE8";
    private const string White  = "#FFFFFF";
    private const string Dark   = "#1F2937";
    private const string Ink    = "#374151";

    private readonly string _logoSvg;
    private readonly IWebHostEnvironment _env;

    public PdfService(IWebHostEnvironment env)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        _env = env;
        var logoPath = Path.Combine(env.WebRootPath, "images", "aimbys-logo.svg");
        _logoSvg = File.Exists(logoPath) ? File.ReadAllText(logoPath) : string.Empty;
    }

    public byte[] Generate(Invoice invoice)
    {
        var customer  = invoice.Customer;
        var lines     = invoice.LineItems.ToList();

        decimal subtotal   = lines.Sum(l => l.Quantity * l.UnitPrice);
        decimal totalTax   = lines.Sum(l => l.Quantity * l.UnitPrice * (l.TaxRate / 100m));
        decimal grandTotal = subtotal + totalTax;

        return Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(0);
                page.DefaultTextStyle(t => t.FontFamily("Helvetica").FontSize(9.5f).FontColor(Dark));

                // ── HEADER ────────────────────────────────────────────────────
                page.Header().Column(header =>
                {
                    header.Item().Background(Navy).Padding(36).Row(row =>
                    {
                        // Logo + company block
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Row(logoRow =>
                            {
                                if (!string.IsNullOrWhiteSpace(_logoSvg))
                                {
                                    logoRow.ConstantItem(52).Height(52).Svg(_logoSvg);
                                    logoRow.ConstantItem(14);
                                }
                                logoRow.RelativeItem().Column(nameCol =>
                                {
                                    nameCol.Item().Text(SellerName)
                                        .FontSize(15).Bold().FontColor(White);
                                    nameCol.Item().PaddingTop(3).Text(SellerLine1)
                                        .FontSize(8).FontColor("#A8C4E0");
                                    nameCol.Item().Text(SellerLine2)
                                        .FontSize(8).FontColor("#A8C4E0");
                                    nameCol.Item().PaddingTop(5).Row(r =>
                                    {
                                        r.AutoItem().Text(SellerCIN).FontSize(7.5f).FontColor("#7BAFD4");
                                        r.ConstantItem(16);
                                        r.AutoItem().Text(SellerGST).FontSize(7.5f).FontColor("#7BAFD4");
                                    });
                                    nameCol.Item().PaddingTop(2).Row(r =>
                                    {
                                        r.AutoItem().Text(SellerEmail).FontSize(7.5f).FontColor("#7BAFD4");
                                        r.ConstantItem(16);
                                        r.AutoItem().Text(SellerWeb).FontSize(7.5f).FontColor("#7BAFD4");
                                    });
                                });
                            });
                        });

                        // INVOICE title + meta
                        row.ConstantItem(190).AlignRight().Column(col =>
                        {
                            col.Item().AlignRight().Text("INVOICE")
                                .FontSize(32).Bold().FontColor(White);
                            col.Item().PaddingTop(8).AlignRight().Text($"# {invoice.InvoiceNumber}")
                                .FontSize(11).FontColor("#A8C4E0");
                            col.Item().PaddingTop(3).AlignRight()
                                .Text($"Date: {invoice.IssuedDate:dd MMM yyyy}")
                                .FontSize(9).FontColor("#A8C4E0");
                            if (invoice.DueDate.HasValue)
                                col.Item().PaddingTop(2).AlignRight()
                                    .Text($"Due: {invoice.DueDate:dd MMM yyyy}")
                                    .FontSize(9).FontColor("#FFCA28");
                            col.Item().PaddingTop(8).AlignRight()
                                .Background(StatusColour(invoice.Status))
                                .PaddingVertical(5).PaddingHorizontal(12)
                                .Text(invoice.Status.ToString().ToUpperInvariant())
                                .FontSize(8.5f).Bold().FontColor(White);
                        });
                    });

                    // ── ACCENT STRIPE ─────────────────────────────────────────
                    header.Item().Height(5).Background(Accent);
                });

                // ── CONTENT ───────────────────────────────────────────────────
                page.Content().Column(col =>
                {

                    // Bill From / Bill To
                    col.Item().Background(Light).PaddingHorizontal(36).PaddingVertical(24).Row(row =>
                    {
                        // Bill From
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("BILL FROM")
                                .FontSize(7.5f).Bold().FontColor(Muted).LetterSpacing(0.1f);
                            c.Item().PaddingTop(6).Text(SellerName)
                                .FontSize(11).Bold().FontColor(Navy);
                            c.Item().PaddingTop(2).Text(SellerLine1)
                                .FontSize(9).FontColor(Ink);
                            c.Item().Text(SellerLine2)
                                .FontSize(9).FontColor(Ink);
                            c.Item().PaddingTop(4).Text(SellerCIN)
                                .FontSize(8).FontColor(Muted);
                            c.Item().Text(SellerGST)
                                .FontSize(8).FontColor(Muted);
                        });

                        row.ConstantItem(1).Background(Border);

                        // Bill To
                        row.RelativeItem().PaddingLeft(28).Column(c =>
                        {
                            c.Item().Text("BILL TO")
                                .FontSize(7.5f).Bold().FontColor(Muted).LetterSpacing(0.1f);
                            c.Item().PaddingTop(6).Text(customer?.Name ?? "—")
                                .FontSize(11).Bold().FontColor(Navy);
                            if (!string.IsNullOrWhiteSpace(customer?.Address))
                                c.Item().PaddingTop(2).Text(customer.Address)
                                    .FontSize(9).FontColor(Ink);
                            if (!string.IsNullOrWhiteSpace(customer?.Email))
                                c.Item().PaddingTop(2).Text(customer.Email)
                                    .FontSize(9).FontColor(Muted);
                            if (!string.IsNullOrWhiteSpace(customer?.Phone))
                                c.Item().Text(customer.Phone)
                                    .FontSize(9).FontColor(Muted);
                        });
                    });

                    // Line items table
                    col.Item().PaddingHorizontal(36).PaddingTop(24).Column(c =>
                    {
                        c.Item().Text("ITEMS & SERVICES")
                            .FontSize(7.5f).Bold().FontColor(Muted).LetterSpacing(0.1f);
                        c.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(cd =>
                            {
                                cd.ConstantColumn(30);    // #
                                cd.RelativeColumn(4.5f);  // Description
                                cd.RelativeColumn(1.2f);  // Qty
                                cd.RelativeColumn(1.8f);  // Unit Price
                                cd.RelativeColumn(1f);    // Tax %
                                cd.RelativeColumn(1.8f);  // Amount
                            });

                            IContainer TH(IContainer x) =>
                                x.Background(Navy).PaddingVertical(10).PaddingHorizontal(8);

                            table.Header(h =>
                            {
                                h.Cell().Element(TH).Text("#").FontSize(8).Bold().FontColor(White);
                                h.Cell().Element(TH).Text("DESCRIPTION").FontSize(8).Bold().FontColor(White);
                                h.Cell().Element(TH).AlignRight().Text("QTY").FontSize(8).Bold().FontColor(White);
                                h.Cell().Element(TH).AlignRight().Text("UNIT PRICE").FontSize(8).Bold().FontColor(White);
                                h.Cell().Element(TH).AlignRight().Text("TAX %").FontSize(8).Bold().FontColor(White);
                                h.Cell().Element(TH).AlignRight().Text("AMOUNT").FontSize(8).Bold().FontColor(White);
                            });

                            for (int i = 0; i < lines.Count; i++)
                            {
                                var li = lines[i];
                                var bg = i % 2 == 0 ? White : Light;
                                var amount = li.Quantity * li.UnitPrice * (1 + li.TaxRate / 100m);

                                IContainer TD(IContainer x) =>
                                    x.Background(bg).BorderBottom(1).BorderColor(Border)
                                     .PaddingVertical(9).PaddingHorizontal(8);

                                table.Cell().Element(TD).Text($"{i + 1}").FontSize(9).FontColor(Muted);
                                table.Cell().Element(TD).Text(li.Description).FontSize(9);
                                table.Cell().Element(TD).AlignRight().Text($"{li.Quantity:0.##}").FontSize(9);
                                table.Cell().Element(TD).AlignRight().Text($"₹{li.UnitPrice:N2}").FontSize(9);
                                table.Cell().Element(TD).AlignRight().Text($"{li.TaxRate:0.##}%").FontSize(9);
                                table.Cell().Element(TD).AlignRight().Text($"₹{amount:N2}").FontSize(9).Bold();
                            }
                        });
                    });

                    // Summary + Notes
                    col.Item().PaddingHorizontal(36).PaddingTop(28).Row(row =>
                    {
                        // Notes / Terms (left)
                        row.RelativeItem().PaddingRight(20).Column(c =>
                        {
                            c.Item().Text("NOTES & TERMS")
                                .FontSize(7.5f).Bold().FontColor(Muted).LetterSpacing(0.1f);
                            c.Item().PaddingTop(6).Text(
                                string.IsNullOrWhiteSpace(invoice.Notes)
                                    ? "Payment is due within 30 days of the invoice date. Please include the invoice number in your payment reference."
                                    : invoice.Notes)
                                .FontSize(8.5f).FontColor(Ink).LineHeight(1.5f);

                            c.Item().PaddingTop(16).Text("BANK DETAILS")
                                .FontSize(7.5f).Bold().FontColor(Muted).LetterSpacing(0.1f);
                            c.Item().PaddingTop(6).Column(bank =>
                            {
                                BankRow(bank, "Bank Name", "HDFC Bank");
                                BankRow(bank, "Account No.", "XXXX XXXX XXXX");
                                BankRow(bank, "IFSC Code", "HDFC0000000");
                                BankRow(bank, "Branch", "Satellite, Ahmedabad");
                            });
                        });

                        // Totals (right)
                        row.ConstantItem(230).Column(c =>
                        {
                            c.Item().Text("SUMMARY")
                                .FontSize(7.5f).Bold().FontColor(Muted).LetterSpacing(0.1f);
                            c.Item().PaddingTop(6).Border(1).BorderColor(Border).Column(totals =>
                            {
                                TotalRow(totals, "Subtotal (excl. tax)", $"₹{subtotal:N2}", false);
                                TotalRow(totals, "Total Tax", $"₹{totalTax:N2}", false);
                                totals.Item().Background(Navy).PaddingVertical(12).PaddingHorizontal(14).Row(r =>
                                {
                                    r.RelativeItem().Text("GRAND TOTAL").FontSize(11).Bold().FontColor(White);
                                    r.AutoItem().Text($"₹{grandTotal:N2}").FontSize(11).Bold().FontColor(White);
                                });
                            });
                        });
                    });

                    // Signature / Stamp
                    col.Item().PaddingHorizontal(36).PaddingTop(36).PaddingBottom(28).Row(row =>
                    {
                        // Stamp box
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("COMPANY STAMP & SIGNATURE")
                                .FontSize(7.5f).Bold().FontColor(Muted).LetterSpacing(0.1f);
                            c.Item().PaddingTop(6).Height(72).Border(1).BorderColor(Border)
                                .Background(Light).AlignCenter().AlignMiddle()
                                .Text("Stamp Here").FontSize(8.5f).FontColor("#9CA3AF");
                            c.Item().PaddingTop(6).Text($"For {SellerName}")
                                .FontSize(8.5f).Bold().FontColor(Navy);
                            c.Item().Text("Authorised Signatory")
                                .FontSize(8).FontColor(Muted);
                        });

                        row.ConstantItem(36);

                        // Declaration
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("DECLARATION")
                                .FontSize(7.5f).Bold().FontColor(Muted).LetterSpacing(0.1f);
                            c.Item().PaddingTop(6)
                                .Text("We declare that this invoice shows the actual price of the goods / services described and that all particulars are true and correct to the best of our knowledge.")
                                .FontSize(8.5f).FontColor(Ink).LineHeight(1.5f);
                            c.Item().PaddingTop(10).Text("This is a computer-generated invoice and does not require a physical signature unless stamped.")
                                .FontSize(7.5f).FontColor(Muted).LineHeight(1.4f);
                        });
                    });
                });

                // ── FOOTER ────────────────────────────────────────────────────
                page.Footer().Background(Navy).PaddingVertical(12).PaddingHorizontal(36).Row(row =>
                {
                    row.RelativeItem().AlignMiddle().Text(
                        $"{SellerName}  ·  {SellerCIN}  ·  {SellerGST}")
                        .FontSize(7).FontColor("#7BAFD4");
                    row.AutoItem().AlignMiddle().Text("Thank you for your business!")
                        .FontSize(7.5f).Bold().FontColor("#A8C4E0");
                });
            });
        }).GeneratePdf();
    }

    private static void TotalRow(ColumnDescriptor col, string label, string value, bool grand)
    {
        col.Item().Background(White).BorderBottom(1).BorderColor(Border)
            .PaddingVertical(8).PaddingHorizontal(14).Row(r =>
            {
                r.RelativeItem().Text(label).FontSize(9).FontColor(Muted);
                r.AutoItem().Text(value).FontSize(9).Bold().FontColor(Dark);
            });
    }

    private static void BankRow(ColumnDescriptor col, string label, string value)
    {
        col.Item().PaddingBottom(3).Row(r =>
        {
            r.ConstantItem(90).Text(label).FontSize(8).FontColor(Muted);
            r.RelativeItem().Text(value).FontSize(8).FontColor(Dark);
        });
    }

    private static string StatusColour(InvoiceStatus status) => status switch
    {
        InvoiceStatus.Paid      => "#16A34A",
        InvoiceStatus.Sent      => "#2563EB",
        InvoiceStatus.Overdue   => "#DC2626",
        InvoiceStatus.Cancelled => "#6B7280",
        _                       => "#D97706",
    };
}
