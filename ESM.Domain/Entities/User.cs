using Microsoft.AspNetCore.Identity;

namespace ESM.Domain.Entities;

public class User : IdentityUser
{
    public string Name { get; set; } = string.Empty;

    public Guid? OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public Employee? Employee { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
}
