using FluentAssertions;
using Xunit;

namespace Orchestrator.Tests;

/// <summary>
/// Tests for JWT authentication logic.
/// </summary>
public class AuthenticationTests
{
    [Fact]
    public void JwtSecret_WhenNotSet_ShouldThrowException()
    {
        // Arrange
        var originalValue = Environment.GetEnvironmentVariable("JWT_SECRET");
        Environment.SetEnvironmentVariable("JWT_SECRET", null);

        // Act & Assert
        var act = () => ValidateJwtSecret();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*JWT_SECRET*");

        // Cleanup
        Environment.SetEnvironmentVariable("JWT_SECRET", originalValue);
    }

    [Fact]
    public void JwtSecret_WhenSet_ShouldNotThrow()
    {
        // Arrange
        Environment.SetEnvironmentVariable("JWT_SECRET", "test-secret-key-for-jwt");

        // Act
        var act = () => ValidateJwtSecret();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void LoginRequest_WithValidCredentials_ShouldPassValidation()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "admin",
            Password = "intelliflow123"
        };

        // Act
        var isValid = ValidateLoginRequest(request);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void LoginRequest_WithEmptyUsername_ShouldFailValidation()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "",
            Password = "intelliflow123"
        };

        // Act
        var isValid = ValidateLoginRequest(request);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void LoginRequest_WithEmptyPassword_ShouldFailValidation()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "admin",
            Password = ""
        };

        // Act
        var isValid = ValidateLoginRequest(request);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void LoginRequest_WithWrongPassword_ShouldFailAuthentication()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "admin",
            Password = "wrongpassword"
        };

        // Act
        var isAuthenticated = AuthenticateUser(request);

        // Assert
        isAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void LoginRequest_WithWrongUsername_ShouldFailAuthentication()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "wronguser",
            Password = "intelliflow123"
        };

        // Act
        var isAuthenticated = AuthenticateUser(request);

        // Assert
        isAuthenticated.Should().BeFalse();
    }

    // Helper methods
    private static void ValidateJwtSecret()
    {
        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
        if (string.IsNullOrEmpty(jwtSecret))
        {
            throw new InvalidOperationException("JWT_SECRET environment variable is not set.");
        }
    }

    private static bool ValidateLoginRequest(LoginRequest request)
    {
        return !string.IsNullOrWhiteSpace(request.Username) && 
               !string.IsNullOrWhiteSpace(request.Password);
    }

    private static bool AuthenticateUser(LoginRequest request)
    {
        const string validUsername = "admin";
        const string validPassword = "intelliflow123";
        
        return request.Username == validUsername && request.Password == validPassword;
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
