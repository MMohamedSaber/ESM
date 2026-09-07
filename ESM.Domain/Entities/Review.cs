using ESM.Domain.Interfaces;

namespace ESM.Domain.Entities;

public class Review : BaseEntity, ISoftDeletable
{
    public Guid ServiceRequestId { get; set; }
    public ServiceRequest ServiceRequest { get; set; } = null!;

    public int Rating { get; set; }
    public string? Comments { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
