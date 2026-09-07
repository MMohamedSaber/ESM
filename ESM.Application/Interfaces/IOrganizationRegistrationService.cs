using ESM.Application.Common.Models;
using ESM.Application.DTOs.Organization;

namespace ESM.Application.Interfaces;

public interface IOrganizationRegistrationService
{
    Task<ApiResponse<bool>> RegisterOrganizationAsync(RegisterOrganizationDto request);
}
