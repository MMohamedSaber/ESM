using ESM.Application.Common.Constants;
using ESM.Application.Common.Models;
using ESM.Application.DTOs.Organization;
using ESM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESM.API.Controllers;

[ApiController]
[Authorize(Roles = Roles.SuperAdmin)]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationRegistrationService _registrationService;
    private readonly IOrganizationService _organizationService;

    public OrganizationController(
        IOrganizationRegistrationService registrationService,
        IOrganizationService organizationService)
    {
        _registrationService = registrationService;
        _organizationService = organizationService;
    }

    [HttpPost(ApiRoutes.Organization.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterOrganizationDto request)
    {
        var response = await _registrationService.RegisterOrganizationAsync(request);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet(ApiRoutes.Organization.GetAll)]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrganizationDto>>>> GetAll()
    {
        var response = await _organizationService.GetAllAsync();
        return Ok(response);
    }
}

