using System.ComponentModel.DataAnnotations;

namespace InvoiceSystem.Shared.Models;

public sealed class ItemCatalogEntry
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Details { get; set; }
}
