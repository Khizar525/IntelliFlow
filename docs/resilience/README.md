# Resilience Patterns

## Overview

IntelliFlow implements resilience patterns using Polly to handle transient failures in distributed systems. These patterns ensure the system remains operational even when downstream services experience temporary issues.

## Patterns Implemented

### 1. Retry Policy

**Purpose:** Automatically retry failed requests that may succeed on subsequent attempts.

**Configuration:**
- Retry count: 3
- Sleep duration: Exponential backoff (2s, 4s, 8s)
- Handles: Transient HTTP errors (5xx, timeout)

**Why Exists:**
- Network glitches are common in distributed systems
- Temporary service overload can cause transient failures
- Database connections may briefly drop

**Implementation:**
```csharp
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (outcome, timespan, retryCount, context) =>
        {
            Console.WriteLine($"Retry {retryCount} after {timespan.TotalSeconds}s");
        });
```

**Failure Scenarios:**
- Network timeout
- Service temporarily overloaded
- Database connection pool exhausted

**Recovery Behavior:**
- Waits exponentially increasing time between retries
- Logs each retry attempt
- Fails after 3 attempts

---

### 2. Circuit Breaker Policy

**Purpose:** Prevent cascading failures by stopping requests to a failing service.

**Configuration:**
- Failure threshold: 5 consecutive failures
- Break duration: 30 seconds
- Half-open state: Allows one test request after break

**Why Exists:**
- Prevents wasting resources on failing services
- Allows failing services time to recover
- Prevents cascading failures across the system

**Implementation:**
```csharp
var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5,
        durationOfBreak: TimeSpan.FromSeconds(30),
        onBreak: (result, breakDelay) =>
        {
            Console.WriteLine($"Circuit broken for {breakDelay.TotalSeconds}s");
        },
        onReset: () =>
        {
            Console.WriteLine("Circuit reset");
        });
```

**States:**
1. **Closed:** Normal operation, requests flow through
2. **Open:** Circuit tripped, requests fail immediately
3. **Half-Open:** Testing if service recovered

**Failure Scenarios:**
- Service completely down
- Database connection lost
- External API unavailable

**Recovery Behavior:**
- After 5 failures, circuit opens for 30 seconds
- After 30 seconds, allows one test request
- If test succeeds, circuit closes
- If test fails, circuit opens again

---

### 3. Timeout Policy

**Purpose:** Prevent requests from hanging indefinitely.

**Configuration:**
- Timeout: 30 seconds

**Why Exists:**
- Prevents resource exhaustion from hung requests
- Ensures users receive timely responses
- Allows failover to fallback mechanisms

**Implementation:**
```csharp
var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromSeconds(30));
```

**Failure Scenarios:**
- Deadlock in downstream service
- Network partition
- Slow database query

**Recovery Behavior:**
- Throws TimeoutRejectedException after 30 seconds
- Request is cancelled
- Caller can handle timeout gracefully

---

## Policy Pipeline

Policies are combined in a pipeline:

```
Request → Timeout → Retry → Circuit Breaker → Service Call
```

**Execution Order:**
1. **Timeout (30s):** Ensures request doesn't hang
2. **Retry (3 attempts):** Retries transient failures
3. **Circuit Breaker:** Prevents cascading failures

```csharp
builder.Services.AddHttpClient("Research", c =>
{
    c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("RESEARCH_SERVICE_URL")!);
    c.Timeout = TimeSpan.FromSeconds(30);
})
.AddPolicyHandler(retryPolicy)
.AddPolicyHandler(circuitBreakerPolicy)
.AddPolicyHandler(timeoutPolicy);
```

---

## Fallback Behavior

When all resilience policies fail, the system has fallback strategies:

### ResearchSummarizer
- **Primary:** OpenRouter API (Gemma 4)
- **Fallback 1:** OpenRouter API (Llama 3.2)
- **Fallback 2:** OpenRouter API (Dolphin Mistral)
- **Last Resort:** Return topic with raw content

### Orchestrator
- **Primary:** Call downstream service
- **Fallback:** Return error response with retry guidance

---

## Monitoring

### Circuit Breaker Metrics
- `circuit_breaker_state`: Current state (Closed/Open/Half-Open)
- `circuit_breaker_failures`: Total failure count
- `circuit_breaker_breaks`: Total circuit breaks

### Retry Metrics
- `retry_count`: Number of retries performed
- `retry_successes`: Retries that succeeded
- `retry_failures`: Retries that failed

### Timeout Metrics
- `timeout_count`: Number of timeouts
- `timeout_duration`: Timeout duration

---

## Testing Resilience

### Manual Testing
1. Stop a downstream service: `docker compose stop reporter`
2. Submit a task
3. Observe retry attempts in logs
4. Circuit breaker opens after 5 failures
5. Restart service
6. Circuit breaker recovers

### Automated Testing
Use MockHttpMessageHandler to simulate failures:
```csharp
var mockHandler = new MockHttpMessageHandler();
mockHandler.When("/api/research")
    .Respond(HttpStatusCode.InternalServerError);

// Verify retry behavior
```

---

## Best Practices

1. **Idempotency:** Ensure retries don't cause duplicate operations
2. **Exponential Backoff:** Prevent overwhelming failing services
3. **Circuit Breaker Tuning:** Adjust thresholds based on service characteristics
4. **Timeout Configuration:** Set realistic timeouts based on SLAs
5. **Fallback Strategy:** Always have a graceful degradation path

---

**Last Updated:** June 2026  
**Author:** M. Khizar Akram
