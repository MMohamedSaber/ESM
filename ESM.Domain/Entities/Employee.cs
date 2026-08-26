using ESM.Domain.Enums;
using ESM.Domain.Interfaces;

namespace ESM.Domain.Entities;

public class Employee : BaseEntity, ISoftDeletable
{
    public string UserId { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public Guid OrganizationId { get; set; }
    public string Position { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Department Department { get; set; } = null!;
    public Organization Organization { get; set; } = null!;
}
