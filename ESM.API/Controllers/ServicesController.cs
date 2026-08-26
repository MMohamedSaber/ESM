using ESM.Application.Common.Models;
using ESM.Application.DTOs;
using ESM.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESM.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _serviceService;

    public ServicesController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var services = await _serviceService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<ServiceDto>>.SuccessResponse(services));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var service = await _serviceService.GetByIdAsync(id);
        if (service == null)
            return NotFound(ApiResponse<object>.ErrorResponse("NOT_FOUND", "Service not found."));

        return Ok(ApiResponse<ServiceDto>.SuccessResponse(service));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.ErrorResponse("VALIDATION_ERROR", "Invalid request."));

        var created = await _serviceService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<ServiceDto>.SuccessResponse(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateServiceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.ErrorResponse("VALIDATION_ERROR", "Invalid request."));

        var existing = await _serviceService.GetByIdAsync(id);
        if (existing == null)
            return NotFound(ApiResponse<object>.ErrorResponse("NOT_FOUND", "Service not found."));

        await _serviceService.UpdateAsync(id, dto);
        return Ok(ApiResponse.SuccessResponse());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _serviceService.GetByIdAsync(id);
        if (existing == null)
            return NotFound(ApiResponse<object>.ErrorResponse("NOT_FOUND", "Service not found."));

        await _serviceService.DeleteAsync(id);
        return Ok(ApiResponse.SuccessResponse());
    }
}
