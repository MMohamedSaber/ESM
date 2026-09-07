namespace ESM.Application.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    Guid? OrganizationId { get; }
    IList<string> Roles { get; }
    bool IsSuperAdmin { get; }
}
