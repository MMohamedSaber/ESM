using ESM.Application.Common.Models;
using ESM.Application.DTOs;
using ESM.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESM.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ServiceCategoriesController : ControllerBase
{
    private readonly IServiceCategoryService _serviceCategoryService;

    public ServiceCategoriesController(IServiceCategoryService serviceCategoryService)
    {
        _serviceCategoryService = serviceCategoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _serviceCategoryService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<ServiceCategoryDto>>.SuccessResponse(categories));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _serviceCategoryService.GetByIdAsync(id);
        if (category == null)
            return NotFound(ApiResponse<object>.ErrorResponse("NOT_FOUND", "Service category not found."));

        return Ok(ApiResponse<ServiceCategoryDto>.SuccessResponse(category));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.ErrorResponse("VALIDATION_ERROR", "Invalid request."));

        var created = await _serviceCategoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<ServiceCategoryDto>.SuccessResponse(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateServiceCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.ErrorResponse("VALIDATION_ERROR", "Invalid request."));

        var existing = await _serviceCategoryService.GetByIdAsync(id);
        if (existing == null)
            return NotFound(ApiResponse<object>.ErrorResponse("NOT_FOUND", "Service category not found."));

        await _serviceCategoryService.UpdateAsync(id, dto);
        return Ok(ApiResponse.SuccessResponse());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _serviceCategoryService.GetByIdAsync(id);
        if (existing == null)
            return NotFound(ApiResponse<object>.ErrorResponse("NOT_FOUND", "Service category not found."));

        await _serviceCategoryService.DeleteAsync(id);
        return Ok(ApiResponse.SuccessResponse());
    }
}
