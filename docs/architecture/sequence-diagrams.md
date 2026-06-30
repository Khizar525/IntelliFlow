# Sequence Diagrams

## Pipeline Execution Sequence

```mermaid
sequenceDiagram
    actor User
    participant Frontend as React Frontend
    participant Orchestrator as Orchestrator API
    participant Research as Research Service
    participant Summarizer as Summarizer Service
    participant Reporter as Reporter Service
    participant Notifier as Notifier Service
    participant OpenRouter as OpenRouter API
    participant Supabase as Supabase
    participant Blockchain as Ethereum Sepolia

    User->>Frontend: Submit research topic
    Frontend->>Orchestrator: POST /api/tasks {topic, email}
    
    Note over Orchestrator: Generate taskId<br/>Authenticate JWT<br/>Validate request

    rect rgb(230, 240, 255)
        Note over Orchestrator,Research: Stage 1-2: Research & Summarize
        Orchestrator->>Research: POST /api/research {taskId, topic}
        Research->>OpenRouter: Fetch topic data
        OpenRouter-->>Research: Raw content
        Research->>Summarizer: Summarize(rawContent, topic)
        Summarizer->>OpenRouter: LLM summarization
        OpenRouter-->>Summarizer: Summary text
        Summarizer-->>Research: Summary
        Research-->>Orchestrator: {summary}
    end

    rect rgb(240, 230, 255)
        Note over Orchestrator,Reporter: Stage 3: Report Generation
        Orchestrator->>Reporter: POST /api/report {taskId, topic, summary}
        Reporter->>Reporter: Format report content
        Reporter->>Reporter: Generate SHA-256 hash
        Reporter->>Supabase: Upload report file
        Supabase-->>Reporter: Blob URL
        Reporter->>Supabase: Save record to DB
        Reporter-->>Orchestrator: {blobUrl, outputHash}
    end

    rect rgb(230, 255, 230)
        Note over Orchestrator,Blockchain: Stage 4-5: Notify & Blockchain
        Orchestrator->>Notifier: POST /api/notify {taskId, email, reportUrl, outputHash}
        Notifier->>Notifier: Send email via SMTP
        Notifier->>Blockchain: Log hash on-chain
        Blockchain-->>Notifier: Transaction hash
        Notifier-->>Orchestrator: {txHash}
    end

    Orchestrator-->>Frontend: {taskId, status, reportUrl, txHash}
    Frontend-->>User: Display pipeline results
```

## Authentication Sequence

```mermaid
sequenceDiagram
    actor User
    participant Frontend as React Frontend
    participant Orchestrator as Orchestrator API

    User->>Frontend: Enter credentials
    Frontend->>Orchestrator: POST /api/auth/login {username, password}
    
    Note over Orchestrator: Validate credentials<br/>Check hardcoded demo user
    
    alt Valid Credentials
        Orchestrator->>Orchestrator: Generate JWT token
        Note over Orchestrator: Set expiry (8 hours)<br/>Sign with HMAC-SHA256
        Orchestrator-->>Frontend: {token, expiresIn, username}
        Frontend->>Frontend: Store token in localStorage
        Frontend-->>User: Redirect to dashboard
    else Invalid Credentials
        Orchestrator-->>Frontend: 401 Unauthorized
        Frontend-->>User: Show error message
    end
```

## Error Handling Sequence

```mermaid
sequenceDiagram
    participant Client
    participant Gateway as Orchestrator
    participant Service as Backend Service
    participant External as External API

    Client->>Gateway: Request
    
    alt Rate Limit Exceeded
        Gateway-->>Client: 429 Too Many Requests
    else Valid Request
        Gateway->>Service: Forward request
        
        alt Service Available
            Service->>External: API call
            
            alt External API Success
                External-->>Service: Response
                Service-->>Gateway: Result
                Gateway-->>Client: 200 OK
            else External API Failure
                External-->>Service: Error
                Service->>Service: Retry (3 attempts)
                
                alt Retries Exhausted
                    Service-->>Gateway: 503 Service Unavailable
                    Gateway-->>Client: 502 Bad Gateway
                else Recovery
                    External-->>Service: Response
                    Service-->>Gateway: Result
                    Gateway-->>Client: 200 OK
                end
            end
        else Service Down
            Service-->>Gateway: Connection refused
            Gateway-->>Client: 503 Service Unavailable
        end
    end
```

## Circuit Breaker Sequence

```mermaid
sequenceDiagram
    participant Client
    participant Gateway as Orchestrator
    participant CB as Circuit Breaker
    participant Service as Downstream Service

    Note over CB: Initial State: CLOSED

    Client->>Gateway: Request 1
    Gateway->>CB: Check circuit
    CB->>Service: Forward (success)
    Service-->>CB: 200 OK
    CB-->>Gateway: Success
    Gateway-->>Client: 200 OK

    Client->>Gateway: Request 2
    Gateway->>CB: Check circuit
    CB->>Service: Forward (fail)
    Service-->>CB: Error
    Note over CB: Failure count: 1

    Client->>Gateway: Request 3
    Gateway->>CB: Check circuit
    CB->>Service: Forward (fail)
    Service-->>CB: Error
    Note over CB: Failure count: 2

    Client->>Gateway: Request 4
    Gateway->>CB: Check circuit
    CB->>Service: Forward (fail)
    Service-->>CB: Error
    Note over CB: Failure count: 3
    Note over CB: State: OPEN

    Client->>Gateway: Request 5
    Gateway->>CB: Check circuit
    Note over CB: Circuit OPEN<br/>Reject immediately
    CB-->>Gateway: CircuitOpenException
    Gateway-->>Client: 503 Service Unavailable

    Note over CB: Wait 30 seconds...

    Client->>Gateway: Request 6
    Gateway->>CB: Check circuit
    Note over CB: State: HALF-OPEN
    CB->>Service: Forward (test)
    Service-->>CB: 200 OK
    Note over CB: State: CLOSED
    CB-->>Gateway: Success
    Gateway-->>Client: 200 OK
```

---

**Last Updated:** June 2026  
**Author:** M. Khizar Akram
