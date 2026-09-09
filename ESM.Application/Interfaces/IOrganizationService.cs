using ESM.Application.Common.Models;
using ESM.Application.DTOs.Organization;

namespace ESM.Application.Interfaces;

public interface IOrganizationService
{
    Task<ApiResponse<IEnumerable<OrganizationDto>>> GetAllAsync();
}
