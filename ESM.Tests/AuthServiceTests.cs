using System.IdentityModel.Tokens.Jwt;
using ESM.Application.DTOs.Auth;
using ESM.Application.Validators.Auth;
using ESM.Domain.Entities;
using ESM.Infrastructure.Configuration;
using ESM.Infrastructure.Data;
using ESM.Infrastructure.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace ESM.Tests.Infrastructure.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly EsmDbContext _context;
    private readonly IOptions<JwtSettings> _jwtSettings;

    public AuthServiceTests()
    {
        var store = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var options = new DbContextOptionsBuilder<EsmDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new EsmDbContext(options);

        _jwtSettings = Options.Create(new JwtSettings
        {
            Key = "SuperSecretKeyThatIsAtLeast32BytesLong!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            AccessTokenExpirationMinutes = 60,
            RefreshTokenExpirationDays = 7
        });
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccessAndTokens()
    {
        // Arrange
        var user = new User { Id = "1", Email = "test@example.com", OrganizationId = Guid.NewGuid() };
        var request = new LoginRequestDto { Email = "test@example.com", Password = "Password123" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, request.Password)).ReturnsAsync(true);
        _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });

        var authService = new AuthService(_userManagerMock.Object, _context, _jwtSettings);

        // Act
        var response = await authService.LoginAsync(request);

        // Assert
        response.Success.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data!.AccessToken.Should().NotBeNullOrEmpty();
        response.Data.RefreshToken.Should().NotBeNullOrEmpty();

        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync();
        storedToken.Should().NotBeNull();
        var expectedHash = HashToken(response.Data.RefreshToken);
        storedToken!.Token.Should().Be(expectedHash);
        storedToken.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task LoginAsync_UnknownEmail_ReturnsError()
    {
        // Arrange
        var request = new LoginRequestDto { Email = "unknown@example.com", Password = "Password123" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync((User?)null);

        var authService = new AuthService(_userManagerMock.Object, _context, _jwtSettings);

        // Act
        var response = await authService.LoginAsync(request);

        // Assert
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();
        response.Error!.Code.Should().Be("InvalidCredentials");
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsError()
    {
        // Arrange
        var user = new User { Id = "1", Email = "test@example.com" };
        var request = new LoginRequestDto { Email = "test@example.com", Password = "WrongPassword" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, request.Password)).ReturnsAsync(false);

        var authService = new AuthService(_userManagerMock.Object, _context, _jwtSettings);

        // Act
        var response = await authService.LoginAsync(request);

        // Assert
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();
        response.Error!.Code.Should().Be("InvalidCredentials");
    }

    [Fact]
    public void Validator_InvalidEmail_ReturnsValidationError()
    {
        // Arrange
        var validator = new LoginRequestDtoValidator();
        var request = new LoginRequestDto { Email = "not-an-email", Password = "123" };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokensAndMarksOldAsUsed()
    {
        // Arrange
        var user = new User { Id = "1", Email = "test@example.com", OrganizationId = Guid.NewGuid() };
        var rawToken = "valid-refresh-token";
        var hashedToken = HashToken(rawToken);

        var existingTokenEntity = new RefreshToken
        {
            Token = hashedToken,
            UserId = user.Id,
            User = user,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(existingTokenEntity);
        await _context.SaveChangesAsync();

        _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });

        var authService = new AuthService(_userManagerMock.Object, _context, _jwtSettings);
        var request = new RefreshTokenRequestDto { RefreshToken = rawToken };

        // Act
        var response = await authService.RefreshTokenAsync(request);

        // Assert
        response.Success.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data!.AccessToken.Should().NotBeNullOrEmpty();
        response.Data.RefreshToken.Should().NotBeNullOrEmpty();
        response.Data.RefreshToken.Should().NotBe(rawToken); // Should be a new token

        // Check DB
        var tokensInDb = await _context.RefreshTokens.ToListAsync();
        tokensInDb.Should().HaveCount(2);

        var oldTokenInDb = tokensInDb.First(t => t.Id == existingTokenEntity.Id);
        oldTokenInDb.IsUsed.Should().BeTrue();

        var newTokenInDb = tokensInDb.First(t => t.Id != existingTokenEntity.Id);
        newTokenInDb.Token.Should().Be(HashToken(response.Data.RefreshToken));
        newTokenInDb.IsUsed.Should().BeFalse();
    }

    [Fact]
    public async Task RefreshTokenAsync_InvalidToken_ReturnsError()
    {
        // Arrange
        var authService = new AuthService(_userManagerMock.Object, _context, _jwtSettings);
        var request = new RefreshTokenRequestDto { RefreshToken = "invalid-token" };

        // Act
        var response = await authService.RefreshTokenAsync(request);

        // Assert
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();
        response.Error!.Code.Should().Be("InvalidToken");
    }

    [Fact]
    public async Task RefreshTokenAsync_ExpiredToken_ReturnsError()
    {
        // Arrange
        var rawToken = "expired-token";
        var hashedToken = HashToken(rawToken);

        var existingTokenEntity = new RefreshToken
        {
            Token = hashedToken,
            UserId = "1",
            ExpiryDate = DateTime.UtcNow.AddDays(-1), // Expired
            IsUsed = false,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(existingTokenEntity);
        await _context.SaveChangesAsync();

        var authService = new AuthService(_userManagerMock.Object, _context, _jwtSettings);
        var request = new RefreshTokenRequestDto { RefreshToken = rawToken };

        // Act
        var response = await authService.RefreshTokenAsync(request);

        // Assert
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();
        response.Error!.Code.Should().Be("InvalidToken");
    }

    [Fact]
    public async Task RefreshTokenAsync_UsedToken_ReturnsError()
    {
        // Arrange
        var rawToken = "used-token";
        var hashedToken = HashToken(rawToken);

        var existingTokenEntity = new RefreshToken
        {
            Token = hashedToken,
            UserId = "1",
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsUsed = true, // Used
            IsRevoked = false
        };

        _context.RefreshTokens.Add(existingTokenEntity);
        await _context.SaveChangesAsync();

        var authService = new AuthService(_userManagerMock.Object, _context, _jwtSettings);
        var request = new RefreshTokenRequestDto { RefreshToken = rawToken };

        // Act
        var response = await authService.RefreshTokenAsync(request);

        // Assert
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNull();
        response.Error!.Code.Should().Be("InvalidToken");
    }

    private static string HashToken(string token)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        var hash = System.Security.Cryptography.SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
