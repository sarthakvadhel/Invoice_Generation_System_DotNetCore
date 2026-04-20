using InvoiceSystem.Shared.Data;
using InvoiceSystem.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSystem.Web.Services;

public sealed class CustomerService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public CustomerService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public IReadOnlyList<Customer> GetAll()
    {
        using var db = _dbFactory.CreateDbContext();
        return db.Customers
            .Include(c => c.Invoices)
            .OrderBy(c => c.Name)
            .ToList();
    }

    public Customer? GetById(int id)
    {
        using var db = _dbFactory.CreateDbContext();
        return db.Customers
            .Include(c => c.Invoices)
            .FirstOrDefault(c => c.Id == id);
    }

    public Customer Add(Customer customer)
    {
        if (customer is null) throw new ArgumentNullException(nameof(customer));

        using var db = _dbFactory.CreateDbContext();
        customer.CreatedAt = DateTime.UtcNow;
        db.Customers.Add(customer);
        db.SaveChanges();
        return customer;
    }

    public bool Update(Customer customer)
    {
        if (customer is null) throw new ArgumentNullException(nameof(customer));

        using var db = _dbFactory.CreateDbContext();
        var existing = db.Customers.FirstOrDefault(c => c.Id == customer.Id);
        if (existing is null) return false;

        existing.Name = customer.Name;
        existing.Email = customer.Email;
        existing.Phone = customer.Phone;
        existing.Address = customer.Address;
        db.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        using var db = _dbFactory.CreateDbContext();
        var customer = db.Customers
            .Include(c => c.Invoices)
            .FirstOrDefault(c => c.Id == id);

        if (customer is null) return false;
        if (customer.Invoices is { Count: > 0 }) return false;

        db.Customers.Remove(customer);
        db.SaveChanges();
        return true;
    }
}
