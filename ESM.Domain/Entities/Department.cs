using ESM.Domain.Interfaces;

namespace ESM.Domain.Entities;

public class Department : BaseEntity, ISoftDeletable
{
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Optional Manager
    public Guid? ManagerId { get; set; }
    public Employee? Manager { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Organization Organization { get; set; } = null!;
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
