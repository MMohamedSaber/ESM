using ESM.Domain.Interfaces;

namespace ESM.Domain.Entities;

public class Appointment : BaseEntity, ISoftDeletable
{
    public Guid ServiceRequestId { get; set; }
    public ServiceRequest ServiceRequest { get; set; } = null!;

    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Invoice? Invoice { get; set; }
}
