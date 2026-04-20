using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceSystem.Shared.Models;

/// <summary>
/// Represents an invoice issued to a customer.
/// </summary>
public class Invoice
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime IssuedDate { get; set; } = DateTime.UtcNow;

    public DateTime? DueDate { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Foreign key to Customer
    public int CustomerId { get; set; }

    // Navigation property – the customer this invoice belongs to
    public Customer Customer { get; set; } = null!;

    // Navigation property – the line items that make up this invoice
    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();

    /// <summary>
    /// Calculated total (sum of all line-item subtotals). Not stored in the database.
    /// </summary>
    [NotMapped]
    public decimal TotalAmount => LineItems.Sum(li => li.Subtotal);
}

public enum InvoiceStatus
{
    Draft,
    Sent,
    Paid,
    Overdue,
    Cancelled
}
