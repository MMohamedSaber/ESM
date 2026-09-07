using System.Security.Claims;
using ESM.API.Services;
using ESM.Application.Common.Constants;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace ESM.Tests.API.Services;

public class CurrentUserServiceTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly CurrentUserService _currentUserService;

    public CurrentUserServiceTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _currentUserService = new CurrentUserService(_httpContextAccessorMock.Object);
    }

    [Fact]
    public void UserId_ShouldReturnNameIdentifierClaim_WhenPresent()
    {
        // Arrange
        var expectedUserId = Guid.NewGuid().ToString();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, expectedUserId)
        };
        SetupHttpContext(claims);

        // Act
        var userId = _currentUserService.UserId;

        // Assert
        userId.Should().Be(expectedUserId);
    }

    [Fact]
    public void UserId_ShouldReturnSubClaim_WhenNameIdentifierIsMissing()
    {
        // Arrange
        var expectedUserId = Guid.NewGuid().ToString();
        var claims = new List<Claim>
        {
            new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, expectedUserId)
        };
        SetupHttpContext(claims);

        // Act
        var userId = _currentUserService.UserId;

        // Assert
        userId.Should().Be(expectedUserId);
    }

    [Fact]
    public void UserId_ShouldReturnNull_WhenNoIdClaimsArePresent()
    {
        // Arrange
        SetupHttpContext(new List<Claim>());

        // Act
        var userId = _currentUserService.UserId;

        // Assert
        userId.Should().BeNull();
    }

    [Fact]
    public void OrganizationId_ShouldReturnGuid_WhenClaimIsPresentAndValid()
    {
        // Arrange
        var expectedOrgId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new Claim("OrganizationId", expectedOrgId.ToString())
        };
        SetupHttpContext(claims);

        // Act
        var orgId = _currentUserService.OrganizationId;

        // Assert
        orgId.Should().Be(expectedOrgId);
    }

    [Fact]
    public void OrganizationId_ShouldReturnNull_WhenClaimIsMissing()
    {
        // Arrange
        SetupHttpContext(new List<Claim>());

        // Act
        var orgId = _currentUserService.OrganizationId;

        // Assert
        orgId.Should().BeNull();
    }

    [Fact]
    public void OrganizationId_ShouldReturnNull_WhenClaimIsInvalidGuid()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("OrganizationId", "invalid-guid-value")
        };
        SetupHttpContext(claims);

        // Act
        var orgId = _currentUserService.OrganizationId;

        // Assert
        orgId.Should().BeNull();
    }

    [Fact]
    public void Roles_ShouldReturnAllRoleClaims()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, Roles.Admin),
            new Claim(ClaimTypes.Role, Roles.Employee)
        };
        SetupHttpContext(claims);

        // Act
        var roles = _currentUserService.Roles;

        // Assert
        roles.Should().Contain(new[] { Roles.Admin, Roles.Employee });
        roles.Should().HaveCount(2);
    }

    [Fact]
    public void Roles_ShouldReturnEmptyList_WhenNoRoleClaimsPresent()
    {
        // Arrange
        SetupHttpContext(new List<Claim>());

        // Act
        var roles = _currentUserService.Roles;

        // Assert
        roles.Should().BeEmpty();
    }

    [Fact]
    public void IsSuperAdmin_ShouldReturnTrue_WhenSuperAdminRoleClaimIsPresent()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, Roles.SuperAdmin)
        };
        SetupHttpContext(claims);

        // Act
        var isSuperAdmin = _currentUserService.IsSuperAdmin;

        // Assert
        isSuperAdmin.Should().BeTrue();
    }

    [Fact]
    public void IsSuperAdmin_ShouldReturnFalse_WhenSuperAdminRoleClaimIsMissing()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, Roles.Admin)
        };
        SetupHttpContext(claims);

        // Act
        var isSuperAdmin = _currentUserService.IsSuperAdmin;

        // Assert
        isSuperAdmin.Should().BeFalse();
    }

    private void SetupHttpContext(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
    }
}
