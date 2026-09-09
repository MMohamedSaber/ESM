using ESM.Application.Common.Models;
using ESM.Application.DTOs.Department;
using ESM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentDto>>>> GetAll()
    {
        var response = await _departmentService.GetAllAsync();
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> Create([FromBody] CreateDepartmentDto dto)
    {
        var response = await _departmentService.CreateAsync(dto);
        if (response.Success)
            return CreatedAtAction(nameof(GetAll), response);

        return BadRequest(response);
    }
}
