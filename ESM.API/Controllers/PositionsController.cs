using ESM.Application.Common.Models;
using ESM.Application.DTOs.Position;
using ESM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PositionsController : ControllerBase
{
    private readonly IPositionService _positionService;

    public PositionsController(IPositionService positionService)
    {
        _positionService = positionService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<PositionDto>>>> GetAll()
    {
        var response = await _positionService.GetAllAsync();
        return Ok(response);
    }
}
