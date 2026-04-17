# SpaceShopper API

SpaceShopper is a backend API for an e-commerce system, built with **ASP.NET Core 8** and organized with a Clean Architecture approach.

This repository demonstrates practical backend engineering concerns: layered design, JWT authentication, refresh-token rotation, Redis integration, SMTP-based transactional email, and structured exception handling.

## Table of Contents

- [Highlights](#highlights)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Technology Stack](#technology-stack)
- [Domain Overview](#domain-overview)
- [API Overview](#api-overview)
- [Authentication and Session Strategy](#authentication-and-session-strategy)
- [Email Flows](#email-flows)
- [Error Handling Contract](#error-handling-contract)
- [Prerequisites](#prerequisites)
- [Configuration](#configuration)
- [Run Locally](#run-locally)
- [Database Migrations](#database-migrations)
- [Current Implementation Notes](#current-implementation-notes)

## Highlights

- Clean separation across API, Application, Domain, and Infrastructure layers.
- JWT access token + refresh token flow with token rotation.
- Refresh token validation backed by Redis with database fallback.
- User workflows for registration, verification, password reset, profile, address, and payment methods.
- Centralized API response envelope and exception middleware.
- Serilog-based request and application logging.

## Architecture

The solution is organized into four projects:

```text
SpaceShopper.API            Presentation layer (Controllers, Middleware, DI, HTTP pipeline)
SpaceShopper.Application    Use-cases, DTOs, interfaces, business services
SpaceShopper.Domain         Core entities, enums, and domain behavior
SpaceShopper.Infrastructure EF Core, repositories, security, caching, email adapters
```

### Patterns in use

- Repository pattern (`IRepository<T>`, aggregate-specific repositories)
- Unit of Work (`IUnitOfWork` + `SaveChangesAsync` boundary)
- Domain-centric entities with aggregate roots and soft-delete support
- Options pattern for configuration binding (`JwtOptions`, `SmtpOptions`, cache settings)

## Project Structure

```text
src/
├── SpaceShopper.API/
│   ├── Controllers/
│   │   ├── Auth/
│   │   ├── Users/
│   │   ├── Catalog/
│   │   ├── Dev/
│   │   └── Common/
│   ├── Middlewares/
│   ├── Models/
│   └── Program.cs
│
├── SpaceShopper.Application/
│   ├── Common/
│   ├── Dtos/
│   ├── Interfaces/
│   ├── Requests/
│   └── Services/
│
├── SpaceShopper.Domain/
│   ├── Common/
│   ├── Entities/
│   │   ├── Users/
│   │   ├── Catalog/
│   │   ├── Orders/
│   │   └── Contents/
│   ├── Enums/
│   └── Models/
│
└── SpaceShopper.Infrastructure/
    ├── Data/
    ├── Migrations/
    ├── Repositories/
    ├── Security/
    ├── Caching/
    └── Services/
        └── Email/
```

## Technology Stack

| Category | Technology |
|---|---|
| Runtime | .NET 8 / ASP.NET Core Web API |
| Data Access | Entity Framework Core 8 |
| Database | PostgreSQL (Npgsql provider) |
| Caching | Redis (`Microsoft.Extensions.Caching.StackExchangeRedis`) |
| Auth | JWT Bearer |
| Password Hashing | PBKDF2 (`Rfc2898DeriveBytes`) |
| Email | MailKit + SMTP templates |
| Mapping | AutoMapper |
| Logging | Serilog |
| API Docs | Swagger / OpenAPI |

## Domain Overview

Key aggregate and relationship ideas:

- `User` manages profile state, addresses, payment methods, and token versioning.
- `UserToken` stores refresh tokens with expiration and version checks.
- `Product` belongs to `Category` and is linked with stock, images, and reviews.
- Order domain includes `Order`, `OrderDetail`, and promotion-related entities.

Base abstractions:

- `BaseEntity`: strongly-typed identity (`Guid`).
- `AuditableAggregateRoot`: audit metadata.
- `SoftDeletableAggregateRoot`: logical delete support.

## API Overview

Base prefix: `/api/v1` (except dev utilities under `/api/dev`).

### Auth (`/api/v1/auth`)

- `POST /login`
- `POST /login-by-code`
- `POST /refresh-token`

### Users (`/api/v1/users`)

- Registration and verification: `register`, `resend-email`
- Password flows: `reset-password`, `change-password-by-code`, `change-password`
- Profile: `GET /`, `PATCH /`
- Address management: `GET/POST/PATCH/DELETE /address...`
- Payment method management: `GET/POST/PATCH/DELETE /payment...`

### Catalog (`/api/v1/product`)

- `GET /products`

### Dev Utilities (`/api/dev`)

- `GET /clone-products`
- `GET /clone-categories`

> Dev endpoints are intended for local data import/seeding scenarios.

## Authentication and Session Strategy

The API uses a dual-token model:

1. Login issues an access token and a refresh token.
2. Refresh flow rotates tokens (old refresh token is revoked).
3. Password changes increment token version, invalidating previous refresh sessions.

Refresh-token validation strategy:

- Primary check in Redis (fast path)
- Fallback check in PostgreSQL token store (resilience path)

## Email Flows

### Registration verification

- Register request stores pending registration data in Redis with TTL.
- Verification code is sent via SMTP using HTML templates.
- Login-by-code completes account activation/login flow.

### Password reset

- Reset request stores reset code in Redis with TTL.
- Reset email is sent using HTML template.
- Code-based password change updates password and refresh-session state.

Email templates are embedded in the Infrastructure assembly and rendered with placeholder content.

## Error Handling Contract

Responses follow a unified envelope (`ApiResponse<T>`):

```json
{
  "success": false,
  "data": null,
  "error": {
    "code": "auth.invalid_credentials",
    "message": "Invalid email or password.",
    "details": null
  }
}
```

Exception mapping is centralized in middleware:

- Validation errors -> `400`
- Unauthorized errors -> `401`
- Not found errors -> `404`
- Domain/business conflicts -> `409`
- Unhandled exceptions -> `500`

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/)
- [Redis](https://redis.io/)
- SMTP account for email delivery (Gmail or equivalent)

## Configuration

Set environment-specific values in `src/SpaceShopper.API/appsettings.Development.json`.

Required sections:

- `ConnectionStrings:PostgresConnection`
- `Jwt` (`Issuer`, `Audience`, `SecretKey`, expiration settings)
- `Redis:ConnectionString`
- `Smtp` (`Host`, `Port`, credentials, sender, secure option)

Security recommendations:

- Do not commit real credentials, connection strings, or app passwords.
- Prefer environment variables or user secrets for sensitive values.
- Rotate keys/passwords before sharing this project publicly.

## Run Locally

```bash
dotnet restore

dotnet ef database update --project src/SpaceShopper.Infrastructure --startup-project src/SpaceShopper.API

dotnet run --project src/SpaceShopper.API
```

After startup:

- Swagger UI: `https://localhost:<port>/swagger`
- Logs: `logs/spaceshopper-<date>.txt`

## Database Migrations

```bash
# Add migration
dotnet ef migrations add <MigrationName> --project src/SpaceShopper.Infrastructure --startup-project src/SpaceShopper.API --output-dir Migrations

# Update database
dotnet ef database update --project src/SpaceShopper.Infrastructure --startup-project src/SpaceShopper.API

# Remove last migration (if not applied in shared environments)
dotnet ef migrations remove --project src/SpaceShopper.Infrastructure --startup-project src/SpaceShopper.API
```

## Current Implementation Notes

To keep this README accurate for evaluators:

- User and authentication flows are implemented and wired through service + repository layers.
- Email delivery is implemented with MailKit and template support.
- Product endpoint currently appears to be a work in progress in `ProductController` (placeholder return value), so catalog behavior should be considered partial at this stage.
- Dev import endpoints are available for seeding/testing and are not intended as production business APIs.
