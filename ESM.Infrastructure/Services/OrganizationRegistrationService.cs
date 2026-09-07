using ESM.Application.Common.Constants;
using ESM.Application.Common.Models;
using ESM.Application.DTOs.Organization;
using ESM.Application.Interfaces;
using ESM.Domain.Entities;
using ESM.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ESM.Infrastructure.Services;

public class OrganizationRegistrationService : IOrganizationRegistrationService
{
    private readonly EsmDbContext _context;
    private readonly UserManager<User> _userManager;

    public OrganizationRegistrationService(EsmDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<ApiResponse<bool>> RegisterOrganizationAsync(RegisterOrganizationDto request)
    {
        // Check if admin user already exists across the entire system
        var existingUser = await _userManager.FindByEmailAsync(request.AdminEmail);
        if (existingUser != null)
        {
            return ApiResponse<bool>.ErrorResponse("EmailAlreadyExists", "A user with this email address already exists.");
        }

        // Use EF Core Execution Strategy for resilience (especially if SQL Server is configured with retries)
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Create Organization
                var organization = new Organization
                {
                    NameEn = request.OrganizationNameEn,
                    NameAr = request.OrganizationNameAr
                };
                
                _context.Organizations.Add(organization);
                await _context.SaveChangesAsync(); // We need the Organization ID for the User

                // 2. Create Admin User
                var adminUser = new User
                {
                    UserName = request.AdminEmail,
                    Email = request.AdminEmail,
                    Name = request.AdminName,
                    OrganizationId = organization.Id
                };

                var userResult = await _userManager.CreateAsync(adminUser, request.AdminPassword);
                if (!userResult.Succeeded)
                {
                    var errors = string.Join(" ", userResult.Errors.Select(e => e.Description));
                    await transaction.RollbackAsync();
                    return ApiResponse<bool>.ErrorResponse("UserCreationFailed", errors);
                }

                // 3. Assign Role
                var roleResult = await _userManager.AddToRoleAsync(adminUser, Roles.Admin);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(" ", roleResult.Errors.Select(e => e.Description));
                    await transaction.RollbackAsync();
                    return ApiResponse<bool>.ErrorResponse("RoleAssignmentFailed", errors);
                }

                await transaction.CommitAsync();
                return ApiResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<bool>.ErrorResponse("RegistrationFailed", "An unexpected error occurred during registration: " + ex.Message);
            }
        });
    }
}
