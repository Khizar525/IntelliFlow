# IntelliFlow — Cloud-Native Multi-Agent AI Task Automation System

> SEL 401 Cloud Computing Lab Project | Bahria University Karachi | BSE-6 | Spring 2026

## Project Overview

IntelliFlow is a cloud-native multi-agent system that automates the knowledge-work pipeline. Users submit research topics through a modern React dashboard, and five specialized agents execute in sequence: Research → Summarize → Report → Notify → Blockchain. The system delivers results via email with an on-chain audit trail, demonstrating practical application of microservices architecture, LLM integration, and blockchain technology.

## Academic Context

This project was developed as part of the SEL 401 Cloud Computing Lab course during the Spring 2026 semester. The primary objective was to apply cloud computing concepts including containerization, microservices architecture, and distributed systems in a practical, real-world scenario.

## Problem Statement

In academic and professional settings, researchers and knowledge workers spend significant time on repetitive tasks: gathering information from multiple sources, summarizing findings, generating reports, and maintaining audit trails. This manual process is time-consuming, error-prone, and lacks systematic documentation.

IntelliFlow addresses this challenge by creating an automated pipeline that:
- Reduces manual research and report generation time
- Provides consistent, structured output formats
- Maintains verifiable audit trails through blockchain technology
- Demonstrates cloud-native architecture patterns for scalable automation

## Features Implemented

### Core Pipeline Features
- **Multi-Agent Architecture**: Five specialized agents working in sequence
- **Research Agent**: Web scraping and content aggregation from multiple sources
- **Summarizer Agent**: LLM-powered content summarization with fallback retry mechanisms
- **Reporter Agent**: Automated PDF report generation with Supabase cloud storage
- **Notifier Agent**: Email delivery with SMTP integration
- **Blockchain Integration**: SHA-256 hash logging on Ethereum Sepolia testnet

### Frontend Features
- **Modern React Dashboard**: Built with React 18, Vite, TypeScript, and Tailwind CSS v4
- **User Authentication**: JWT-based authentication with ASP.NET Core Identity
- **Task Management**: Create, track, and manage automation tasks
- **Pipeline Visualization**: Animated timeline showing real-time pipeline execution
- **Responsive Design**: Mobile-friendly interface with modern UI/UX

### Backend Features
- **Microservices Architecture**: Four independent services communicating via HTTP
- **Containerization**: Docker Compose orchestration for all backend services
- **Cloud Integration**: Supabase for PostgreSQL database and file storage
- **API Gateway**: Centralized orchestrator managing pipeline execution
- **Error Handling**: Retry mechanisms and fallback strategies for external APIs

### DevOps Features
- **Docker Containerization**: All services containerized for consistent deployment
- **CI/CD Pipeline**: GitHub Actions for automated testing and deployment
- **Environment Management**: Secure configuration with environment variables
- **Health Checks**: Service health monitoring and automatic restarts

### Security & Resilience Enhancements
- **Rate Limiting**: IP-based rate limiting to prevent abuse
- **Global Exception Handler**: Consistent error responses across all services
- **Request Logging**: Correlation IDs and request duration tracking
- **CORS Configuration**: Proper cross-origin resource sharing policies
- **Polly Retry Policies**: Automatic retry with exponential backoff for external calls

## Technology Stack

### Frontend
- **React 18** with TypeScript for type-safe development
- **Vite** for fast build tooling and development server
- **Tailwind CSS v4** for modern, utility-first styling
- **React Router** for client-side routing

### Backend
- **ASP.NET Core 8** for high-performance REST APIs
- **Entity Framework Core** for database operations
- **HTTP-based microservices** for inter-service communication
- **Polly** for resilience and transient fault handling

### AI & LLM Integration
- **OpenRouter API** for LLM access (Gemma 4, Llama 3.2, Dolphin Mistral)
- **Auto-fallback mechanism** for API rate limits and errors

### Cloud Services
- **Supabase** for PostgreSQL database and file storage
- **SMTP integration** for email delivery

### Blockchain
- **Ethereum Sepolia** testnet for proof-of-concept
- **Solidity** smart contracts for audit logging
- **Nethereum** .NET library for blockchain interaction
- **Alchemy RPC** for blockchain connectivity

### DevOps & Tools
- **Docker & Docker Compose** for containerization
- **GitHub Actions** for CI/CD pipelines
- **Git** for version control

## Project Structure

```
IntelliFlow/
├── frontend/                  # React + Vite + TypeScript + Tailwind v4
│   ├── src/
│   │   ├── components/        # LoginPage, Dashboard, TaskForm, PipelineTimeline, ResultCard
│   │   ├── api/               # API client services
│   │   ├── context/           # Auth context
│   │   └── types/             # TypeScript type definitions
│   └── package.json
├── src/
│   ├── Orchestrator/          # Module 1 — API Gateway, pipeline orchestration
│   │   ├── API/
│   │   │   ├── Controllers/   # AuthController, TasksController
│   │   │   ├── Middlewares/   # RateLimiting, GlobalException, RequestLogging
│   │   │   └── Services/      # OrchestratorService
│   │   └── Dockerfile
│   ├── ResearchSummarizer/    # Module 2 — Web scraping, LLM summarization
│   │   ├── ResearchSummarizer.API/
│   │   │   ├── Controllers/   # ResearchController
│   │   │   └── Services/      # ResearchService, SummarizerService
│   │   └── Dockerfile
│   ├── Reporter/              # Module 3 — PDF generation, Supabase storage
│   │   ├── API/
│   │   │   ├── Controllers/   # ReportController
│   │   │   └── Services/      # ReportService
│   │   └── Dockerfile
│   └── Notifier/              # Module 4 — SMTP email, blockchain logging
│       ├── API/
│       │   ├── Controllers/   # NotifyController
│       │   └── Services/      # EmailService, BlockchainService
│       └── Dockerfile
├── contracts/                 # Solidity smart contract (AuditLog.sol)
├── database/                  # SQL schema
├── docs/                      # Documentation
├── .github/workflows/         # CI/CD pipeline
├── docker-compose.yml         # 4 backend services
├── .env.example               # Environment variable template
├── LICENSE                    # MIT License
├── CONTRIBUTING.md            # Contribution guidelines
├── SECURITY.md                # Security documentation
└── README.md                  # This file
```

## Quick Start

### Prerequisites
- Docker Desktop
- Node.js 18+ (for frontend development)
- Git

### 1. Clone & Configure
```bash
git clone https://github.com/Khizar525/IntelliFlow.git
cd IntelliFlow
cp .env.example .env
# Edit .env with your API keys and credentials
```

### 2. Start Backend (Docker)
```bash
docker compose up --build -d
```
Backend API: `http://localhost:5000`

### 3. Start Frontend (React)
```bash
cd frontend
npm install    # first time only
npm run dev
```
Frontend Dashboard: `http://localhost:5173`

### 4. Run a Pipeline
1. Open `http://localhost:5173` in your browser
2. Login with: `admin@intelliflow.com` / `password123`
3. Create a new task with a research topic
4. Watch the animated pipeline timeline execute all 5 stages

## Pipeline Stages

| # | Stage | Service | Port | Description |
|---|-------|---------|------|-------------|
| 1 | Research | ResearchSummarizer | 5001 | Fetches and scrapes web content on the topic |
| 2 | Summarize | ResearchSummarizer | 5001 | LLM summary via OpenRouter (auto-fallback on 404/429) |
| 3 | Report | Reporter | 5002 | Generates PDF report, stores in Supabase |
| 4 | Notify | Notifier | 5003 | Sends email via SMTP with report link |
| 5 | Blockchain | Notifier | 5003 | Logs SHA-256 hash on Ethereum Sepolia testnet |

## API Endpoints

### Orchestrator (Port 5000)
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/login` | Get JWT token | No |
| GET | `/api/auth/health` | Auth service health | No |
| POST | `/api/tasks` | Submit new task | Yes |
| GET | `/api/tasks/health` | Tasks service health | No |
| GET | `/health` | Overall health check | No |

### ResearchSummarizer (Port 5001)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/research` | Research and summarize topic |
| GET | `/api/research/health` | Service health |
| GET | `/health` | Overall health check |

### Reporter (Port 5002)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/report` | Generate and store report |
| GET | `/api/report/health` | Service health |
| GET | `/health` | Overall health check |

### Notifier (Port 5003)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/notify` | Send email and log to blockchain |
| GET | `/api/notify/health` | Service health |
| GET | `/health` | Overall health check |

## Environment Variables

The `.env` file contains all required secrets. **Never commit it.** Key variables:
- `OPENROUTER_API_KEY` — LLM access
- `SUPABASE_URL` / `SUPABASE_KEY` — Storage + DB
- `ALCHEMY_SEPOLIA_RPC_URL` / `ETH_PRIVATE_KEY` — Blockchain
- `SMTP_*` — Email delivery
- `JWT_SECRET` — Authentication

## Key Concepts Learned

### Cloud Computing & Architecture
- **Microservices Design**: Breaking monolithic applications into independent, scalable services
- **Containerization**: Using Docker to ensure consistent environments across development and deployment
- **API Gateway Pattern**: Centralized entry point for managing complex service interactions
- **Event-Driven Architecture**: Implementing sequential pipeline processing with error handling

### AI & Machine Learning Integration
- **LLM API Integration**: Working with multiple language model providers and handling rate limits
- **Prompt Engineering**: Crafting effective prompts for consistent summarization outputs
- **Fallback Strategies**: Implementing retry mechanisms and provider fallback for reliability

### Blockchain Technology
- **Smart Contract Development**: Writing Solidity contracts for audit logging
- **Testnet Deployment**: Using Ethereum Sepolia for proof-of-concept testing
- **Hash Verification**: Implementing SHA-256 hashing for data integrity

### Full-Stack Development
- **React State Management**: Using Context API for authentication and application state
- **TypeScript**: Leveraging type safety for better code quality and developer experience
- **Modern CSS**: Utilizing Tailwind CSS for rapid UI development
- **Database Design**: Implementing relational schemas with Supabase PostgreSQL

### DevOps Practices
- **CI/CD Pipelines**: Automating build, test, and deployment processes
- **Environment Configuration**: Managing secrets and configuration across environments
- **Container Orchestration**: Using Docker Compose for multi-service applications

## Future Improvements

### Short-term Enhancements
- **Enhanced Error Handling**: More granular error reporting and recovery mechanisms
- **User Interface Improvements**: Additional dashboard analytics and task filtering options
- **Documentation**: Comprehensive API documentation and deployment guides
- **Testing**: Unit tests for individual agents and integration tests for the pipeline

### Medium-term Features
- **Real-time Updates**: WebSocket integration for live pipeline status updates
- **Task Scheduling**: Cron-based scheduling for automated recurring tasks
- **Multi-user Support**: Role-based access control and user management
- **Export Options**: Additional report formats (Word, HTML, Markdown)

### Long-term Vision
- **Scalability**: Kubernetes deployment for production-grade orchestration
- **Performance**: Caching mechanisms and parallel processing optimization
- **Security**: Enhanced authentication, input validation, and API security
- **Monitoring**: Comprehensive logging, metrics, and alerting systems
- **AI Enhancement**: Integration with additional AI models and specialized agents

## Team

| # | Name | Enrollment | Module |
|---|------|-----------|--------|
| 1 | M. Khizar Akram (Lead) | 02-131232-064 | Orchestrator + API Gateway + Frontend |
| 2 | Hamza Khaliq | 02-131232-059 | Research Agent + LLM Summarizer |
| 3 | Hassan Asif | 02-131232-113 | Reporter Agent + Storage + DB |
| 4 | Shamraiz | 02-131232-112 | Notifier + Blockchain + DevOps |

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Note**: This project demonstrates cloud computing concepts and multi-agent architecture patterns for educational purposes. It is designed to showcase practical application of course concepts rather than production-grade enterprise systems.
