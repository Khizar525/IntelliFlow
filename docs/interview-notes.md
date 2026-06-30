# IntelliFlow — Interview Preparation Notes

## Why IntelliFlow?

IntelliFlow started as a Cloud Computing capstone project, but I positioned it as a **cloud-native agent orchestration platform** because I wanted to build something that reflected real-world distributed systems. Most university projects stop at a single CRUD API. I wanted to show I could design, build, and ship a multi-service, event-driven system with production-grade observability, resilience, and CI/CD — all running in containers.

It gave me a chance to explore:
- How microservices discover and communicate with each other
- How to make distributed systems observable (tracing, metrics)
- How to make them resilient (retries, circuit breakers, timeouts)
- How to organize a team codebase with multiple module owners
- How to automate testing and deployment

## Why Microservices?

For this project, microservices were over-engineering — and I knew that. But the learning objective was **Cloud Computing**, and microservices let me demonstrate:

- **Service decomposition**: Splitting a monolithic pipeline into Orchestrator, Research Summarizer, Reporter, and Notifier — each with its own bounded context
- **Containerization**: Each service has its own Dockerfile with independent build stages
- **Orchestration**: Docker Compose with health checks, dependency ordering, and network isolation
- **Independent deployability**: Each service can be built, tested, and deployed independently
- **Team simulation**: Each module assigned to a different team member with defined ownership

The takeaway is: I understand the tradeoffs. I can justify when microservices are appropriate and when they're not. For a portfolio project, the complexity itself demonstrates breadth.

## Why OpenTelemetry?

Distributed tracing is table stakes for production microservices. Without it, debugging a request that spans 4 services becomes guesswork.

I chose **OpenTelemetry** because:
- It's the industry standard (CNCF graduated project)
- Vendor-neutral — you can export to Jaeger, Zipkin, Datadog, Grafana Tempo, etc.
- Single instrumentation library for traces, metrics, and logs
- Works across multiple languages (C# with the .NET SDK)

I configured:
- **AspNetCoreInstrumentation** — automatically captures HTTP request/response spans
- **HttpClientInstrumentation** — captures outbound HTTP calls between services
- **Custom ActivitySource** (`PipelineActivitySource`) — for manual instrumentation of pipeline operations
- **OTLP exporter** — sends traces to Jaeger's OTLP gRPC endpoint

The result: every request through the orchestrator generates a trace with spans showing service-to-service calls, including HTTP methods, status codes, and duration.

## Why Jaeger?

Jaeger was the pragmatic choice because:
- It's the most widely-adopted open-source tracing backend
- Single-container deployment (`jaegertracing/all-in-one`) — perfect for demo/development
- Built-in OTLP receiver (gRPC on port 4317, HTTP on 4318)
- UI at localhost:16686 for visualizing traces
- Badger storage for persistence without external dependencies

In production I'd use Grafana Tempo or a managed OTEL backend, but for a portfolio project Jaeger delivers maximum visibility with minimum operational overhead.

## Why Circuit Breakers?

In a synchronous microservice chain, one slow or failing service can cascade failures upstream. The **circuit breaker pattern** prevents that by:

- **Closed** state: requests flow normally; failures are counted
- **Open** state: after N consecutive failures, requests are rejected immediately without waiting
- **Half-Open** state: after a timeout, a probe request is allowed to test if the service recovered

I implemented a test circuit breaker in the unit tests to verify:
1. Closed state allows requests
2. After threshold is reached, circuit opens
3. Open state rejects requests with `InvalidOperationException`
4. After timeout, circuit moves to half-open and recovers

This was paired with **retry logic** (transient failures retried up to 3 times) and **timeout policies** (slow operations cancelled after 50ms).

## Why Shared Pipeline Context?

The `PipelineContext` model is shared across all services. This was a deliberate design choice:

- **Trace correlation**: Each pipeline run has a unique context that flows through all services
- **Cross-service state**: Services can enrich the context as it passes through (e.g., ResearchSummarizer adds findings, Reporter generates the report)
- **Strategy selection**: The orchestrator selects the execution strategy based on context properties (topic complexity, preferred output format)

This pattern resembles how distributed tracing works in production — a trace context propagates through service boundaries, carrying metadata along the way.

## Why the Strategy Pattern?

The agent orchestration logic uses a **Strategy pattern** because different tasks need different execution flows:

- **SequentialStrategy**: Executes services one at a time (for simple, linear workflows)
- **ParallelStrategy**: Executes independent services concurrently (for complex tasks where services don't depend on each other)
- **ConditionalStrategy**: Branches execution based on context properties

This makes the orchestration engine extensible — adding a new strategy doesn't require modifying existing code. It follows the Open/Closed principle.

## Biggest Challenge

The hardest part was **getting OpenTelemetry to work end-to-end** through Docker Compose.

The issues were:
1. Jaeger was restarting due to badger file permissions — had to fix volume mounts
2. The .NET OTLP exporter was using `http://localhost:4317` (container-local) instead of `http://jaeger:4317` (cross-service) because `appsettings.json` was missing
3. Health check endpoints flooded the traces — had to add an `ActivityFilter` to exclude `/health` and `/swagger`
4. The `OpenTelemetry.Exporter.Jaeger` NuGet package was deprecated — had to switch to OTLP exporter
5. The `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable wasn't being read by the code (it read `OpenTelemetry:OtlpEndpoint` from `IConfiguration` instead)

Fixing these taught me how configuration cascades in .NET (`IConfiguration` → environment variables → `appsettings.json`), how Docker networking works for service-to-service communication, and how OpenTelemetry's .NET SDK initializes exporters.

## What Would I Do Differently?

If I were redesigning IntelliFlow today:

1. **Async communication**: Use a message queue (RabbitMQ, Redis streams, or Azure Service Bus) instead of synchronous HTTP for inter-service communication. This would decouple services, improve resilience, and make the system truly reactive.

2. **Configuration management**: Use a central config service or environment-variable-only approach instead of relying on `appsettings.json` files that can go missing.

3. **Health check consolidation**: Instead of curl-based health checks in Docker, use .NET's built-in `IHealthCheck` with better diagnostics and a dedicated health check endpoint.

4. **Faster CI/CD**: The Docker builds take several minutes because the SDK image is pulled fresh. I'd implement Docker layer caching more aggressively or use a multi-stage build cache.

5. **API Gateway**: Instead of the Orchestrator acting as both gateway and workflow engine, I'd separate API Gateway (routing, auth, rate limiting) from the workflow engine.

6. **Database for state**: Currently, the pipeline context is in-memory. For a production system, I'd persist context to a database with saga-like compensation transactions.

## How Is This Different from ARPS?

This question isn't directly applicable to IntelliFlow since it's not related to ARPS (which was a different project). However, if asked about prior work:

Each academic project at Bahria University served a different purpose:
- **Semester 1 (Computer Programming Lab)**: Individual programming basics — data structures, algorithms, console applications
- **Semester 5 (Software Construction Lab)**: Client-server architecture — socket programming with a chatbot
- **Semester 6 (Cloud Computing Lab — IntelliFlow)**: Distributed systems — microservices, containers, observability, resilience

The progression shows growth from procedural programming to networked applications to distributed cloud-native systems.

## Why Call It an Agent Orchestration Platform?

"Agent orchestration" describes what the orchestrator does — it coordinates multiple specialized services to complete a complex task:

1. **User submits a task** → Orchestrator receives the request
2. **Selects a strategy** → Based on task parameters, picks sequential, parallel, or conditional execution
3. **Dispatches to agents** → Research Summarizer (researches topic), Reporter (generates findings), Notifier (delivers results)
4. **Collects results** → Aggregates responses into a pipeline context
5. **Handles failures** → Retries, circuit breakers, and timeouts ensure robustness

Each service acts as an "agent" with a specialized capability. The orchestrator is the "orchestrator" that composes them into a workflow. This is the same pattern used by platforms like Apache Airflow, Temporal, or AWS Step Functions — just at a smaller scale.

The term "agent" here aligns with the industry concept of an autonomous service that performs a specific function, which is distinct from "AI agents" (though IntelliFlow's pipeline could integrate AI summarization as one of its agents).
