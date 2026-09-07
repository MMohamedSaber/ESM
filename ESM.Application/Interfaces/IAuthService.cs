using ESM.Application.Common.Models;
using ESM.Application.DTOs.Auth;

namespace ESM.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<ApiResponse<UserProfileDto>> GetCurrentUserAsync(string userId);
    Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordRequestDto request);
}
