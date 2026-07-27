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
- **Domain** — 23 entities, enums, base types (auditable / soft-delete).
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
| Tool | Required? | Notes |
|---|---|---|
| **.NET 8 SDK** | yes | `dotnet --version` should print 8.x |
| **SQL Server LocalDB** | yes | ships with Visual Studio. Connection string in `code/src/PrintHub.Api/appsettings.json` |
| **dotnet-ef tool** | only for migrations | `dotnet tool install --global dotnet-ef` |
| **RabbitMQ** | optional | only to watch async production run itself; everything works without it |
| **Node.js** | optional | only if you want to run the Postman collection headlessly via `newman` |

### Processes and ports at a glance
| Process | Command | Port | Needed for |
|---|---|---|---|
| **API** | `dotnet run --project src/PrintHub.Api --urls http://localhost:5080` | 5080 | everything |
| **Quote Engine** (gRPC) | `dotnet run --project src/PrintHub.QuoteEngine` | 5090 | quote comparison — without it quotes are *indicative* and **orders are blocked** |
| **Web UI** | `dotnet run --project src/PrintHub.Web --urls http://localhost:5100` | 5100 | the browser client (all roles) |
| **Desktop admin** | `dotnet run --project src/PrintHub.Desktop` | — | WPF admin console (Windows only) |
| **Production Agent** | `dotnet run --project src/PrintHub.ProductionAgent` | — | auto-advances orders after production (needs RabbitMQ) |
| RabbitMQ broker | see step 5 | 5672 / 15672 | required by the Production Agent only |

Each is a **separate process** — open one terminal per process, all from the `code/` folder.

### 1. Database
Code-first. The API **applies migrations and seeds demo data automatically on first
run**, so usually you do not need to do anything here.

```bat
cd code
dotnet run --project src/PrintHub.Api -- --seed-only    :: seed without starting the server
```

Reset to a clean demo dataset at any time:
```bat
dotnet ef database drop -f --project src/PrintHub.Infrastructure --startup-project src/PrintHub.Api
dotnet run --project src/PrintHub.Api -- --seed-only
```

A plain SQL script is also provided in [`database/`](database/) if you prefer to create
the schema directly in SQL Server without .NET.

### 2. Secrets (optional — everything degrades gracefully without them)
Copy `code/src/PrintHub.Api/.env.example.json` to `.env.json` (gitignored) and fill in
what you have. Leave a section blank to keep that feature off:

| Section | Effect when filled | Effect when blank |
|---|---|---|
| `Cloudinary` | uploads go to Cloudinary, returns a CDN URL | uploads saved to local disk |
| `Email` | in-app notifications are also emailed (SMTP) | no email is sent |
| `Authentication:Google` | "Continue with Google" works on the web login | the button returns a clear "not configured" message |
| `Jwt:Key` | overrides the dev signing key | dev key from `appsettings.json` is used |

For Google, register `http://localhost:5080/signin-google` as an authorised redirect URI.

### 3. Minimum to run (API + Quote Engine)
Two terminals, both from `code/`:
```bat
:: terminal 1
dotnet run --project src/PrintHub.QuoteEngine

:: terminal 2
dotnet run --project src/PrintHub.Api --urls http://localhost:5080
```
Swagger UI: <http://localhost:5080/swagger>

> The Quote Engine is not optional for the ordering flow: with it down, the API still
> answers but marks quotes `isIndicative: true`, and placing an order from an
> indicative quote is rejected by design.

### 4. Web UI
```bat
dotnet run --project src/PrintHub.Web --urls http://localhost:5100
```
Then open <http://localhost:5100>. The web app is a **thin client** — it calls the API
over HTTP, so the API must already be running.

### 5. Async production (RabbitMQ) — optional
```bat
:: broker (foreground launcher, no admin rights needed)
"D:\RabbitMQ\rabbitmq_server-4.3.1\sbin\rabbitmq-server.bat"

:: agent, in another terminal
dotnet run --project src/PrintHub.ProductionAgent
```
With both running, an order moves `InProduction → ReadyForPickup` on its own about 4
seconds after the shop starts production, recorded in the history with `System` as the
actor. Without them the API is unaffected — the shop just marks the order ready manually.

Broker management UI: <http://localhost:15672> (guest / guest).

### 6. Desktop admin console (WPF, Windows only)
```bat
dotnet run --project src/PrintHub.Desktop
```
Sign in with the admin account below; it rejects non-admin roles. Requires the API to
be running.

### 7. Try the API without writing requests
Import [`postman/PrintHub.postman_collection.json`](postman/) into Postman. Run the
**"0. Login"** folder once — all four role tokens are captured automatically — then walk
the **"⭐ LUỒNG CHÍNH"** folder top to bottom for a complete order lifecycle across
customer, shop staff and admin. 99 requests, all 42 use cases, plus folders for the
technology showcase and negative/security cases.

### Troubleshooting
| Symptom | Cause / fix |
|---|---|
| `MSB3021` / `MSB3027` — file in use during build | a running process is holding the DLL. `taskkill /F /IM PrintHub.Api.exe /IM PrintHub.Web.exe /IM PrintHub.QuoteEngine.exe /IM PrintHub.ProductionAgent.exe`, then rebuild |
| Placing an order redirects back / returns 409 | quote is `indicative` → the Quote Engine is not running (step 3) |
| `dotnet ef database update` says "no migrations applied" | stale assembly — build first, then re-run with `--no-build` |
| Web pages error on every request | the API is not running, or `Api:BaseUrl` in the Web `appsettings.json` points elsewhere |
| Agent logs "RabbitMQ not reachable" | broker not started; harmless — the agent retries every 5 s |

### Demo accounts (password `Password123!`)
| Email | Role |
|-------|------|
| admin@printhub.vn | Admin |
| owner.quickprint@ / owner.campuscopy@ / owner.makerlab@ / owner.printcorner@ / owner.sinhvien@ / owner.colorzone@ / owner.binderpro@ / owner.rainbow@ / owner.photoexpress@printhub.vn | Shop Owner |
| staff.quickprint@ / staff.campuscopy@ / staff.sinhvien@ / staff.colorzone@ / staff.binderpro@printhub.vn | Shop Staff |
| customer1@ .. customer12@printhub.vn | Customer |

The seed creates 9 shops (7 active, 1 pending review, 1 suspended — useful for
exercising the admin approval/governance screens), ~40 orders spanning every
lifecycle status, wallet ledgers, reviews, documents, and vouchers.

---

## Repository layout

```
Project Final PRN/
├─ code/            ASP.NET Core solution (PrintHub.sln) — 9 projects
├─ database/        SQL schema script (code-first, provided for convenience)
├─ docs/            analysis & design docs, diagrams, business rules, full API list
├─ postman/         importable Postman collection (99 requests, all 42 use cases)
└─ README.md
```

Key documents:
- [`docs/PrintHub_TongQuan_NghiepVu.md`](docs/PrintHub_TongQuan_NghiepVu.md) — project
  overview, the problem it solves, where the business rules come from, and the full
  list of 114 business rules *(Vietnamese)*
- [`docs/api-endpoints-full.md`](docs/api-endpoints-full.md) — complete endpoint list
- [`docs/2_SRS_Part1/2/3`](docs/) — full SRS (context, 42 use cases, requirements)

## Tech stack
.NET 8 · ASP.NET Core · EF Core 8 (code-first) · SQL Server LocalDB · gRPC ·
RabbitMQ · Microsoft.AspNetCore.OData · AutoMapper · FluentValidation · BCrypt ·
JWT.
