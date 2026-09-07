namespace ESM.Application.DTOs.Auth;

public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid? OrganizationId { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}
