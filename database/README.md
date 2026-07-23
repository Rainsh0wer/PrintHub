# Database

PrintHub is **EF Core code-first** — the schema is defined in C# and created by
migrations. This folder provides a plain SQL script so the schema can be created
and inspected directly in Microsoft SQL Server, without running the application
(handy for a quick walkthrough).

## Files

| File | What it does |
|------|--------------|
| `PrintHub_CreateSchema.sql` | Creates the `PrintHubDb` database and every table, primary key, unique index, and foreign key. Idempotent (safe to re-run). |

## How to run

**SSMS / Azure Data Studio:** open `PrintHub_CreateSchema.sql` and click *Execute*.

**Command line:**
```bat
sqlcmd -S (localdb)\MSSQLLocalDB -i PrintHub_CreateSchema.sql
```
(replace the server with `.` or your instance name if you use full SQL Server).

## Sample data

The schema script creates **empty** tables. Demo data (accounts, shops, service
types, rate cards, machines, materials, sample orders/reviews/vouchers) is seeded
by the application. From the `code` folder:

```bat
dotnet run --project src/PrintHub.Api -- --seed-only
```

Or just run the API normally — it applies migrations and seeds on first start.

**Demo accounts** (password `Password123!` for all):

| Email | Role |
|-------|------|
| admin@printhub.vn | Admin |
| owner.quickprint@printhub.vn | Shop Owner |
| staff.quickprint@printhub.vn | Shop Staff |
| customer1@printhub.vn | Customer |

## How the script was generated

```bat
dotnet ef migrations script --idempotent ^
  --project src/PrintHub.Infrastructure --startup-project src/PrintHub.Api ^
  --output ../database/PrintHub_CreateSchema.sql
```
Re-run this after adding a migration to refresh the script.
