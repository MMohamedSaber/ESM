using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ESM.Application.Common.Models;
using ESM.Application.DTOs.Auth;
using ESM.Application.Interfaces;
using ESM.Domain.Entities;
using ESM.Infrastructure.Configuration;
using ESM.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace ESM.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly EsmDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthService(UserManager<User> userManager, EsmDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return ApiResponse<LoginResponseDto>.ErrorResponse("InvalidCredentials", "Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        
        var accessToken = GenerateAccessToken(user, roles);
        var refreshToken = GenerateRefreshToken();
        var hashedRefreshToken = HashToken(refreshToken);

        var refreshTokenEntity = new RefreshToken
        {
            Token = hashedRefreshToken,
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            IsRevoked = false,
            IsUsed = false
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return ApiResponse<LoginResponseDto>.SuccessResponse(new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiryDate = refreshTokenEntity.ExpiryDate
        });
    }

    public async Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var hashedToken = HashToken(request.RefreshToken);

        var refreshTokenEntity = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == hashedToken);

        if (refreshTokenEntity == null || !refreshTokenEntity.IsActive)
        {
            return ApiResponse<LoginResponseDto>.ErrorResponse("InvalidToken", "Refresh token is invalid or expired.");
        }

        var user = refreshTokenEntity.User;
        var roles = await _userManager.GetRolesAsync(user);

        refreshTokenEntity.IsUsed = true;

        var newAccessToken = GenerateAccessToken(user, roles);
        var newRefreshToken = GenerateRefreshToken();
        var newHashedRefreshToken = HashToken(newRefreshToken);

        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newHashedRefreshToken,
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            IsRevoked = false,
            IsUsed = false
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync();

        return ApiResponse<LoginResponseDto>.SuccessResponse(new LoginResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiryDate = newRefreshTokenEntity.ExpiryDate
        });
    }

    public async Task<ApiResponse<UserProfileDto>> GetCurrentUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ApiResponse<UserProfileDto>.ErrorResponse("UserNotFound", "The user could not be found.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var profile = new UserProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email ?? string.Empty,
            OrganizationId = user.OrganizationId,
            Roles = roles
        };

        return ApiResponse<UserProfileDto>.SuccessResponse(profile);
    }

    public async Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordRequestDto request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ApiResponse<bool>.ErrorResponse("UserNotFound", "The user could not be found.");
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            return ApiResponse<bool>.ErrorResponse("PasswordChangeFailed", errors);
        }

        return ApiResponse<bool>.SuccessResponse(true);
    }

    private string GenerateAccessToken(User user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (user.OrganizationId.HasValue)
        {
            claims.Add(new Claim("OrganizationId", user.OrganizationId.Value.ToString()));
        }

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private static string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
