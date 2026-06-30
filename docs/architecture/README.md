# System Architecture

## High-Level Architecture Diagram

```mermaid
graph TB
    subgraph "Client Layer"
        UI[React Frontend<br/>Port 5173]
    end

    subgraph "API Gateway Layer"
        ORC[Orchestrator API<br/>Port 5000]
    end

    subgraph "Agent Services Layer"
        RS[Research Service<br/>Port 5001]
        SS[Summarizer Service<br/>Port 5001]
        RP[Reporter Service<br/>Port 5002]
        NT[Notifier Service<br/>Port 5003]
    end

    subgraph "External Services"
        OR[OpenRouter API<br/>LLM Provider]
        SB[Supabase<br/>PostgreSQL + Storage]
        SM[SMTP Server<br/>Email Delivery]
        ETH[Ethereum Sepolia<br/>Blockchain]
    end

    subgraph "Observability"
        JG[Jaeger<br/>Distributed Tracing]
    end

    UI -->|HTTP/REST| ORC
    ORC -->|HTTP| RS
    ORC -->|HTTP| RP
    ORC -->|HTTP| NT
    RS -->|HTTP| SS
    RS -->|HTTPS| OR
    RP -->|HTTPS| SB
    NT -->|SMTP| SM
    NT -->|RPC| ETH

    ORC -.->|Traces| JG
    RS -.->|Traces| JG
    RP -.->|Traces| JG
    NT -.->|Traces| JG

    style UI fill:#61DAFB,stroke:#333
    style ORC fill:#4CAF50,stroke:#333
    style RS fill:#FF9800,stroke:#333
    style SS fill:#FF9800,stroke:#333
    style RP fill:#9C27B0,stroke:#333
    style NT fill:#F44336,stroke:#333
    style OR fill:#00BCD4,stroke:#333
    style SB fill:#3ECF8E,stroke:#333
    style SM fill:#FF5722,stroke:#333
    style ETH fill:#627EEA,stroke:#333
    style JG fill:#FF6B6B,stroke:#333
```

## Service Communication Flow

```mermaid
graph LR
    subgraph "Pipeline Execution"
        A[User Request] --> B[Orchestrator]
        B --> C[Research]
        C --> D[Summarizer]
        D --> E[Reporter]
        E --> F[Notifier]
        F --> G[Blockchain]
    end

    subgraph "Data Flow"
        H[Topic] --> I[Raw Content]
        I --> J[Summary]
        J --> K[PDF Report]
        K --> L[Email + Hash]
        L --> M[On-Chain Record]
    end

    style A fill:#2196F3,stroke:#333
    style B fill:#4CAF50,stroke:#333
    style C fill:#FF9800,stroke:#333
    style D fill:#FF9800,stroke:#333
    style E fill:#9C27B0,stroke:#333
    style F fill:#F44336,stroke:#333
    style G fill:#627EEA,stroke:#333
```

## Sequence Diagram

```mermaid
sequenceDiagram
    participant U as User
    participant UI as React Frontend
    participant ORC as Orchestrator
    participant RS as Research Service
    participant SS as Summarizer Service
    participant RP as Reporter Service
    participant NT as Notifier Service
    participant OR as OpenRouter
    participant SB as Supabase
    participant ETH as Ethereum

    U->>UI: Submit Topic
    UI->>ORC: POST /api/tasks
    ORC->>ORC: Generate taskId
    
    rect rgb(255, 200, 200)
        ORC->>RS: POST /api/research
        RS->>OR: Fetch topic data
        OR-->>RS: Raw content
        RS->>SS: Summarize content
        SS->>OR: LLM summarization
        OR-->>SS: Summary
        SS-->>RS: Summary
        RS-->>ORC: {summary}
    end

    rect rgb(200, 200, 255)
        ORC->>RP: POST /api/report
        RP->>SB: Upload report
        SB-->>RP: Blob URL
        RP->>RP: Generate SHA-256 hash
        RP-->>ORC: {blobUrl, outputHash}
    end

    rect rgb(200, 255, 200)
        ORC->>NT: POST /api/notify
        NT->>NT: Send email
        NT->>ETH: Log hash
        ETH-->>NT: txHash
        NT-->>ORC: {txHash}
    end

    ORC-->>UI: {taskId, status, reportUrl, txHash}
    UI-->>U: Display Results
```

## Container Architecture

```mermaid
graph TB
    subgraph "Docker Network: intelliflow-network"
        subgraph "Frontend Container"
            FE[React App<br/>Vite Dev Server]
        end

        subgraph "Backend Containers"
            ORC[Orchestrator<br/>ASP.NET Core]
            RS[ResearchSummarizer<br/>ASP.NET Core]
            RP[Reporter<br/>ASP.NET Core]
            NT[Notifier<br/>ASP.NET Core]
        end

        subgraph "Observability Container"
            JG[Jaeger<br/>All-in-One]
        end
    end

    subgraph "External Services"
        OR[OpenRouter]
        SB[Supabase]
        SM[SMTP]
        ETH[Ethereum]
    end

    FE -->|5173| ORC
    ORC -->|5000| RS
    ORC -->|5000| RP
    ORC -->|5000| NT
    RS --> OR
    RP --> SB
    NT --> SM
    NT --> ETH

    ORC -.->|16686| JG
    RS -.->|16686| JG
    RP -.->|16686| JG
    NT -.->|16686| JG
```

## API Gateway Pattern

```mermaid
graph TB
    subgraph "Client"
        C[React Frontend]
    end

    subgraph "API Gateway (Orchestrator)"
        Auth[JWT Authentication]
        RateLimit[Rate Limiting]
        Route[Request Routing]
        Circuit[Circuit Breaker]
        Log[Request Logging]
    end

    subgraph "Backend Services"
        S1[Research]
        S2[Reporter]
        S3[Notifier]
    end

    C --> Auth
    Auth --> RateLimit
    RateLimit --> Route
    Route --> Circuit
    Circuit --> Log

    Log --> S1
    Log --> S2
    Log --> S3

    style Auth fill:#4CAF50,stroke:#333
    style RateLimit fill:#FF9800,stroke:#333
    style Route fill:#2196F3,stroke:#333
    style Circuit fill:#F44336,stroke:#333
    style Log fill:#9C27B0,stroke:#333
```

## Resilience Patterns

```mermaid
graph TB
    subgraph "Polly Resilience Pipeline"
        A[Request] --> B[Timeout Policy<br/>30 seconds]
        B --> C[Retry Policy<br/>3 attempts]
        C --> D[Circuit Breaker<br/>5 failures]
        D --> E[Service Call]
    end

    subgraph "Circuit Breaker States"
        F[Closed<br/>Normal] -->|5 failures| G[Open<br/>Blocked]
        G -->|30s timeout| H[Half-Open<br/>Testing]
        H -->|Success| F
        H -->|Failure| G
    end

    style F fill:#4CAF50,stroke:#333
    style G fill:#F44336,stroke:#333
    style H fill:#FF9800,stroke:#333
```

## Observability Architecture

```mermaid
graph TB
    subgraph "Application Services"
        ORC[Orchestrator]
        RS[Research]
        RP[Reporter]
        NT[Notifier]
    end

    subgraph "OpenTelemetry SDK"
        OT[OTel Tracer]
        SP[Span Processor]
        EX[OTLP Exporter]
    end

    subgraph "Jaeger Backend"
        AG[Jaeger Agent<br/>Port 6831]
        CO[Jaeger Collector<br/>Port 14268]
        QP[Jaeger Query<br/>Port 16686]
    end

    ORC --> OT
    RS --> OT
    RP --> OT
    NT --> OT

    OT --> SP
    SP --> EX
    EX -->|OTLP| AG
    AG --> CO
    CO --> QP

    style ORC fill:#4CAF50,stroke:#333
    style RS fill:#FF9800,stroke:#333
    style RP fill:#9C27B0,stroke:#333
    style NT fill:#F44336,stroke:#333
    style OT fill:#00BCD4,stroke:#333
    style JG fill:#FF6B6B,stroke:#333
```

---

**Last Updated:** June 2026  
**Author:** M. Khizar Akram
