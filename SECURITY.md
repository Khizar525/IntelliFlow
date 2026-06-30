# Security Policy

## Overview

This document outlines the security measures implemented in the IntelliFlow system.

## Reporting Security Issues

If you discover a security vulnerability, please report it responsibly:

1. **Do not** create a public GitHub issue
2. **Email**: [Your email here]
3. **Include**: Description of the vulnerability and steps to reproduce
4. **Response**: We will respond within 48 hours

## Security Features

### Authentication & Authorization

#### JWT Authentication
- **Token-Based Authentication**: JWT tokens for stateless authentication
- **Secure Token Storage**: Tokens stored securely in client-side storage
- **Token Expiration**: Configurable token lifetime
- **Issuer/Audience Validation**: Token validation against configured values

#### Password Security
- **BCrypt Hashing**: Passwords hashed using BCrypt with salt
- **No Plain Text Storage**: Passwords are never stored in plain text
- **Secure Password Requirements**: Minimum length and complexity requirements

### API Security

#### Rate Limiting
- **Request Rate Limits**: Prevents abuse and DoS attacks
- **IP-Based Tracking**: Tracks requests per IP address
- **Configurable Limits**: Adjustable rate limits per endpoint

#### Input Validation
- **Request Validation**: All inputs validated before processing
- **SQL Injection Prevention**: Parameterized queries via Entity Framework Core
- **XSS Protection**: Output encoding and input sanitization

#### CORS Configuration
- **Origin Restrictions**: Configured allowed origins
- **Method Restrictions**: Only allowed HTTP methods
- **Header Restrictions**: Controlled allowed headers

### Data Security

#### Encryption in Transit
- **HTTPS/TLS**: All communications encrypted in transit
- **Certificate Validation**: Proper SSL certificate validation

#### Encryption at Rest
- **Database Encryption**: Supabase provides encryption at rest
- **Secret Management**: Environment variables for sensitive data

#### Secret Management
- **Environment Variables**: Secrets stored in environment variables
- **Gitignore Protection**: `.env` files excluded from version control
- **No Hardcoded Secrets**: No secrets in source code

### Infrastructure Security

#### Container Security
- **Non-Root User**: Containers run as non-root user
- **Minimal Base Images**: Using minimal Docker base images
- **Security Scanning**: Regular vulnerability scanning

#### Network Security
- **Network Isolation**: Docker network isolation for services
- **Port Exposure**: Minimal port exposure (only necessary ports)
- **Internal Communication**: Services communicate via internal network

### Blockchain Security

#### Smart Contract Security
- **Access Control**: Owner-only functions for sensitive operations
- **Input Validation**: Proper validation of contract inputs
- **Event Logging**: Comprehensive event emission for audit trails

#### Private Key Management
- **Secure Storage**: Private keys stored securely
- **No Hardcoded Keys**: No private keys in source code
- **Testnet Only**: Using Sepolia testnet for development

## Security Checklist

### Authentication
- [x] JWT token authentication
- [x] Password hashing with BCrypt
- [x] Token expiration
- [x] Secure token storage

### API Security
- [x] Rate limiting
- [x] Input validation
- [x] CORS configuration
- [x] Error handling without information leakage

### Data Security
- [x] HTTPS/TLS encryption
- [x] Environment variable management
- [x] Gitignore for secrets

### Infrastructure
- [x] Container security
- [x] Network isolation
- [x] Health checks
- [x] Logging and monitoring

## Best Practices

### For Developers

1. **Never commit secrets**: Use environment variables
2. **Input validation**: Always validate user input
3. **Output encoding**: Encode outputs to prevent XSS
4. **Parameterized queries**: Use ORM for database access
5. **Error handling**: Don't expose sensitive info in errors
6. **Logging**: Log security events without sensitive data
7. **Dependencies**: Keep dependencies updated

### For Deployment

1. **HTTPS**: Always use HTTPS in production
2. **Environment Variables**: Store secrets in environment variables
3. **Database Security**: Use strong passwords and limit access
4. **Firewall**: Configure firewall appropriately
5. **Monitoring**: Monitor for suspicious activity
6. **Backups**: Regular encrypted backups
7. **Updates**: Keep system and dependencies updated

## Known Security Considerations

### Development Mode
- **Debug Information**: More detailed errors in development
- **Testnet**: Using Sepolia testnet for blockchain
- **Local Development**: Running locally for development

### Production Recommendations
1. **Enable HTTPS**: Configure SSL certificates
2. **Database Encryption**: Enable encryption at rest
3. **Audit Logging**: Implement comprehensive audit logging
4. **Intrusion Detection**: Set up intrusion detection
5. **Regular Audits**: Conduct regular security assessments

## Compliance

This application implements security controls aligned with:
- **OWASP Top 10**: Addresses common web application risks
- **NIST Cybersecurity Framework**: Follows security best practices

## Last Updated

- **Date**: June 2026
- **Version**: 1.0
- **Author**: M. Khizar Akram
