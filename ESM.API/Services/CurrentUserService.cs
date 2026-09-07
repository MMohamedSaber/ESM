using System.Security.Claims;
using ESM.Application.Interfaces;
using ESM.Application.Common.Constants;

namespace ESM.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                             _httpContextAccessor.HttpContext?.User?.FindFirstValue(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub);

    public Guid? OrganizationId
    {
        get
        {
            var orgIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("OrganizationId")?.Value;
            if (Guid.TryParse(orgIdClaim, out var orgId))
            {
                return orgId;
            }
            return null;
        }
    }

    public IList<string> Roles => _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? new List<string>();

    public bool IsSuperAdmin => Roles.Contains(ESM.Application.Common.Constants.Roles.SuperAdmin);
}
