using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceSystem.Shared.Models;

/// <summary>
/// Represents a single line item on an invoice (product or service entry).
/// </summary>
public class InvoiceLineItem
{
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Optional tax rate as a percentage, e.g. 10 for 10%. Defaults to 0 (no tax).
    /// </summary>
    [Range(0, 100)]
    public decimal TaxRate { get; set; } = 0;

    /// <summary>
    /// Subtotal including tax (Quantity × UnitPrice × (1 + TaxRate / 100)). Not stored in the database.
    /// </summary>
    [NotMapped]
    public decimal Subtotal => Quantity * UnitPrice * (1 + TaxRate / 100m);

    // Foreign key to Invoice
    public int InvoiceId { get; set; }

    // Navigation property – the invoice this line item belongs to
    public Invoice Invoice { get; set; } = null!;
}
