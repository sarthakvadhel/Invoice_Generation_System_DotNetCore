using System.ComponentModel.DataAnnotations;

namespace InvoiceSystem.Shared.Models;

/// <summary>
/// Represents a customer who can be billed via invoices.
/// </summary>
public class Customer
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(320), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property – a customer can have many invoices
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
