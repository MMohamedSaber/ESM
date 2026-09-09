using ESM.Application.Common.Models;
using ESM.Application.DTOs.Employee;
using ESM.Application.Interfaces;
using ESM.Domain.Entities;
using ESM.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ESM.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEsmDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<User> _userManager;

    public EmployeeService(IEsmDbContext context, ICurrentUserService currentUserService, UserManager<User> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    private IQueryable<Employee> GetQueryable()
    {
        var query = _context.Employees.AsQueryable();

        if (!_currentUserService.IsSuperAdmin)
        {
            var orgId = _currentUserService.OrganizationId;
            query = query.Where(e => e.Department.OrganizationId == orgId);
        }

        return query;
    }

    public async Task<ApiResponse<EmployeeDto>> CreateAsync(CreateEmployeeDto dto)
    {
        var dept = await _context.Departments.FindAsync(dto.DepartmentId);
        if (dept == null)
            return ApiResponse<EmployeeDto>.ErrorResponse("DepartmentNotFound", "The specified department does not exist.");

        if (!_currentUserService.IsSuperAdmin && dept.OrganizationId != _currentUserService.OrganizationId)
            return ApiResponse<EmployeeDto>.ErrorResponse("Unauthorized", "Department does not belong to your organization.");

        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return ApiResponse<EmployeeDto>.ErrorResponse("DuplicateUser", "A user with this email already exists.");

        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            Name = dto.Name,
            OrganizationId = dept.OrganizationId
        };

        var userResult = await _userManager.CreateAsync(user, dto.Password);
        if (!userResult.Succeeded)
        {
            var errors = string.Join(" ", userResult.Errors.Select(e => e.Description));
            return ApiResponse<EmployeeDto>.ErrorResponse("UserCreationFailed", errors);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, ESM.Application.Common.Constants.Roles.Employee);
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(" ", roleResult.Errors.Select(e => e.Description));
            return ApiResponse<EmployeeDto>.ErrorResponse("RoleAssignmentFailed", errors);
        }

        var userIdString = user.Id;

        // Auto-generate employee number: find the highest existing sequence and increment
        var lastNumber = await _context.Employees
            .Where(e => e.EmployeeNumber.StartsWith("EMP-"))
            .Select(e => e.EmployeeNumber)
            .ToListAsync();

        var maxSeq = lastNumber
            .Select(n => int.TryParse(n.Replace("EMP-", ""), out var num) ? num : 0)
            .DefaultIfEmpty(0)
            .Max();

        var employeeNumber = $"EMP-{(maxSeq + 1):D4}";

        var employee = new Employee
        {
            UserId = userIdString,
            EmployeeNumber = employeeNumber,
            DepartmentId = dto.DepartmentId,
            PositionId = dto.PositionId,
            HireDate = DateTime.UtcNow,
            Salary = dto.Salary,
            Status = EmployeeStatus.Active
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return ApiResponse<EmployeeDto>.SuccessResponse(MapToDto(employee));
    }

    public async Task<ApiResponse<EmployeeDto>> GetByIdAsync(Guid id)
    {
        var employee = await GetQueryable().FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null)
            return ApiResponse<EmployeeDto>.ErrorResponse("NotFound", "Employee not found.");

        return ApiResponse<EmployeeDto>.SuccessResponse(MapToDto(employee));
    }

    public async Task<ApiResponse<IEnumerable<EmployeeDto>>> GetAllAsync()
    {
        var employees = await GetQueryable().ToListAsync();
        var dtos = employees.Select(MapToDto).ToList();
        return ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<EmployeeDto>> UpdateAsync(Guid id, UpdateEmployeeDto dto)
    {
        var employee = await GetQueryable().FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null)
            return ApiResponse<EmployeeDto>.ErrorResponse("NotFound", "Employee not found.");

        var dept = await _context.Departments.FindAsync(dto.DepartmentId);
        if (dept == null)
            return ApiResponse<EmployeeDto>.ErrorResponse("DepartmentNotFound", "The specified department does not exist.");

        // Check if the department belongs to the same org as the employee (indirectly via current user if not super admin, but even super admin shouldn't mix orgs)
        var currentOrgId = await _context.Departments.Where(d => d.Id == employee.DepartmentId).Select(d => d.OrganizationId).FirstOrDefaultAsync();
        if (dept.OrganizationId != currentOrgId)
             return ApiResponse<EmployeeDto>.ErrorResponse("OrganizationMismatch", "The new department does not belong to the employee's organization.");

        employee.DepartmentId = dto.DepartmentId;
        employee.PositionId = dto.PositionId;
        employee.Salary = dto.Salary;
        employee.Status = dto.Status;

        await _context.SaveChangesAsync();

        return ApiResponse<EmployeeDto>.SuccessResponse(MapToDto(employee));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var employee = await GetQueryable().FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null)
            return ApiResponse<bool>.ErrorResponse("NotFound", "Employee not found.");

        employee.Status = EmployeeStatus.Inactive;
        employee.IsDeleted = true;
        employee.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true);
    }

    private static EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            UserId = employee.UserId,
            EmployeeNumber = employee.EmployeeNumber,
            DepartmentId = employee.DepartmentId,
            PositionId = employee.PositionId,
            HireDate = employee.HireDate,
            Salary = employee.Salary,
            Status = employee.Status
        };
    }
}
