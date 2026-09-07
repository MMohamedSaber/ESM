using ESM.Domain.Entities;
using ESM.Domain.Enums;
using ESM.Application.Common.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ESM.Infrastructure.Data;

public class EsmDbSeeder
{
    private readonly EsmDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public EsmDbSeeder(EsmDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        // 1. Seed Roles
        var roles = new[] { Roles.SuperAdmin, Roles.Admin, Roles.Employee, Roles.Customer };
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Ensure default Organization exists
        var org = await _context.Organizations.FirstOrDefaultAsync();
        bool orgCreated = false;
        if (org == null)
        {
            org = new Organization
            {
                NameAr = "شركة الأمانة التجارية",
                NameEn = "Al-Amana Trading Co."
            };
            _context.Organizations.Add(org);
            await _context.SaveChangesAsync();
            orgCreated = true;
        }

        // 3. Ensure SuperAdmin exists
        var superAdmin = new User
        {
            UserName = "superadmin@esm.com",
            Email = "superadmin@esm.com",
            Name = "System SuperAdmin",
            OrganizationId = null // SuperAdmin is global
        };
        if (await _userManager.FindByEmailAsync(superAdmin.Email) == null)
        {
            await _userManager.CreateAsync(superAdmin, "P@ssw0rd123");
            await _userManager.AddToRoleAsync(superAdmin, Roles.SuperAdmin);
        }

        // Only seed demo data if the organization was just created
        if (!orgCreated) return;

        // 3. ServiceCategory
        var category = new ServiceCategory { Name = "IT Maintenance" };
        _context.ServiceCategories.Add(category);
        await _context.SaveChangesAsync();

        // 4. Service
        var service = new Service
        {
            OrganizationId = org.Id,
            CategoryId = category.Id,
            Name = "Network Diagnostics",
            Description = "Full network health check",
            BasePrice = 150.00m,
            DurationInMinutes = 60,
            IsActive = true
        };
        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        // 5. Position
        var position = new Position { Name = "Senior Technician", Description = "Handles complex technical issues." };
        _context.Positions.Add(position);
        await _context.SaveChangesAsync();

        // 6. Department
        var department = new Department { OrganizationId = org.Id, Name = "IT Support", Description = "Level 2 Support" };
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        // 7. Users
        var adminUser = new User
        {
            UserName = "admin@esm.com",
            Email = "admin@esm.com",
            Name = "System Admin",
            OrganizationId = org.Id
        };
        if (await _userManager.FindByEmailAsync(adminUser.Email) == null)
        {
            await _userManager.CreateAsync(adminUser, "P@ssw0rd123");
            await _userManager.AddToRoleAsync(adminUser, Roles.Admin);
        }

        var empUser = new User
        {
            UserName = "employee@esm.com",
            Email = "employee@esm.com",
            Name = "John Doe",
            OrganizationId = org.Id
        };
        if (await _userManager.FindByEmailAsync(empUser.Email) == null)
        {
            await _userManager.CreateAsync(empUser, "P@ssw0rd123");
            await _userManager.AddToRoleAsync(empUser, Roles.Employee);
        }

        var custUser = new User
        {
            UserName = "customer@esm.com",
            Email = "customer@esm.com",
            Name = "Alice Smith",
            OrganizationId = org.Id
        };
        if (await _userManager.FindByEmailAsync(custUser.Email) == null)
        {
            await _userManager.CreateAsync(custUser, "P@ssw0rd123");
            await _userManager.AddToRoleAsync(custUser, Roles.Customer);
        }

        // 8. Employee
        var employee = new Employee
        {
            UserId = empUser.Id,
            EmployeeNumber = "EMP-001",
            DepartmentId = department.Id,
            PositionId = position.Id,
            HireDate = DateTime.UtcNow.AddYears(-2),
            Salary = 5000.00m,
            Status = EmployeeStatus.Active
        };
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        // 9. Customer
        var customer = new Customer
        {
            UserId = custUser.Id,
            OrganizationId = org.Id,
            CustomerNumber = "CUST-100",
            DefaultAddress = "123 Main St, Tech City",
            Phone = "+1234567890",
            Status = CustomerStatus.Active
        };
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        // 10. ServiceRequest
        var serviceRequest = new ServiceRequest
        {
            OrganizationId = org.Id,
            CustomerId = customer.Id,
            ServiceId = service.Id,
            CreatedByUserId = custUser.Id,
            Status = ServiceRequestStatus.Pending,
            ServiceLocation = customer.DefaultAddress ?? "Branch Office"
        };
        _context.ServiceRequests.Add(serviceRequest);
        await _context.SaveChangesAsync();

        // 11. Appointment
        var appointment = new Appointment
        {
            ServiceRequestId = serviceRequest.Id,
            EmployeeId = employee.Id,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(2)
        };
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        // Update SR status
        serviceRequest.Status = ServiceRequestStatus.Assigned;
        await _context.SaveChangesAsync();

        // 12. Invoice
        var invoice = new Invoice
        {
            AppointmentId = appointment.Id,
            Status = InvoiceStatus.Draft,
            TotalAmount = service.BasePrice
        };
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        // 13. InvoiceItem
        var invoiceItem = new InvoiceItem
        {
            InvoiceId = invoice.Id,
            ServiceId = service.Id,
            ServiceName = service.Name, // Historical snapshot
            UnitPrice = service.BasePrice,
            Quantity = 1
        };
        _context.InvoiceItems.Add(invoiceItem);
        await _context.SaveChangesAsync();

        // Update Invoice status
        invoice.Status = InvoiceStatus.Sent;
        await _context.SaveChangesAsync();

        // 14. Payment
        var payment = new Payment
        {
            InvoiceId = invoice.Id,
            Method = PaymentMethod.CreditCard,
            Status = PaymentStatus.Completed,
            Amount = invoice.TotalAmount
        };
        _context.Payments.Add(payment);
        
        invoice.Status = InvoiceStatus.Paid;
        await _context.SaveChangesAsync();

        // 15. Review
        var review = new Review
        {
            ServiceRequestId = serviceRequest.Id,
            Rating = 5,
            Comments = "Excellent diagnostic service!"
        };
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
    }
}
