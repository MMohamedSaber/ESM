using ESM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESM.Application.Interfaces;

public interface IEsmDbContext
{
    DbSet<Organization> Organizations { get; }
    DbSet<Department> Departments { get; }
    DbSet<Employee> Employees { get; }
    DbSet<Customer> Customers { get; }
    DbSet<ServiceCategory> ServiceCategories { get; }
    DbSet<Service> Services { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
