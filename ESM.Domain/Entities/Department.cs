using ESM.Domain.Interfaces;

namespace ESM.Domain.Entities;

public class Department : BaseEntity, ISoftDeletable
{
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
