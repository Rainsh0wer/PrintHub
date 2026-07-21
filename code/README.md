# PrintHub — Multi-Vendor Printing & Fabrication Service Marketplace

Final project for **PRN232**. A marketplace connecting customers with independent
print/fabrication shops: upload once, compare real quotes across nearby shops,
order, and track production to pickup.

See the analysis and design documents in [`../docs`](../docs).

## Solution layout

```
PrintHub.sln
└─ src/
   ├─ PrintHub.Domain          Entities, enums, domain primitives (no dependencies)
   ├─ PrintHub.Contracts       Shared message contracts + gRPC .proto (planned)
   ├─ PrintHub.Application      Use-case services, DTOs, AutoMapper, FluentValidation,
   │                           MediatR pipeline, repository/UoW interfaces
   ├─ PrintHub.Infrastructure   EF Core DbContext + migrations, repository/UoW impl,
   │                           RabbitMQ, gRPC client, external clients, BCrypt
   ├─ PrintHub.Api              ASP.NET Core Web API — controllers, OData, JWT,
   │                           content negotiation, middleware
   ├─ PrintHub.QuoteEngine      gRPC service — computes price/ETA per shop pricing rules
   ├─ PrintHub.ProductionAgent  Worker service — consumes the queue, simulates machines
   ├─ PrintHub.Web              ASP.NET Core MVC + JS — customer web client
   └─ PrintHub.Desktop          WPF (MVVM) — shop counter & platform admin console
```

**Reference direction** (Domain depends on nothing):
`Application → Domain, Contracts` · `Infrastructure → Application, Contracts` ·
`Api → Application, Infrastructure, Contracts` · `QuoteEngine → Contracts, Domain` ·
`ProductionAgent → Contracts` · `Web → Application` · `Desktop → Application`.

## Conventions

- **Target framework:** .NET 8 (LTS). WPF client is `net8.0-windows`.
- **Primary keys:** `int` identity. **Money:** `decimal`. **Timestamps:** `DateTime` UTC.
- **Soft delete:** entities implementing `ISoftDelete`, excluded by a global query filter.
- **Auditing:** `AuditableEntity` (CreatedAt/UpdatedAt) stamped centrally in SaveChanges.
- Common build settings in `Directory.Build.props` (nullable, implicit usings, latest C#).

## Build

```
dotnet build PrintHub.sln
```

## Runtime dependencies (not yet provisioned)

Docker is **not** installed on the dev machine, so the plan's `docker-compose`
is deferred. To run the full system you will need:

- **SQL Server** — LocalDB / SQL Server Express / a container. Connection string
  goes in `PrintHub.Api/appsettings.json`.
- **RabbitMQ** — a local install or container (or CloudAMQP free tier as fallback).

## Status

- [x] Solution scaffolded — 9 projects, references, packages, clean build.
- [x] Domain layer — 21 entities, 20 enums, base/audit/soft-delete primitives.
- [x] Application foundation — Result, PagedResult, Specification, IRepository, IUnitOfWork.
- [x] Infrastructure — DbContext + Fluent config, Repository/UoW/Specification evaluator,
      InitialCreate migration (21 tables, applied to LocalDB), DataSeeder (verified).
- [ ] Application — services, DTOs, validation, AutoMapper, MediatR pipeline.
- [ ] Api — auth, controllers, OData, formatters, middleware.
- [ ] QuoteEngine (gRPC), ProductionAgent (RabbitMQ).
- [ ] Web (MVC), Desktop (WPF).

### Run the seed / verify the database

```
dotnet run --project src/PrintHub.Api -- --seed-only
```

Seeds LocalDB `PrintHubDb` and prints row counts. Sample accounts (password
`Password123!`): `admin@printhub.vn`, `owner.quickprint@printhub.vn`,
`staff.quickprint@printhub.vn`, `customer1@printhub.vn`.
