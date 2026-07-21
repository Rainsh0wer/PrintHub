# PrintHub — Campus Fabrication Job Orchestration System

> **Đề tài tự đề xuất cho Project cuối kỳ PRN232**
> Hệ thống điều phối dịch vụ in 3D / cắt laser cho xưởng chế tạo (FabLab) trong trường đại học.
> Backend ASP.NET Core Web API + Web client (MVC + JS) + WPF Admin Desktop + gRPC Quote Engine + RabbitMQ Machine Agent.

---

## 1. Ý tưởng & lý do chọn đề tài

### 1.1. Bài toán

Trường có một xưởng chế tạo (FabLab) với nhiều máy in 3D / máy cắt laser. Sinh viên muốn in mô hình phải:

1. Upload file thiết kế (.stl / .obj / .3mf) và chọn vật liệu, chất lượng in.
2. Hệ thống **báo giá tự động** (thời gian in, lượng vật liệu, chi phí) qua một Quote Engine.
3. Sinh viên xác nhận, tiền bị **tạm giữ từ ví** nội bộ.
4. Job đi vào **hàng đợi**, được điều phối tới máy phù hợp (máy phải hỗ trợ vật liệu đó).
5. Máy in (giả lập bằng Worker service) nhận job qua **RabbitMQ**, báo tiến độ realtime.
6. In xong → kiểm tra chất lượng → sinh viên nhận hàng → trừ tiền thật, cập nhật tồn kho vật liệu.

Operator (nhân viên xưởng) duyệt job, quản lý máy/vật liệu, xử lý bảo trì. Admin quản lý người dùng, cấu hình giá, xem báo cáo doanh thu & hiệu suất máy.

### 1.2. Tại sao đề tài này "ăn điểm"

| Tiêu chí | PrintHub đáp ứng thế nào |
|---|---|
| Độc lạ | Không trùng 7 đề gợi ý (toàn booking/ticket/library). Ít người nghĩ tới, dễ gây ấn tượng khi demo máy in "chạy" realtime. |
| Đủ độ khó theo Mục 3 đề bài | 3+ role ✅, 9 entity ✅, workflow 10 trạng thái ✅, n-n có thuộc tính ✅ |
| RabbitMQ **có lý do tồn tại thật** | Job in 3D bản chất là hàng đợi bất đồng bộ: máy in chạy hàng giờ, API không thể chờ. Message queue là kiến trúc đúng, không phải gắn vào cho có. |
| gRPC có lý do thật | Quote Engine (tính giá/thời gian in từ metadata file) là service tính toán riêng — đúng use case gRPC internal call. |
| Pipeline logic tự nhiên | Job intake: validate file → check quota → check tồn kho → check capability máy → tính giá → reserve tiền. Mỗi bước là 1 pipeline step (MediatR behaviors / Chain of Responsibility). |
| Production-minded | Ví tiền + reserve/refund, tồn kho vật liệu, state machine chặt chẽ, audit log, Docker compose — các bài toán thật của hệ thống production. |

### 1.3. Hai phương án dự phòng (nếu muốn đổi)

- **LockerLink** — hệ thống tủ khóa thông minh giao nhận đồ nội bộ campus (OTP, compartment, courier role).
- **BidCampus** — sàn đấu giá đồ cũ nội bộ (bid pipeline qua RabbitMQ, anti-sniping, escrow).

Cả hai đều dùng được cùng bộ kỹ thuật, nhưng **PrintHub có câu chuyện queue + pipeline mạnh nhất** → chọn PrintHub.

---

## 2. Vai trò người dùng (3 roles)

| Role | Mô tả | Quyền chính |
|---|---|---|
| **Customer** (sinh viên) | Người đặt in | Đăng ký/đăng nhập, nạp ví, tạo job, xem quote, xác nhận/hủy job, theo dõi tiến độ, xem lịch sử **của mình** |
| **Operator** (nhân viên xưởng) | Vận hành máy | Duyệt/từ chối job, gán máy, xác nhận QC, quản lý máy & vật liệu, ghi maintenance log, xem báo cáo vận hành |
| **Admin** | Quản trị hệ thống | Toàn quyền: quản lý user/role, cấu hình bảng giá, khóa tài khoản, xem toàn bộ báo cáo, audit log |

---

## 3. Thiết kế dữ liệu (9 entities)

```
User ──1:n── RefreshToken
User ──1:n── PrintJob ──1:n── JobStatusHistory
User ──1:n── WalletTransaction
PrintJob ──n:1── Material
PrintJob ──n:1── Machine (nullable, gán khi dispatch)
PrintJob ──1:1── Quote
Machine ──n:n── Material  (qua MachineMaterial — n-n CÓ THUỘC TÍNH)
Machine ──1:n── MaintenanceLog
```

| Entity | Thuộc tính chính | Ghi chú |
|---|---|---|
| **User** | Id, FullName, Email, PasswordHash, Role, WalletBalance, IsLocked, CreatedAt | Password hash bằng BCrypt |
| **RefreshToken** | Token, UserId, ExpiresAt, RevokedAt | Yêu cầu nâng cao: refresh token |
| **Machine** | Id, Name, Type (FDM/Resin/Laser), Status (Idle/Busy/Maintenance/Offline), Location | |
| **Material** | Id, Name, Type, Color, StockGrams, ReservedGrams, UnitCostPerGram, ImportCostUsd | Tồn kho + phần đã reserve |
| **MachineMaterial** | MachineId, MaterialId, **NozzleTempC, MaxSpeedMmS, IsCalibrated** | ⭐ Quan hệ n-n có thuộc tính (capability matrix) — thỏa yêu cầu Mục 4 |
| **PrintJob** | Id, UserId, MaterialId, MachineId?, FileName, FileSizeKb, Quality (Draft/Standard/Fine), Priority (Normal/Rush), Status, ProgressPercent, CreatedAt, CompletedAt | Aggregate trung tâm |
| **Quote** | Id, JobId, EstimatedMinutes, EstimatedGrams, MaterialCost, MachineCost, RushSurcharge, Total, ExpiresAt | Do gRPC Quote Engine sinh ra |
| **JobStatusHistory** | Id, JobId, FromStatus, ToStatus, ActorUserId, Note, Timestamp | Audit workflow |
| **WalletTransaction** | Id, UserId, Type (TopUp/Reserve/Capture/Refund), Amount, JobId?, RefCode, CreatedAt | Sổ cái ví |
| **MaintenanceLog** | Id, MachineId, OperatorId, Description, StartedAt, FinishedAt | |

> ERD chi tiết sẽ vẽ bằng dbdiagram.io / Mermaid trong tài liệu nộp.

### 3.1. Chiến lược database: Code-First + lựa chọn provider

- **Code-First bắt buộc:** toàn bộ schema sinh từ entity + Fluent API, quản lý bằng EF Core Migrations (`dotnet ef migrations add` / `database update`). Không vẽ DB tay. Seed data nằm trong `DataSeeder` chạy lúc khởi động — thỏa luôn yêu cầu "migration hoặc script tạo database" của Mục 6.
- ⚠️ **Lưu ý về Supabase:** đề bài **Mục 6 ghi rõ "phải sử dụng Entity Framework Core với SQL Server"** — Supabase là PostgreSQL, dùng nó là sai yêu cầu bắt buộc, rủi ro mất điểm phần chấm cứng. Kết luận:
  - **Mặc định: SQL Server chạy Docker** (`mcr.microsoft.com/mssql/server`) trong docker-compose — vừa đúng đề, vừa "production-style".
  - Nhờ Repository/UoW + EF Core, connection string và provider tách biệt hoàn toàn khỏi business code; **nếu** giảng viên đồng ý bằng văn bản cho dùng Postgres thì đổi provider (`UseNpgsql`) + regenerate migrations là xong trong ~1 buổi. Chỉ làm khi đã có xác nhận.
  - Nếu muốn DB "trên cloud" để demo cho tiện mà vẫn đúng đề: dùng **Azure SQL free tier** (vẫn là SQL Server) thay vì Supabase.

---

## 4. Workflow trạng thái của PrintJob (yêu cầu Mục 4)

```
                       ┌──────────┐
 Draft ──submit──▶ Submitted ──quote(gRPC)──▶ Quoted ──customer confirm──▶ PendingApproval
                       │                        │ (quote hết hạn 24h → Expired)
                       ▼                        ▼
                   Cancelled ◀────────────── Cancelled

 PendingApproval ──operator approve──▶ Queued ──dispatcher gán máy──▶ Printing
        │                                                    │
        └──reject──▶ Rejected (refund reserve)               ├──progress events (RabbitMQ)
                                                             ▼
                                            Failed (refund) ◀─┴─▶ PostProcessing ──▶ ReadyForPickup
                                                                                        │
                                                                          customer nhận hàng
                                                                                        ▼
                                                                                   Completed (capture tiền, trừ kho)
```

**Trạng thái:** `Draft → Submitted → Quoted → PendingApproval → Queued → Printing → PostProcessing → ReadyForPickup → Completed` + nhánh `Rejected / Cancelled / Failed / Expired`.

**Ai được chuyển trạng thái nào** (một phần của security matrix):

| Transition | Customer | Operator | Admin | Hệ thống (worker) |
|---|---|---|---|---|
| submit, confirm quote, cancel (trước Queued), nhận hàng | ✅ | | ✅ | |
| approve / reject / gán máy / QC pass–fail | | ✅ | ✅ | |
| Printing progress, Failed do máy | | | | ✅ (RabbitMQ event) |

Triển khai bằng **State pattern / state machine table**: dictionary `(FromStatus, Action, Role) → ToStatus`, chuyển trạng thái sai → 400/403, mọi transition ghi vào `JobStatusHistory`.

---

## 5. Business rules (7 rules — vượt mức tối thiểu 5)

1. **Capability rule:** Job chỉ được gán vào máy có `MachineMaterial(machine, material)` tồn tại và `IsCalibrated = true`.
2. **Stock rule:** Khi Operator approve, hệ thống reserve `EstimatedGrams` vào `Material.ReservedGrams`; nếu `StockGrams - ReservedGrams < EstimatedGrams` → từ chối với lỗi nghiệp vụ rõ ràng. Trừ kho thật khi Completed, hoàn reserve khi Failed/Cancelled.
3. **Wallet rule:** Khi customer confirm quote, ví phải đủ `Quote.Total`; tiền bị **Reserve** (tạm giữ), **Capture** khi Completed, **Refund** khi Rejected/Failed/Cancelled hợp lệ.
4. **Quota rule:** Mỗi Customer tối đa **3 job active** (từ Submitted đến trước Completed) cùng lúc; Rush job tối đa 1.
5. **Quote expiry rule:** Quote hết hạn sau **24h** — confirm sau đó phải re-quote (giá vật liệu có thể đã đổi theo tỉ giá).
6. **Cancel rule:** Customer chỉ được cancel khi job **chưa vào Printing**; cancel ở Queued mất phí 10% (phí giữ chỗ), trước đó hoàn 100%.
7. **Maintenance rule:** Máy chuyển sang Maintenance thì job đang Queued trên máy đó tự động **re-queue** sang máy khác đủ capability; nếu không còn máy nào → notify Operator.

---

## 6. Kiến trúc hệ thống

### 6.1. Sơ đồ tổng thể

```
┌─────────────────┐        ┌──────────────────┐
│ PrintHub.Web    │        │ PrintHub.Desktop │
│ (MVC + JS fetch)│        │ (WPF - MVVM)     │
│ → Customer      │        │ → Admin/Operator │
└───────┬─────────┘        └────────┬─────────┘
        │  HTTPS + JWT Bearer       │
        ▼                           ▼
┌───────────────────────────────────────────────┐     gRPC      ┌─────────────────────┐
│         PrintHub.Api (ASP.NET Core)           │──────────────▶│ PrintHub.QuoteEngine │
│  Controllers · OData · Formatters · SignalR   │               │ (gRPC service)       │
└──────┬──────────────────────────┬─────────────┘               └─────────────────────┘
       │ EF Core                  │ publish/consume
       ▼                          ▼
┌─────────────┐          ┌─────────────────┐    consume    ┌──────────────────────┐
│ SQL Server  │          │    RabbitMQ     │◀─────────────▶│ PrintHub.MachineAgent │
│  (Docker)   │          │    (Docker)     │  jobs/progress│ (Worker giả lập máy) │
└─────────────┘          └─────────────────┘               └──────────────────────┘
                                                External APIs: ExchangeRate-API, VietQR
```

### 6.2. Solution structure (Clean-ish Architecture)

```
PrintHub.sln
├─ src/
│  ├─ PrintHub.Domain          → Entities, Enums, domain exceptions, state machine table
│  ├─ PrintHub.Application     → Services (use cases), DTOs, AutoMapper profiles,
│  │                             FluentValidation, MediatR pipeline behaviors,
│  │                             interface IRepository/IUnitOfWork/IEventBus/IQuoteClient
│  ├─ PrintHub.Infrastructure  → DbContext, Migrations, Repository + UnitOfWork impl,
│  │                             RabbitMQ publisher/consumer, gRPC client,
│  │                             ExchangeRate + VietQR client, BCrypt, Serilog sinks
│  ├─ PrintHub.Api             → Controllers, OData config, CsvOutputFormatter,
│  │                             JWT config, SignalR hub, middleware, Program.cs
│  ├─ PrintHub.QuoteEngine     → gRPC service độc lập (proto: EstimateJob)
│  ├─ PrintHub.MachineAgent    → .NET Worker Service, consume queue, giả lập in, publish progress
│  ├─ PrintHub.Web             → ASP.NET Core MVC client (Customer) + JS fetch gọi API
│  ├─ PrintHub.Desktop         → WPF (MVVM, CommunityToolkit.Mvvm, LiveCharts2)
│  └─ PrintHub.Contracts       → Message contracts (RabbitMQ events), proto files dùng chung
├─ docs/                        → Tài liệu nộp (ERD, security matrix, API list, hướng dẫn chạy)
├─ postman/PrintHub.postman_collection.json
├─ docker-compose.yml           → SQL Server + RabbitMQ (management UI)
└─ README.md
```

### 6.3. Service layer design (nộp kèm theo yêu cầu Mục 5)

| Service | Trách nhiệm |
|---|---|
| `AuthService` | Register, login, issue JWT + refresh token, revoke |
| `UserService` | CRUD user (Admin), profile, khóa/mở tài khoản |
| `WalletService` | TopUp (sinh VietQR), Reserve/Capture/Refund, sao kê |
| `JobIntakeService` | **Pipeline** tạo job: validate file → quota → gọi QuoteEngine → tạo Quote |
| `JobWorkflowService` | Mọi transition trạng thái, enforce state machine + business rules |
| `DispatchService` | Chọn máy phù hợp (capability + Idle), publish message dispatch |
| `MachineService` | CRUD máy, maintenance mode, re-queue job khi bảo trì |
| `MaterialService` | CRUD vật liệu, tồn kho, cập nhật giá theo tỉ giá (external API) |
| `ReportService` | Thống kê: doanh thu, hiệu suất máy, tiêu hao vật liệu, tỉ lệ fail |
| `NotificationService` | Consume event → SignalR push + email giả lập (log) |

---

## 7. Mapping kỹ thuật đề bài → nơi sử dụng (checklist chấm điểm)

| Yêu cầu đề bài | Triển khai cụ thể trong PrintHub |
|---|---|
| **RESTful Web API** | Toàn bộ `PrintHub.Api`, đúng verb/status code, resource-based routes |
| **OData (≥2 API)** | `/odata/Jobs`, `/odata/Machines`, `/odata/Materials` — hỗ trợ `$filter, $orderby, $top, $skip, $select, $expand` (vd: `$expand=Quote`). Customer bị inject filter `userId eq me` ở server |
| **Media Formatter / Content negotiation** | JSON (mặc định) + XML (`AddXmlSerializerFormatters`) + ⭐ **custom `CsvOutputFormatter`** cho `/api/reports/*` với `Accept: text/csv` — điểm cộng vì tự viết formatter. Dùng `[Produces]`/`[Consumes]` trên controller |
| **JWT Authentication** | Access token 15' + refresh token 7 ngày, role claim, `[Authorize(Roles=...)]`, policy `OwnDataOnly` |
| **AutoMapper** | Profiles trong Application layer: Entity ↔ DTO, có custom resolver (vd: tính `RemainingStock`) |
| **Repository + UnitOfWork** | Generic `IRepository<T>` + repo đặc thù (`IJobRepository` với query phức tạp), `IUnitOfWork.SaveChangesAsync` bọc transaction cho nghiệp vụ reserve tiền + reserve kho (atomic) |
| **MVC** | `PrintHub.Web` là ASP.NET Core MVC; JS fetch trong views gọi API với Bearer token (thỏa yêu cầu "JavaScript client" của đề) |
| **WPF Desktop** | `PrintHub.Desktop` cho Admin/Operator: dashboard máy realtime, duyệt job, quản lý kho, chart báo cáo — pattern MVVM |
| **gRPC/Microservice (Mục 12)** | ⭐ Cả hai: `QuoteEngine` là **gRPC service** API gọi sang; `MachineAgent` + `NotificationService` là **microservice simulation** qua RabbitMQ |
| **Design patterns** | Repository, Unit of Work, **State** (workflow), **Strategy** (pricing theo Quality/Priority), **Chain of Responsibility / Pipeline** (MediatR behaviors: Validation → Logging → Transaction), **Publisher–Subscriber** (RabbitMQ), **Factory** (notification channel), Options pattern, Result pattern, MVVM |
| **Logic pipeline (điểm đột phá)** | MediatR `IPipelineBehavior`: `ValidationBehavior` (FluentValidation) → `LoggingBehavior` (Serilog) → `TransactionBehavior` (UoW) bọc quanh mọi command; cộng thêm job-intake pipeline nghiệp vụ |
| **RabbitMQ (điểm đột phá)** | Exchange topic `printhub.events`: routing `job.dispatch` (API→Agent), `job.progress` / `job.completed` / `job.failed` (Agent→API), `notify.*` (fan-out). Có retry + dead-letter queue để nói chuyện "production" |
| **External API (điểm đột phá)** | 1) **ExchangeRate-API** (free): cập nhật `ImportCostUsd → VND` cho giá vật liệu, cache 12h. 2) **VietQR.io quicklink**: sinh ảnh QR nạp ví (demo được thật, không cần merchant) |
| **Yêu cầu nâng cao Mục 15** | Refresh token ✅, Audit log (`JobStatusHistory` + Serilog) ✅, Soft delete (User, Machine, Material) ✅, Pagination chuẩn (`PagedResult<T>`) ✅, Global exception middleware + `ApiResponse<T>` wrapper ✅, FluentValidation ✅, SignalR notify ✅, Dashboard ✅, Docker compose ✅, Serilog ✅ |

---

## 8. API endpoint chính (rút gọn — bản đầy đủ viết trong docs khi code)

```
# Auth
POST   /api/auth/register            (anon)
POST   /api/auth/login               (anon)
POST   /api/auth/refresh             (anon + refresh token)

# Jobs (workflow)
POST   /api/jobs                     Customer   — tạo job (upload metadata) → pipeline → Quoted
POST   /api/jobs/{id}/confirm        Customer   — chấp nhận quote, reserve tiền
PUT    /api/jobs/{id}/approve        Operator
PUT    /api/jobs/{id}/reject         Operator
PUT    /api/jobs/{id}/cancel         Customer (rule 6)
PUT    /api/jobs/{id}/qc             Operator   — pass/fail
PUT    /api/jobs/{id}/pickup         Customer   — Completed, capture tiền
GET    /api/jobs/{id}/history        Owner/Staff

# OData
GET    /odata/Jobs?$filter=status eq 'Printing'&$orderby=createdAt desc&$top=10
GET    /odata/Machines?$filter=status eq 'Idle'&$select=name,type
GET    /odata/Materials?$expand=Machines

# Wallet
POST   /api/wallet/topup             Customer — trả về ảnh VietQR + RefCode
POST   /api/wallet/topup/confirm     Admin    — mock xác nhận đã chuyển khoản
GET    /api/wallet/transactions      Owner

# Machines / Materials (Operator/Admin)
CRUD   /api/machines, /api/materials
PUT    /api/machines/{id}/maintenance

# Reports (Operator/Admin) — hỗ trợ Accept: application/json | application/xml | text/csv
GET    /api/reports/revenue?from=&to=&groupBy=material
GET    /api/reports/machine-utilization
GET    /api/reports/failure-rate
GET    /api/reports/top-customers
```

### Security matrix (rút gọn)

| Chức năng | Admin | Operator | Customer |
|---|---|---|---|
| Quản lý user | ✅ | ❌ | ❌ |
| Quản lý máy / vật liệu | ✅ | ✅ | ❌ |
| Duyệt / gán / QC job | ✅ | ✅ | ❌ |
| Tạo job, confirm, cancel, pickup | ✅ | ✅ | ✅ (job của mình) |
| Xem báo cáo | ✅ | ✅ | ❌ |
| Xem job / ví / lịch sử | ✅ tất cả | ✅ tất cả | ✅ của mình |

---

## 9. Chia Phase & kế hoạch chi tiết

> Nguyên tắc theo Mục 15 đề bài: **hoàn thành phần bắt buộc trước, đột phá sau**. Mỗi phase kết thúc đều có sản phẩm chạy được (vertical slice), không để dồn tích hợp về cuối.
> Ước lượng cho 1 người, ~3-4h/ngày. Tổng ≈ 5 tuần + 1 tuần buffer.

### Phase 0 — Khởi tạo & thiết kế trên giấy (2 ngày)
- [ ] Vẽ ERD (dbdiagram.io), state diagram, kiến trúc — chốt trước khi code.
- [ ] Tạo solution 8 project + reference đúng chiều (Domain không tham chiếu ai).
- [ ] `docker-compose.yml`: SQL Server + RabbitMQ (kèm management UI :15672).
- [ ] Git init, `.gitignore`, README khung.
- **Định nghĩa xong:** tài liệu thiết kế bản nháp đã có ERD + workflow + security matrix.

### Phase 1 — Domain + Database + Seed (4 ngày) — Code-First
- [ ] 9 entities + enums + Fluent API config (quan hệ, index, soft-delete query filter) — schema 100% sinh từ code.
- [ ] Migration đầu tiên (EF Core Migrations, SQL Server) + `DataSeeder`: 1 Admin, 2 Operator, 5 Customer, 4 máy, 6 vật liệu, capability matrix, 15 job rải đủ các trạng thái (để demo & báo cáo có số liệu).
- [ ] Generic Repository + UnitOfWork + repo đặc thù.
- **Định nghĩa xong:** `dotnet ef database update` ra DB đầy đủ dữ liệu demo.

### Phase 2 — Auth + phân quyền + khung API (4 ngày)
- [ ] JWT (access + refresh + revoke), BCrypt, role claims.
- [ ] Global exception middleware, `ApiResponse<T>`, `PagedResult<T>`, Serilog.
- [ ] AutoMapper profiles + FluentValidation + MediatR pipeline behaviors (Validation/Logging/Transaction) — **dựng pipeline ngay từ đây** để mọi feature sau hưởng chung.
- [ ] `AuthController`, `UsersController`; policy "own data only".
- **Định nghĩa xong:** login 3 role bằng Swagger, gọi API bị chặn đúng 401/403.

### Phase 3 — Core nghiệp vụ: Job workflow + Wallet + OData + Formatter (7 ngày) ⭐ phase nặng nhất
- [ ] State machine table + `JobWorkflowService` + `JobStatusHistory`.
- [ ] `JobIntakeService` pipeline (rule 1,2,4,5) — tạm dùng **quote giả** (chưa cần gRPC).
- [ ] `WalletService` reserve/capture/refund trong transaction (rule 3,6).
- [ ] CRUD Machines/Materials + maintenance re-queue (rule 7).
- [ ] OData controllers (Jobs/Machines/Materials) + inject owner filter.
- [ ] XML formatter + custom `CsvOutputFormatter` + `[Produces]`; `ReportService` với 4 báo cáo.
- **Định nghĩa xong:** chạy trọn vòng đời job bằng Postman từ tạo → completed, tiền & kho đúng từng xu/gram; demo được OData + JSON/XML/CSV. → **Đến đây đã đạt chuẩn "pass" của đề bài (trừ client + gRPC).**

### Phase 4 — Đột phá: gRPC Quote Engine + RabbitMQ Machine Agent (6 ngày)
- [ ] Proto `EstimateJob(fileMeta, material, quality, priority) → (minutes, grams, breakdown)`; QuoteEngine dùng Strategy pattern cho pricing. Thay quote giả ở Phase 3 bằng gRPC call (có fallback khi engine chết → 503 + message rõ).
- [ ] RabbitMQ: publisher trong Infrastructure; `MachineAgent` consume `job.dispatch`, giả lập in (tick progress 5%/2s, 10% xác suất fail), publish `job.progress|completed|failed`; API consume và cập nhật DB + đẩy SignalR.
- [ ] Dead-letter queue + retry đơn giản (nói được chuyện production khi vấn đáp).
- [ ] External APIs: ExchangeRate-API (background job cập nhật giá 12h/lần, cache) + VietQR quicklink cho topup.
- **Định nghĩa xong:** approve job trên Swagger → thấy MachineAgent log tiến độ → job tự chuyển PostProcessing; rút mạng QuoteEngine vẫn không sập API.

### Phase 5 — Clients: Web MVC (Customer) + WPF (Admin/Operator) (7 ngày)
- [ ] **PrintHub.Web (4 ngày):** login lưu JWT (localStorage), trang tạo job + xem quote + confirm, danh sách job của tôi (progress bar realtime qua SignalR), ví + QR nạp tiền, xử lý lỗi 400/401/403/404 hiển thị thân thiện. Bootstrap là đủ, không cầu kỳ UI.
- [ ] **PrintHub.Desktop (3 ngày):** MVVM; màn hình: login, dashboard máy (trạng thái + job đang in, refresh realtime), hàng đợi duyệt job (approve/reject), quản lý vật liệu (cảnh báo tồn kho thấp), tab báo cáo với LiveCharts2.
- **Định nghĩa xong:** demo end-to-end 2 màn hình song song: Customer tạo job trên web → Operator approve trên WPF → cả hai cùng thấy tiến độ chạy.

### Phase 6 — Tài liệu + đóng gói + tổng duyệt demo (4 ngày)
- [ ] Tài liệu nộp đầy đủ theo Mục 13: giới thiệu, use case, ERD, business rules, workflow, kiến trúc, service design, API list, security matrix, OData demo, content-negotiation demo, gRPC/RabbitMQ demo, hướng dẫn chạy.
- [ ] Postman collection đầy đủ (có script tự lưu token).
- [ ] README: bảng **tài khoản mẫu 3 role**, các bước chạy: `docker compose up` → `ef database update` → chạy 4 process (Api, QuoteEngine, MachineAgent, Web) → mở WPF.
- [ ] **Kịch bản demo 10 phút** (viết sẵn, tập 2 lần): login 3 role → tạo job → OData → XML/CSV → approve → RabbitMQ UI cho thấy message → progress realtime → báo cáo → tắt QuoteEngine cho thấy graceful degradation.
- [ ] Tổng duyệt trên máy sạch (clone về chạy lại từ đầu).

### Buffer (1 tuần)
Dành cho trượt tiến độ, fix bug, hoặc nâng cấp thêm nếu dư thời gian (thứ tự ưu tiên: audit log viewer trong WPF → email giả lập → Dockerfile cho từng service).

---

## 10. Chiến lược cắt giảm khi thiếu thời gian (thứ tự hy sinh)

1. WPF thu gọn còn 2 màn hình (dashboard + duyệt job) — web đã đủ thỏa yêu cầu client.
2. Bỏ SignalR → web polling 3s (vẫn demo được progress).
3. VietQR bỏ, giữ ExchangeRate (chỉ cần 1 external API để "đột phá").
4. Dead-letter/retry bỏ, giữ happy-path RabbitMQ.
5. **Không bao giờ cắt:** workflow + business rules + OData + XML + JWT 3 role + gRPC tối thiểu + tài liệu — đây là phần chấm cứng của đề.

## 11. Rủi ro & cách né

| Rủi ro | Phòng ngừa |
|---|---|
| RabbitMQ/Docker lằng nhằng trên Windows | Dựng ngay Phase 0; nếu bí, fallback RabbitMQ → cloudamqp free tier |
| Ôm đồm UI đẹp | Đề nói rõ "client không cần giao diện phức tạp" — Bootstrap mặc định, dồn thời gian cho backend |
| Reserve tiền/kho bị race condition | Mọi thao tác ví + kho đi qua TransactionBehavior (UoW), có unique constraint RefCode |
| Quote Engine chết khi demo | Fallback message rõ ràng + demo luôn tình huống này thành "điểm cộng resilience" |
| Không giải thích được code khi vấn đáp | Cuối mỗi phase viết 5 dòng "quyết định thiết kế & lý do" vào docs — vừa là tài liệu nộp vừa là đề cương trả lời |

---

## 12. Việc cần làm ngay (next actions)

1. Chốt đề tài với giảng viên (đề tự đề xuất cần được chấp nhận — gửi mô tả Mục 1-5 của file này).
2. Chạy Phase 0: vẽ ERD + dựng solution + docker-compose.
3. Bắt đầu Phase 1.
