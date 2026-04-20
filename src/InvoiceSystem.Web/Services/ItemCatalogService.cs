using InvoiceSystem.Shared.Data;
using InvoiceSystem.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSystem.Web.Services;

public sealed class ItemCatalogService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public ItemCatalogService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public IReadOnlyList<ItemCatalogEntry> GetAll()
    {
        using var db = _dbFactory.CreateDbContext();
        return db.ItemCatalogEntries.OrderBy(i => i.Name).ToList();
    }

    public ItemCatalogEntry Add(ItemCatalogEntry item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        var name = item.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Item name is required.");

        using var db = _dbFactory.CreateDbContext();
        if (db.ItemCatalogEntries.Any(i => i.Name.ToLower() == name.ToLower()))
            throw new InvalidOperationException("An item with this name already exists.");

        item.Name = name;
        item.Details = item.Details?.Trim();
        db.ItemCatalogEntries.Add(item);
        db.SaveChanges();
        return item;
    }

    public bool Update(ItemCatalogEntry item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        var name = item.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Item name is required.");

        using var db = _dbFactory.CreateDbContext();
        if (db.ItemCatalogEntries.Any(i => i.Id != item.Id && i.Name.ToLower() == name.ToLower()))
            throw new InvalidOperationException("An item with this name already exists.");

        var existing = db.ItemCatalogEntries.FirstOrDefault(i => i.Id == item.Id);
        if (existing is null) return false;

        existing.Name = name;
        existing.Details = item.Details?.Trim();
        db.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        using var db = _dbFactory.CreateDbContext();
        var item = db.ItemCatalogEntries.FirstOrDefault(i => i.Id == id);
        if (item is null) return false;

        db.ItemCatalogEntries.Remove(item);
        db.SaveChanges();
        return true;
    }
}
