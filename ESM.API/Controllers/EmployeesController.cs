    using ESM.Application.Common.Models;
using ESM.Application.DTOs.Employee;
using ESM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> Create([FromBody] CreateEmployeeDto dto)
    {
        var response = await _employeeService.CreateAsync(dto);
        if (response.Success)
            return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);

        return BadRequest(response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeDto>>>> GetAll()
    {
        var response = await _employeeService.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetById(Guid id)
    {
        var response = await _employeeService.GetByIdAsync(id);
        if (response.Success)
            return Ok(response);

        return NotFound(response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> Update(Guid id, [FromBody] UpdateEmployeeDto dto)
    {
        var response = await _employeeService.UpdateAsync(id, dto);
        if (response.Success)
            return Ok(response);

        return BadRequest(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var response = await _employeeService.DeleteAsync(id);
        if (response.Success)
            return Ok(response);

        return NotFound(response);
    }
}
