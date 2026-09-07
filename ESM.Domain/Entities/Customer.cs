using ESM.Domain.Enums;
using ESM.Domain.Interfaces;

namespace ESM.Domain.Entities;

public class Customer : BaseEntity, ISoftDeletable
{
    public string? UserId { get; set; }
    public User? User { get; set; }

    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public string CustomerNumber { get; set; } = string.Empty;
    public string? DefaultAddress { get; set; }
    public string? Phone { get; set; }
    public CustomerStatus Status { get; set; } = CustomerStatus.Active;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
}
