using ESM.Application.Common.Constants;
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

    public OrganizationController(IOrganizationRegistrationService registrationService)
    {
        _registrationService = registrationService;
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
}
