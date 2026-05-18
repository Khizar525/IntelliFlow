# IntelliFlow — Cloud-Native Multi-Agent AI Task Automation System

> SEL 401 Cloud Computing Lab Project | Bahria University Karachi | BSE-6 | Spring 2026

## Overview
IntelliFlow automates the full knowledge-work pipeline: a user submits a research topic through a modern React dashboard → 5 agents execute in sequence (Research → Summarize → Report → Notify → Blockchain) → results delivered via email with an on-chain audit trail.

## Team
| # | Name | Enrollment | Module |
|---|------|-----------|--------|
| 1 | M. Khizar Akram (Lead) | 02-131232-064 | Orchestrator + API Gateway + Frontend |
| 2 | Hamza Khaliq | 02-131232-059 | Research Agent + LLM Summarizer |
| 3 | Hassan Asif | 02-131232-113 | Reporter Agent + Storage + DB |
| 4 | Shamraiz | 02-131232-112 | Notifier + Blockchain + DevOps |

## Architecture
```
User → React Dashboard (localhost:5173)
         ↓
   Orchestrator API (localhost:5000)
               ↓
      ┌────────┴────────┐
      ↓                 ↓
  Research Agent    (sequential pipeline)
      ↓
  Summarizer Agent (OpenRouter free-tier with fallback retry)
      ↓
  Reporter Agent (Supabase Storage + PostgreSQL)
      ↓
  Notifier Agent → Email (SMTP) + Blockchain Hash (Sepolia)
```

## Quick Start

### Prerequisites
- Docker Desktop
- Node.js 18+ (for frontend dev)

### 1. Clone & Configure
```bash
git clone https://github.com/Khizar525/IntelliFlow.git
cd IntelliFlow
# .env is already configured — do NOT commit it
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

## Repository Structure
```
IntelliFlow/
├── frontend/                  # React + Vite + TypeScript + Tailwind v4
│   ├── src/
│   │   ├── components/        # LoginPage, Dashboard, TaskForm, PipelineTimeline, ResultCard
│   │   ├── services/          # API client
│   │   └── context/           # Auth context
│   └── package.json
├── src/
│   ├── Orchestrator/          # Module 1 — Khizar (API Gateway, pipeline orchestration)
│   ├── ResearchSummarizer/    # Module 2 — Hamza (web scraping, LLM summarization)
│   ├── Reporter/              # Module 3 — Hassan (PDF generation, Supabase storage)
│   └── Notifier/              # Module 4 — Shamraiz (SMTP email, blockchain logging)
├── contracts/                 # Solidity smart contract (AuditLog.sol)
├── database/                  # SQL schema
├── docs/                      # Architecture diagrams
├── .github/workflows/         # CI/CD pipeline
├── docker-compose.yml         # 4 backend services
└── .env                       # Secrets (gitignored)
```

## Tech Stack
- **Frontend:** React 18, Vite, TypeScript, Tailwind CSS v4, React Router
- **Backend:** ASP.NET Core 8, Entity Framework Core, HTTP-based microservices
- **LLM:** OpenRouter API (free-tier: Gemma 4, Llama 3.2, Dolphin Mistral — with auto-fallback)
- **Cloud:** Supabase (PostgreSQL, Storage)
- **Blockchain:** Ethereum Sepolia, Solidity, Nethereum, Alchemy RPC
- **DevOps:** Docker, Docker Compose, GitHub Actions
- **Auth:** JWT (ASP.NET Core Identity)

## Environment Variables
The `.env` file contains all required secrets. **Never commit it.** Key variables:
- `OPENROUTER_API_KEY` — LLM access
- `SUPABASE_URL` / `SUPABASE_KEY` — Storage + DB
- `ALCHEMY_SEPOLIA_RPC_URL` / `ETH_PRIVATE_KEY` — Blockchain
- `SMTP_*` — Email delivery
