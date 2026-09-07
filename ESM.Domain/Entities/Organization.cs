using ESM.Domain.Interfaces;

namespace ESM.Domain.Entities;

public class Organization : BaseEntity, ISoftDeletable
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public ICollection<Department> Departments { get; set; } = new List<Department>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
}
