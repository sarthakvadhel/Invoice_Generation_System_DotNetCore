using InvoiceSystem.Shared.Data;
using InvoiceSystem.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSystem.Web.Services;

public sealed class InvoiceService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public InvoiceService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public IReadOnlyList<Invoice> GetAll()
    {
        using var db = _dbFactory.CreateDbContext();
        return db.Invoices
            .Include(i => i.Customer)
            .Include(i => i.LineItems)
            .OrderByDescending(i => i.IssuedDate)
            .ToList();
    }

    public Invoice? GetById(int id)
    {
        using var db = _dbFactory.CreateDbContext();
        return db.Invoices
            .Include(i => i.Customer)
            .Include(i => i.LineItems)
            .FirstOrDefault(i => i.Id == id);
    }

    public Invoice Add(Invoice invoice)
    {
        if (invoice is null) throw new ArgumentNullException(nameof(invoice));

        using var db = _dbFactory.CreateDbContext();
        var customer = db.Customers.FirstOrDefault(c => c.Id == invoice.CustomerId);
        if (customer is null) throw new InvalidOperationException("Customer not found.");

        invoice.LineItems ??= new List<InvoiceLineItem>();
        db.Invoices.Add(invoice);
        db.SaveChanges();

        // reload with navigation properties
        db.Entry(invoice).Reference(i => i.Customer).Load();
        return invoice;
    }

    public bool Update(int id, Invoice updatedInvoice)
    {
        if (updatedInvoice is null) throw new ArgumentNullException(nameof(updatedInvoice));

        using var db = _dbFactory.CreateDbContext();
        var customer = db.Customers.FirstOrDefault(c => c.Id == updatedInvoice.CustomerId);
        if (customer is null) throw new InvalidOperationException("Customer not found.");

        var existing = db.Invoices
            .Include(i => i.LineItems)
            .FirstOrDefault(i => i.Id == id);
        if (existing is null) return false;

        existing.InvoiceNumber = updatedInvoice.InvoiceNumber;
        existing.CustomerId = updatedInvoice.CustomerId;
        existing.IssuedDate = updatedInvoice.IssuedDate;
        existing.DueDate = updatedInvoice.DueDate;
        existing.Status = updatedInvoice.Status;
        existing.Notes = updatedInvoice.Notes;

        // replace line items
        db.InvoiceLineItems.RemoveRange(existing.LineItems);
        existing.LineItems = updatedInvoice.LineItems
            .Select(li => new InvoiceLineItem
            {
                InvoiceId = existing.Id,
                Description = li.Description,
                Quantity = li.Quantity,
                UnitPrice = li.UnitPrice,
                TaxRate = li.TaxRate,
            })
            .ToList();

        db.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        using var db = _dbFactory.CreateDbContext();
        var invoice = db.Invoices.FirstOrDefault(i => i.Id == id);
        if (invoice is null) return false;

        db.Invoices.Remove(invoice);
        db.SaveChanges();
        return true;
    }
}
