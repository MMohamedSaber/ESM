using ESM.Domain.Enums;
using ESM.Domain.Interfaces;

namespace ESM.Domain.Entities;

public class Customer : BaseEntity, ISoftDeletable
{
    public string UserId { get; set; } = string.Empty;
    public string CustomerNumber { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public CustomerStatus Status { get; set; } = CustomerStatus.Active;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
