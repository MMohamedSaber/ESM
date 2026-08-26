using ESM.Domain.Interfaces;

namespace ESM.Domain.Entities;

public class Service : BaseEntity, ISoftDeletable
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public int DurationInMinutes { get; set; }
    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ServiceCategory Category { get; set; } = null!;
}
