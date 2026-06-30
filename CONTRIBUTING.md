# Contributing to IntelliFlow

Thank you for your interest in contributing to IntelliFlow! This document provides guidelines and information for contributors.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Contributing Guidelines](#contributing-guidelines)
- [Pull Request Process](#pull-request-process)
- [Coding Standards](#coding-standards)
- [Reporting Issues](#reporting-issues)

## Code of Conduct

This project and everyone participating in it is governed by our Code of Conduct. By participating, you are expected to uphold this code. Please report unacceptable behavior to [your-email@example.com].

## Getting Started

### Prerequisites

- Docker Desktop
- Node.js 18+ (for frontend development)
- .NET 8 SDK (for backend development)
- Git

### Development Setup

1. **Fork the repository**
   ```bash
   # Fork on GitHub, then clone your fork
   git clone https://github.com/YOUR_USERNAME/IntelliFlow.git
   cd IntelliFlow
   ```

2. **Set up environment variables**
   ```bash
   cp .env.example .env
   # Edit .env with your API keys and credentials
   ```

3. **Start the development environment**
   ```bash
   # Start backend services
   docker compose up --build -d

   # Start frontend (in a separate terminal)
   cd frontend
   npm install
   npm run dev
   ```

4. **Verify the setup**
   - Backend API: http://localhost:5000
   - Frontend Dashboard: http://localhost:5173
   - Swagger UI: http://localhost:5000/swagger

## Contributing Guidelines

### Branch Naming

Use descriptive branch names with prefixes:

- `feature/` - New features
- `fix/` - Bug fixes
- `docs/` - Documentation updates
- `refactor/` - Code refactoring
- `test/` - Adding or updating tests

Example:
```bash
git checkout -b feature/add-rate-limiting
git checkout -b fix/email-service-timeout
```

### Commit Messages

Write clear, concise commit messages:

- Use the present tense ("Add feature" not "Added feature")
- Use the imperative mood ("Move cursor to..." not "Moves cursor to...")
- Keep the first line under 72 characters
- Reference issues and pull requests

Example:
```
feat: add rate limiting to API endpoints

- Implement IP-based rate limiting
- Add configurable limits per endpoint
- Update documentation

Closes #123
```

### Code Style

#### C# (.NET)
- Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Add XML documentation comments for public members
- Keep methods focused and under 30 lines

#### TypeScript/React
- Follow [Airbnb JavaScript Style Guide](https://github.com/airbnb/javascript)
- Use functional components with hooks
- Add TypeScript types for all props and state
- Keep components small and focused

#### Solidity
- Follow [Solidity Style Guide](https://docs.soliditylang.org/en/latest/style-guide.html)
- Use NatSpec comments for public functions
- Keep contracts focused and modular

## Pull Request Process

1. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make your changes**
   - Follow coding standards
   - Add or update tests as needed
   - Update documentation if required

3. **Test your changes**
   ```bash
   # Run backend tests
   dotnet test

   # Run frontend tests
   cd frontend
   npm test

   # Test with Docker
   docker compose up --build
   ```

4. **Commit your changes**
   ```bash
   git add .
   git commit -m "feat: add your feature description"
   ```

5. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Create a Pull Request**
   - Provide a clear description of the changes
   - Reference any related issues
   - Include screenshots if applicable
   - Ensure all checks pass

### Review Process

- At least one team member must review the PR
- Address all review comments
- Ensure CI/CD pipeline passes
- Update documentation as needed

## Coding Standards

### Security

- Never commit secrets, API keys, or credentials
- Use environment variables for configuration
- Validate all user inputs
- Follow OWASP security guidelines

### Testing

- Write unit tests for new features
- Maintain test coverage above 70%
- Test edge cases and error scenarios
- Include integration tests for API endpoints

### Documentation

- Update README for significant changes
- Add inline comments for complex logic
- Document API endpoints with examples
- Keep architecture diagrams up to date

## Reporting Issues

### Bug Reports

When filing a bug report, please include:

- **Description**: Clear description of the issue
- **Steps to reproduce**: Detailed steps to reproduce the behavior
- **Expected behavior**: What you expected to happen
- **Actual behavior**: What actually happened
- **Environment**: OS, browser, .NET version, Node.js version
- **Screenshots**: If applicable, add screenshots

### Feature Requests

When suggesting a feature, please include:

- **Description**: Clear description of the feature
- **Use case**: Why this feature would be useful
- **Proposed solution**: How you think it could be implemented
- **Alternatives**: Any alternative solutions considered

## Architecture Overview

```
IntelliFlow/
├── frontend/                  # React + Vite + TypeScript
├── src/
│   ├── Orchestrator/          # API Gateway + Pipeline Orchestration
│   ├── ResearchSummarizer/    # Web Scraping + LLM Summarization
│   ├── Reporter/              # PDF Generation + Storage
│   └── Notifier/              # Email + Blockchain Logging
├── contracts/                 # Solidity Smart Contracts
├── database/                  # SQL Schema
└── docs/                      # Documentation
```

## Getting Help

- Check the [README](README.md) for setup instructions
- Review existing [issues](https://github.com/Khizar525/IntelliFlow/issues)
- Contact the team lead: M. Khizar Akram

## License

By contributing to IntelliFlow, you agree that your contributions will be licensed under the MIT License.
