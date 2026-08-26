using ESM.Domain.Interfaces;

namespace ESM.Domain.Entities;

public class ServiceCategory : BaseEntity, ISoftDeletable
{
    public string Name { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<Service> Services { get; set; } = new List<Service>();
}
