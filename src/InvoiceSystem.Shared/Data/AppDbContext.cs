using InvoiceSystem.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSystem.Shared.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLineItem> InvoiceLineItems => Set<InvoiceLineItem>();
    public DbSet<ItemCatalogEntry> ItemCatalogEntries => Set<ItemCatalogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Invoices)
            .WithOne(i => i.Customer)
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Invoice>()
            .HasMany(i => i.LineItems)
            .WithOne(li => li.Invoice)
            .HasForeignKey(li => li.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Customer>().Property(c => c.Name).IsRequired().HasMaxLength(200);
        modelBuilder.Entity<Customer>().Property(c => c.Email).IsRequired().HasMaxLength(320);
        modelBuilder.Entity<Customer>().Property(c => c.Phone).HasMaxLength(20);
        modelBuilder.Entity<Customer>().Property(c => c.Address).HasMaxLength(500);

        modelBuilder.Entity<Invoice>().Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<Invoice>().Property(i => i.Notes).HasMaxLength(1000);

        modelBuilder.Entity<InvoiceLineItem>().Property(li => li.Description).IsRequired().HasMaxLength(300);

        modelBuilder.Entity<ItemCatalogEntry>().Property(e => e.Name).IsRequired().HasMaxLength(200);
        modelBuilder.Entity<ItemCatalogEntry>().Property(e => e.Details).HasMaxLength(500);
        modelBuilder.Entity<ItemCatalogEntry>().HasIndex(e => e.Name).IsUnique();
    }
}
