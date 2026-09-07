using ESM.Domain.Enums;
using ESM.Domain.Interfaces;

namespace ESM.Domain.Entities;

public class Employee : BaseEntity, ISoftDeletable
{
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    public string EmployeeNumber { get; set; } = string.Empty;
    
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public Guid PositionId { get; set; }
    public Position Position { get; set; } = null!;

    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
