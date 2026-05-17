# IntelliFlow — Cloud-Native Multi-Agent AI Task Automation System

> SEL 401 Cloud Computing Lab Project | Bahria University Karachi | BSE-6 | Spring 2026

## Overview
IntelliFlow automates the full knowledge-work pipeline: a user submits a research topic → agents research, summarize, generate a report, deliver it via email, and log a blockchain hash for audit.

## Team
| # | Name | Enrollment | Module |
|---|------|-----------|--------|
| 1 | M. Khizar Akram (Lead) | 02-131232-064 | Orchestrator + API Gateway |
| 2 | Hamza Khaliq | 02-131232-059 | Research Agent + LLM Summarizer |
| 3 | Hassan Asif | 02-131232-113 | Reporter Agent + Storage + DB |
| 4 | Shamraiz | 02-131232-112 | Notifier + Blockchain + DevOps |

## Architecture
```
User → REST API (Orchestrator)
              ↓
     ┌────────┴────────┐
     ↓                 ↓
 Research Agent    (parallel fan-out)
     ↓
 Summarizer Agent (OpenRouter / Llama 3)
     ↓
 Reporter Agent (Supabase Storage + PostgreSQL)
     ↓
 Notifier Agent → Email + Blockchain Hash (Sepolia)
```

## Quick Start (Local Dev with Docker Compose)
```bash
git clone https://github.com/<your-org>/IntelliFlow.git
cd IntelliFlow
cp .env.example .env        # fill in your secrets
docker compose up --build
```
API will be available at `http://localhost:5000`

## Repository Structure
```
IntelliFlow/
├── src/
│   ├── Orchestrator/        # Module 1 — Khizar
│   ├── ResearchSummarizer/  # Module 2 — Hamza
│   ├── Reporter/            # Module 3 — Hassan
│   └── Notifier/            # Module 4 — Shamraiz
├── contracts/               # Solidity smart contract
├── database/                # SQL schema
├── docs/                    # Architecture diagrams
└── .github/workflows/       # CI/CD pipeline
```

## Environment Variables
See `.env.example` for all required variables. Never commit real secrets.

## Tech Stack
- **Backend:** ASP.NET Core 8, Entity Framework Core
- **LLM:** OpenRouter API (Llama 3 70B)
- **Cloud:** Supabase (PostgreSQL, Storage) + Azure Container Instances
- **Blockchain:** Ethereum Sepolia, Solidity, Nethereum, Alchemy
- **DevOps:** Docker, GitHub Container Registry, GitHub Actions
- **Auth:** JWT (built-in ASP.NET Core)
