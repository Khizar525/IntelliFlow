# System Architecture

## High-Level Architecture Diagram

![System Architecture](images/system-architecture.png)

### Service Communication Flow

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

![Sequence Diagram](images/sequence-diagram.png)

## Container Architecture

![Service Dependencies](images/service-dependencies.png)

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

![Resilience Patterns](images/resilience-patterns.png)

## Observability Architecture

![Observability Architecture](images/observability-architecture.png)

## Pipeline Flow

![Pipeline Flow](images/pipeline-flow.png)

---

**Last Updated:** June 2026  
**Author:** M. Khizar Akram
