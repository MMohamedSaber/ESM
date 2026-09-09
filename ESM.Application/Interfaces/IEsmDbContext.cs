using ESM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESM.Application.Interfaces;

public interface IEsmDbContext
{
    DbSet<User> Users { get; }
    DbSet<Organization> Organizations { get; }
    DbSet<Department> Departments { get; }
    DbSet<Employee> Employees { get; }
    DbSet<Customer> Customers { get; }
    DbSet<ServiceCategory> ServiceCategories { get; }
    DbSet<Service> Services { get; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<ServiceRequest> ServiceRequests { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
