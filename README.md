# PrintHub — Multi-Vendor Printing & Fabrication Marketplace

PrintHub is a web-based marketplace that connects customers with local print &
fabrication shops. A customer configures a job once (document printing, binding,
3D printing, laser cutting…), **compares real quotes across eligible shops**, pays
from a wallet, and tracks the order through production to hand-over. Shops manage
their rate card, machines, staff and order queue; administrators approve shops,
adjudicate disputes and run platform reports.

Built as the **PRN232 (.NET Back-End)** individual final project. The emphasis is a
clean, layered **ASP.NET Core** back-end that showcases the required technology
stack: **gRPC, OData, RabbitMQ (async messaging), and content negotiation**, on top of
EF Core code-first, JWT auth, and a set of classic design patterns.

---

## Architecture

API-first: all business logic lives in the back-end; any client (web, desktop) is
a thin layer over the same HTTP API — one source of truth, no duplicated rules.

```
                       ┌─────────────────────────────┐
   Clients  ─────────▶ │  PrintHub.Api (REST + OData) │
   (web / WPF / Swagger)└──────────────┬──────────────┘
                                        │
             ┌──────────────────────────┼───────────────────────────┐
             ▼                          ▼                            ▼
   PrintHub.Application        PrintHub.Infrastructure      Microservices
   (use cases, services,   (EF Core, repositories, JWT,   ┌───────────────────────┐
    specs, validators,      RabbitMQ publisher,           │ QuoteEngine  (gRPC)   │
    Result pattern)         local file storage)           │ ProductionAgent(worker│
             │                          │                 │  ← RabbitMQ consumer) │
             └────────── Domain ────────┘                 └───────────────────────┘
             (entities, enums, base types)                          │
                                        │                           ▼
                              SQL Server (PrintHubDb)          RabbitMQ broker
```

### Layers & patterns
- **Domain** — 22 entities, enums, base types (auditable / soft-delete).
- **Application** — use-case **services returning `Result<T>`** (no MediatR),
  **Specification** pattern for all queries, FluentValidation, AutoMapper.
- **Infrastructure** — EF Core (code-first), **Repository + Unit of Work**,
  JWT/BCrypt, RabbitMQ publisher, local file storage.
- **Api** — thin controllers, `ApiResponse` envelope, global exception middleware,
  JWT bearer + **scoped shop authorization**, OData, content-negotiation formatters.
- Patterns used: Repository, Unit of Work, Specification, **Strategy** (pricing in
  the Quote Engine), Result, Options, and a **table-driven order state machine**.

### The four required technologies
| Requirement | Where |
|-------------|-------|
| **gRPC** | `PrintHub.QuoteEngine` computes prices over gRPC (h2c :5090); the API is the client. |
| **OData** | `/odata/Shops` and `/odata/Orders` (with `$expand`) — `$filter/$orderby/$select/$top/$skip/$count`. |
| **RabbitMQ (async)** | On *start production* the API publishes a job; `PrintHub.ProductionAgent` consumes it and drives the order to *ready* — degrades gracefully if the broker is down. |
| **Content negotiation** | Report endpoints return **JSON / XML / CSV** from one action by `Accept` header. |

---

## Solution layout (`code/PrintHub.sln`)

| Project | Purpose |
|---------|---------|
| `PrintHub.Domain` | Entities, enums, base types |
| `PrintHub.Application` | Use-case services, DTOs, specs, validators, mapping |
| `PrintHub.Infrastructure` | EF Core, repositories/UoW, security, messaging, storage |
| `PrintHub.Contracts` | Shared gRPC `.proto` + messaging contracts |
| `PrintHub.Api` | REST + OData Web API (the host) |
| `PrintHub.QuoteEngine` | gRPC pricing microservice (Strategy pattern) |
| `PrintHub.ProductionAgent` | RabbitMQ consumer worker (async production) |
| `PrintHub.Web` / `PrintHub.Desktop` | Client shells (scaffolded) |

---

## Features (by role)

- **Guest** — browse/search shops, view shop detail & reviews, register/login,
  forgot/reset password.
- **Customer** — profile, document library (upload), **compare quotes**, apply
  vouchers, **place orders** (wallet payment), track/cancel/confirm, wallet
  top-up, reviews, complaints, notifications.
- **Shop Owner / Staff** — onboarding, storefront profile, rate card & pricing
  rules, machines & materials, staff, **order queue** (accept/decline/produce/
  hand-over), respond to complaints, revenue report.
- **Admin** — approve/suspend shops, manage users (lock/unlock), service-type
  catalogue, vouchers, **commission rate**, adjudicate complaints, platform report.

The complete endpoint list (≈70 endpoints across 42 use cases) is in
[`docs/api-endpoints-full.md`](docs/api-endpoints-full.md).

---

## Getting started

### Prerequisites
- **.NET 8 SDK**
- **SQL Server LocalDB** (ships with Visual Studio) — connection string in
  `code/src/PrintHub.Api/appsettings.json`
- **RabbitMQ** *(optional)* — only needed to see async production live; the app
  runs fine without it (production can be driven manually).

### 1. Database
Code-first — the API applies migrations and seeds demo data on first run. To seed
explicitly:
```bat
cd code
dotnet run --project src/PrintHub.Api -- --seed-only
```
A plain SQL schema script is also provided in [`database/`](database/) for creating
the schema directly in SQL Server.

### 2. Run the API
```bat
cd code
dotnet run --project src/PrintHub.Api --urls http://localhost:5080
```
Swagger UI: <http://localhost:5080/swagger>

### 3. Quote comparison (gRPC) — run alongside the API
```bat
dotnet run --project src/PrintHub.QuoteEngine
```
(listens on :5090; the API falls back to an indicative price if it is down.)

### 4. Async production (RabbitMQ) — optional
Start the broker, then the agent:
```bat
rem broker (foreground launcher, no admin needed):
"D:\RabbitMQ\rabbitmq_server-4.3.1\sbin\rabbitmq-server.bat"
rem agent:
dotnet run --project src/PrintHub.ProductionAgent
```

### Demo accounts (password `Password123!`)
| Email | Role |
|-------|------|
| admin@printhub.vn | Admin |
| owner.quickprint@printhub.vn | Shop Owner |
| staff.quickprint@printhub.vn | Shop Staff |
| customer1@printhub.vn / customer2 / customer3 | Customer |

---

## Repository layout

```
Project Final PRN/
├─ code/            ASP.NET Core solution (PrintHub.sln)
├─ database/        SQL schema script (code-first, provided for convenience)
├─ docs/            analysis & design docs, diagrams, full API list
└─ README.md
```

## Tech stack
.NET 8 · ASP.NET Core · EF Core 8 (code-first) · SQL Server LocalDB · gRPC ·
RabbitMQ · Microsoft.AspNetCore.OData · AutoMapper · FluentValidation · BCrypt ·
JWT.
