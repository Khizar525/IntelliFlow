using FluentAssertions;
using Xunit;

namespace Orchestrator.Tests;

/// <summary>
/// Tests for resilience patterns (retry, circuit breaker, timeout).
/// </summary>
public class ResilienceTests
{
    [Fact]
    public void RetryPolicy_WhenSuccess_ShouldNotRetry()
    {
        // Arrange
        int attemptCount = 0;

        // Act
        var result = ExecuteWithRetry(() =>
        {
            attemptCount++;
            return "Success";
        }, maxRetries: 3);

        // Assert
        result.Should().Be("Success");
        attemptCount.Should().Be(1);
    }

    [Fact]
    public void RetryPolicy_WhenFailureThenSuccess_ShouldRetry()
    {
        // Arrange
        int attemptCount = 0;

        // Act
        var result = ExecuteWithRetry(() =>
        {
            attemptCount++;
            if (attemptCount < 3)
                throw new HttpRequestException("Transient error");
            return "Success";
        }, maxRetries: 3);

        // Assert
        result.Should().Be("Success");
        attemptCount.Should().Be(3);
    }

    [Fact]
    public void RetryPolicy_WhenAllAttemptsFail_ShouldThrow()
    {
        // Arrange
        int attemptCount = 0;

        // Act & Assert
        var act = () => ExecuteWithRetry<string>(() =>
        {
            attemptCount++;
            throw new HttpRequestException("Persistent error");
        }, maxRetries: 3);

        act.Should().Throw<HttpRequestException>();
        attemptCount.Should().Be(4); // 1 initial + 3 retries
    }

    [Fact]
    public void CircuitBreaker_WhenClosed_ShouldAllowRequests()
    {
        // Arrange
        var circuitBreaker = new TestCircuitBreaker(failureThreshold: 3);

        // Act
        var result = circuitBreaker.Execute(() => "Success");

        // Assert
        result.Should().Be("Success");
        circuitBreaker.State.Should().Be("Closed");
    }

    [Fact]
    public void CircuitBreaker_WhenThresholdReached_ShouldOpen()
    {
        // Arrange
        var circuitBreaker = new TestCircuitBreaker(failureThreshold: 3);

        // Act
        for (int i = 0; i < 3; i++)
        {
            try { circuitBreaker.Execute<string>(() => throw new Exception("Error")); }
            catch { /* Expected - recording failure */ }
        }

        // Assert
        circuitBreaker.State.Should().Be("Open");
    }

    [Fact]
    public void CircuitBreaker_WhenOpen_ShouldRejectRequests()
    {
        // Arrange
        var circuitBreaker = new TestCircuitBreaker(failureThreshold: 3);
        for (int i = 0; i < 3; i++)
        {
            try { circuitBreaker.Execute<string>(() => throw new Exception("Error")); }
            catch { /* Expected - recording failure */ }
        }

        // Act & Assert
        var act = () => circuitBreaker.Execute(() => "Success");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Circuit*open*");
    }

    [Fact]
    public void CircuitBreaker_WhenTimeoutExpires_ShouldMoveToHalfOpen()
    {
        // Arrange
        var circuitBreaker = new TestCircuitBreaker(
            failureThreshold: 3, 
            breakDuration: TimeSpan.FromMilliseconds(100));
        
        for (int i = 0; i < 3; i++)
        {
            try { circuitBreaker.Execute<string>(() => throw new Exception("Error")); }
            catch { /* Expected - recording failure */ }
        }

        // Act
        Thread.Sleep(150); // Wait for break duration
        circuitBreaker.TryExecute(() => "Success");

        // Assert
        circuitBreaker.State.Should().Be("Closed");
    }

    [Fact]
    public void TimeoutPolicy_WhenSlow_ShouldTimeout()
    {
        // Arrange
        var timeoutPolicy = new TestTimeoutPolicy(TimeSpan.FromMilliseconds(50));

        // Act & Assert
        var act = () => timeoutPolicy.Execute(() =>
        {
            Thread.Sleep(100); // Simulate slow operation
            return "Success";
        });

        act.Should().Throw<TimeoutException>();
    }

    [Fact]
    public void TimeoutPolicy_WhenFast_ShouldSucceed()
    {
        // Arrange
        var timeoutPolicy = new TestTimeoutPolicy(TimeSpan.FromMilliseconds(100));

        // Act
        var result = timeoutPolicy.Execute(() =>
        {
            Thread.Sleep(10); // Fast operation
            return "Success";
        });

        // Assert
        result.Should().Be("Success");
    }

    // Helper methods
    private static T ExecuteWithRetry<T>(Func<T> operation, int maxRetries)
    {
        int attempt = 0;
        while (true)
        {
            try
            {
                return operation();
            }
            catch (Exception) when (attempt < maxRetries)
            {
                attempt++;
                Thread.Sleep(10); // Simulate backoff
            }
        }
    }
}

public class TestCircuitBreaker
{
    private readonly int _failureThreshold;
    private readonly TimeSpan _breakDuration;
    private int _failureCount;
    private DateTime? _breakStartTime;

    public string State { get; private set; } = "Closed";

    public TestCircuitBreaker(int failureThreshold, TimeSpan? breakDuration = null)
    {
        _failureThreshold = failureThreshold;
        _breakDuration = breakDuration ?? TimeSpan.FromSeconds(30);
    }

    public T Execute<T>(Func<T> action)
    {
        if (State == "Open")
        {
            if (DateTime.UtcNow - _breakStartTime >= _breakDuration)
            {
                State = "HalfOpen";
            }
            else
            {
                throw new InvalidOperationException("Circuit is open");
            }
        }

        try
        {
            var result = action();
            
            if (State == "HalfOpen")
            {
                State = "Closed";
                _failureCount = 0;
            }
            
            return result;
        }
        catch (Exception)
        {
            _failureCount++;
            
            if (_failureCount >= _failureThreshold)
            {
                State = "Open";
                _breakStartTime = DateTime.UtcNow;
            }
            
            throw;
        }
    }

    public bool TryExecute<T>(Func<T> action)
    {
        try
        {
            Execute(action);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class TestTimeoutPolicy
{
    private readonly TimeSpan _timeout;

    public TestTimeoutPolicy(TimeSpan timeout)
    {
        _timeout = timeout;
    }

    public T Execute<T>(Func<T> action)
    {
        var task = Task.Run(action);
        
        if (task.Wait(_timeout))
        {
            return task.Result;
        }
        
        throw new TimeoutException("Operation timed out");
    }
}
