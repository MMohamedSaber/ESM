using ESM.Domain.Enums;
using ESM.Domain.Interfaces;

namespace ESM.Domain.Entities;

public class ServiceRequest : BaseEntity, ISoftDeletable
{
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public string CreatedByUserId { get; set; } = string.Empty;
    public User CreatedByUser { get; set; } = null!;

    public ServiceRequestStatus Status { get; set; }
    
    public string ServiceLocation { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Appointment? Appointment { get; set; }
    public Review? Review { get; set; }
}
