# Testing

## Overview

IntelliFlow includes comprehensive unit tests covering authentication, validation, orchestration logic, and resilience patterns. Tests are written using xUnit and FluentAssertions.

## Test Structure

```
tests/
└── Orchestrator.Tests/
    ├── AuthenticationTests.cs      # JWT authentication logic tests
    ├── TaskValidationTests.cs      # Input validation tests
    ├── OrchestratorLogicTests.cs   # Pipeline logic and state tests
    └── ResilienceTests.cs          # Retry, circuit breaker, timeout tests
```

## Running Tests

### Prerequisites
- .NET 8 SDK

### Commands
```bash
# Run all tests
dotnet test tests/Orchestrator.Tests/

# Run with verbosity
dotnet test tests/Orchestrator.Tests/ --verbosity normal

# Run specific test category
dotnet test tests/Orchestrator.Tests/ --filter "FullyQualifiedName~AuthenticationTests"

# Generate coverage report
dotnet test tests/Orchestrator.Tests/ /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

## Test Categories

### 1. Authentication Tests

Tests for JWT authentication and login validation.

| Test | Description |
|------|-------------|
| `JwtSecret_WhenNotSet_ShouldThrowException` | Validates JWT_SECRET is required |
| `JwtSecret_WhenSet_ShouldNotThrow` | Validates proper JWT setup |
| `LoginRequest_WithValidCredentials_ShouldPassValidation` | Valid login request passes |
| `LoginRequest_WithEmptyUsername_ShouldFailValidation` | Empty username rejected |
| `LoginRequest_WithEmptyPassword_ShouldFailValidation` | Empty password rejected |
| `LoginRequest_WithWrongPassword_ShouldFailAuthentication` | Wrong password fails auth |
| `LoginRequest_WithWrongUsername_ShouldFailAuthentication` | Wrong username fails auth |

### 2. Task Validation Tests

Tests for request input validation.

| Test | Description |
|------|-------------|
| `TaskRequest_WithValidData_ShouldPassValidation` | Valid request passes |
| `TaskRequest_WithEmptyTopic_ShouldFailValidation` | Empty topic rejected |
| `TaskRequest_WithWhitespaceTopic_ShouldFailValidation` | Whitespace topic rejected |
| `TaskRequest_WithEmptyEmail_ShouldFailValidation` | Empty email rejected |
| `TaskRequest_WithInvalidEmailFormat_ShouldFailValidation` | Invalid email format rejected |
| `TaskRequest_WithVeryLongTopic_ShouldPassValidation` | Long topic accepted |
| `TaskRequest_WithSpecialCharactersInTopic_ShouldPassValidation` | Special characters accepted |

### 3. Orchestrator Logic Tests

Tests for pipeline orchestration and state management.

| Test | Description |
|------|-------------|
| `GenerateTaskId_ShouldReturnUniqueIds` | Task IDs are unique |
| `GenerateTaskId_ShouldBeValidGuid` | Task IDs are valid GUIDs |
| `PipelineStatus_WhenStarted_ShouldBeProcessing` | Pipeline starts correctly |
| `PipelineStatus_WhenCompleted_ShouldBeSuccess` | Pipeline completes correctly |
| `PipelineStatus_WhenFailed_ShouldBeError` | Pipeline handles failures |
| `PipelineStatus_Duration_ShouldCalculateCorrectly` | Duration calculated properly |
| `TopicClassifier_TechnicalTopic_ShouldReturnTechnical` | Technical topics classified |
| `TopicClassifier_GeneralTopic_ShouldReturnGeneral` | General topics classified |
| `ModelSelector_TechnicalTopic_ShouldSelectTechnicalModel` | Model selection works |
| `ModelSelector_GeneralTopic_ShouldSelectGeneralModel` | Model selection works |

### 4. Resilience Tests

Tests for retry, circuit breaker, and timeout patterns.

| Test | Description |
|------|-------------|
| `RetryPolicy_WhenSuccess_ShouldNotRetry` | No retry on success |
| `RetryPolicy_WhenFailureThenSuccess_ShouldRetry` | Retries on transient failure |
| `RetryPolicy_WhenAllAttemptsFail_ShouldThrow` | Throws after max retries |
| `CircuitBreaker_WhenClosed_ShouldAllowRequests` | Closed state allows requests |
| `CircuitBreaker_WhenThresholdReached_ShouldOpen` | Opens after threshold |
| `CircuitBreaker_WhenOpen_ShouldRejectRequests` | Open state rejects requests |
| `CircuitBreaker_WhenTimeoutExpires_ShouldMoveToHalfOpen` | Recovers to half-open |
| `TimeoutPolicy_WhenSlow_ShouldTimeout` | Timeout on slow operations |
| `TimeoutPolicy_WhenFast_ShouldSucceed` | Fast operations succeed |

## Test Helpers

### Test Models

The tests define local test models that mirror production models:

```csharp
// Authentication test models
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// Task validation models
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
```

### Resilience Test Helpers

```csharp
// Circuit breaker test implementation
public class TestCircuitBreaker
{
    public string State { get; }  // Closed, Open, HalfOpen
    public T Execute<T>(Func<T> action);
    public bool TryExecute<T>(Func<T> action);
}

// Timeout test implementation
public class TestTimeoutPolicy
{
    public T Execute<T>(Func<T> action);
}
```

## Test Coverage

| Category | Tests | Coverage Target |
|----------|-------|-----------------|
| Authentication | 7 | 100% of auth logic |
| Task Validation | 7 | 100% of validation logic |
| Orchestration | 10 | 90% of pipeline logic |
| Resilience | 9 | 100% of resilience patterns |
| **Total** | **33** | **High coverage** |

## Best Practices

### 1. Arrange-Act-Assert Pattern
Every test follows the AAA pattern:
```csharp
[Fact]
public void Test_ShouldExpectedBehavior()
{
    // Arrange - Setup test data
    var request = new TaskRequest { Topic = "Test" };
    
    // Act - Execute the method
    var result = ValidateTaskRequest(request);
    
    // Assert - Verify the result
    result.IsValid.Should().BeTrue();
}
```

### 2. Descriptive Test Names
Test names follow the pattern: `Method_Scenario_ExpectedResult`

### 3. Isolated Tests
Each test is independent and doesn't depend on other tests.

### 4. Fast Execution
Tests complete quickly without external dependencies.

## Adding New Tests

1. Create a new test class in `tests/Orchestrator.Tests/`
2. Use the `[Fact]` attribute for test methods
3. Follow the AAA pattern
4. Use FluentAssertions for assertions
5. Keep tests focused on single behavior

## CI/CD Integration

Tests run automatically in the GitHub Actions CI/CD pipeline:

```yaml
- name: Test
  run: dotnet test tests/Orchestrator.Tests/ --no-build --verbosity normal
```

---

**Last Updated:** June 2026  
**Author:** M. Khizar Akram
