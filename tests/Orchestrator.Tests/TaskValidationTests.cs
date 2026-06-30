using FluentAssertions;
using Xunit;

namespace Orchestrator.Tests;

/// <summary>
/// Tests for task request validation.
/// </summary>
public class TaskValidationTests
{
    [Fact]
    public void TaskRequest_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new TaskRequest
        {
            Topic = "Cloud Computing",
            NotifyEmail = "test@example.com"
        };

        // Act
        var validationResult = ValidateTaskRequest(request);

        // Assert
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void TaskRequest_WithEmptyTopic_ShouldFailValidation()
    {
        // Arrange
        var request = new TaskRequest
        {
            Topic = "",
            NotifyEmail = "test@example.com"
        };

        // Act
        var validationResult = ValidateTaskRequest(request);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.Contains("Topic"));
    }

    [Fact]
    public void TaskRequest_WithWhitespaceTopic_ShouldFailValidation()
    {
        // Arrange
        var request = new TaskRequest
        {
            Topic = "   ",
            NotifyEmail = "test@example.com"
        };

        // Act
        var validationResult = ValidateTaskRequest(request);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.Contains("Topic"));
    }

    [Fact]
    public void TaskRequest_WithEmptyEmail_ShouldFailValidation()
    {
        // Arrange
        var request = new TaskRequest
        {
            Topic = "Cloud Computing",
            NotifyEmail = ""
        };

        // Act
        var validationResult = ValidateTaskRequest(request);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.Contains("Email"));
    }

    [Fact]
    public void TaskRequest_WithInvalidEmailFormat_ShouldFailValidation()
    {
        // Arrange
        var request = new TaskRequest
        {
            Topic = "Cloud Computing",
            NotifyEmail = "not-an-email"
        };

        // Act
        var validationResult = ValidateTaskRequest(request);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.Contains("Email"));
    }

    [Fact]
    public void TaskRequest_WithVeryLongTopic_ShouldPassValidation()
    {
        // Arrange
        var request = new TaskRequest
        {
            Topic = new string('A', 1000),
            NotifyEmail = "test@example.com"
        };

        // Act
        var validationResult = ValidateTaskRequest(request);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public void TaskRequest_WithSpecialCharactersInTopic_ShouldPassValidation()
    {
        // Arrange
        var request = new TaskRequest
        {
            Topic = "AI/ML & Deep Learning: A Comprehensive Guide",
            NotifyEmail = "test@example.com"
        };

        // Act
        var validationResult = ValidateTaskRequest(request);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    // Helper method
    private static ValidationResult ValidateTaskRequest(TaskRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Topic))
        {
            errors.Add("Topic is required.");
        }

        if (string.IsNullOrWhiteSpace(request.NotifyEmail))
        {
            errors.Add("Email is required.");
        }
        else if (!request.NotifyEmail.Contains("@") || !request.NotifyEmail.Contains("."))
        {
            errors.Add("Invalid email format.");
        }

        return new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }
}

public class TaskRequest
{
    public string Topic { get; set; } = string.Empty;
    public string NotifyEmail { get; set; } = string.Empty;
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}
