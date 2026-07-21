# -*- coding: utf-8 -*-
"""PrintHub content for the PRN232 Final Project Document, section-by-section,
matching the reference template's structure and brevity."""


def build_content(b):
    # ===================================================================== #
    # 1. Project Introduction
    # ===================================================================== #
    b.heading("1. Project Introduction", 1)
    b.heading("1.1. Project Name", 2)
    b.para("PrintHub — Multi-Vendor Printing & Fabrication Service Marketplace")

    b.heading("1.2. Objectives", 2)
    b.para(
        "PrintHub is a web-based marketplace that connects customers who need "
        "printing and fabrication services with independent print shops in their "
        "area. Instead of sending files through chat applications and waiting for a "
        "manually calculated price, a customer configures an order once and "
        "immediately receives comparable quotes from multiple nearby shops — price, "
        "estimated completion time, and rating — then chooses a shop, orders, and "
        "tracks the job to completion.")
    b.para(
        "The project demonstrates the required PRN232 technical outcomes: a "
        "multi-layer architecture, RESTful APIs, Entity Framework Core with "
        "Microsoft SQL Server, JWT authentication with role-based and shop-scoped "
        "authorization, OData, JSON/XML/CSV content negotiation, a client "
        "application, and service-to-service communication through a gRPC Quote "
        "Engine and an asynchronous production pipeline over RabbitMQ.")

    b.heading("1.3. Scope", 2)
    b.para("In scope:")
    b.bullets([
        "Account management with Customer, Shop Owner, Shop Staff, and Admin roles. "
        "Customers self-register; shops are created through an application reviewed by an Admin.",
        "Shop discovery: search, filter, sort, and paginate shops and view rate cards and reviews.",
        "Shop management: storefront profile, rate card (services + pricing rules), machines, materials, and staff.",
        "Multi-shop quote comparison computed by a gRPC Quote Engine using each shop's pricing rules.",
        "Order lifecycle: place (wallet payment), shop accept/decline, asynchronous production, hand-over, cancel.",
        "Internal wallet with VietQR top-up, payment, and automatic refunds.",
        "Reviews after completion and a complaint / resolution workflow.",
        "Revenue, commission, and platform reports; OData query endpoints.",
    ])
    b.para("Out of scope:")
    b.bullets([
        "A live banking API — top-up is confirmed against the platform record (a valid VietQR code is still generated).",
        "Shop payout/settlement transfers (commission is calculated and reported only).",
        "Real courier/logistics integration and automated order dispatch (the customer selects the shop).",
        "Physical printer/machine drivers — production is executed by a software agent that simulates machine behaviour.",
        "Automated content inspection of uploaded files (rights are declared by the customer).",
    ])

    # ===================================================================== #
    # 2. User Roles
    # ===================================================================== #
    b.heading("2. User Roles", 1)
    b.table([
        ["Role", "Description", "Main permissions"],
        ["Guest", "Unauthenticated visitor",
         "Browse the shop directory, view rate cards and reviews (no ordering)"],
        ["Customer", "Person ordering services",
         "Register/login, maintain a document library, compare quotes, order and pay from a wallet, track and collect orders, review and complain, apply to open a shop"],
        ["Shop Owner", "Proprietor of a shop",
         "Manage own shop: profile, rate card and pricing rules, machines, materials, staff; view revenue reports"],
        ["Shop Staff", "Counter operator of a shop",
         "Process the shop's order queue and manage machines/materials; cannot change pricing or view full revenue"],
        ["Admin", "Platform operator",
         "Review/suspend shops, manage users, maintain the service catalogue and commission, adjudicate complaints, view platform reports"],
    ])
    b.para(
        "Shop operations are scoped by membership: an owner or staff member may act "
        "only on a shop they belong to (verified from the JWT's shop claims). This "
        "second authorization dimension — not just \"what role\" but \"which shop\" — "
        "is enforced in the service layer.")

    # ===================================================================== #
    # 3. Use Cases
    # ===================================================================== #
    b.heading("3. Use Cases", 1)
    b.heading("3.1. Use Case List", 2)
    b.table([
        ["ID", "Actor", "Use case", "Expected result"],
        ["UC-01", "Guest", "Register", "Create a Customer account and receive a JWT + refresh token"],
        ["UC-02", "Guest", "Log in", "Receive a JWT with identity, role, and shop-membership claims"],
        ["UC-03", "Common", "Refresh / logout / change password", "Rotate tokens; revoke server-side; re-hash password and revoke all tokens"],
        ["UC-09", "Guest/Customer", "Browse & search shops", "Filter/sort/paginate active shops (also via OData)"],
        ["UC-10", "Guest/Customer", "View shop detail", "See a shop's rate card, machines, and reviews"],
        ["UC-11", "Customer", "Manage favourites", "Add/remove/list favourite shops"],
        ["UC-13", "Customer", "Compare quotes", "Configure items once and receive comparable quotes from eligible shops"],
        ["UC-15", "Customer", "Place order", "Confirm a quote and pay from the wallet (atomic)"],
        ["UC-16", "Customer", "Track order", "View status, progress, and the full transition history"],
        ["UC-17", "Customer", "Cancel order", "Cancel before production with a rule-based refund"],
        ["UC-23", "Customer", "Review shop", "Rate a shop after a completed order"],
        ["UC-24", "Customer", "Raise complaint", "Open a complaint and respond to the shop's resolution"],
        ["UC-25", "Customer", "Apply to open shop", "Submit a shop application (enters PendingReview)"],
        ["UC-26/27", "Shop Owner", "Manage profile & rate card", "Edit storefront; manage services, prices, and pricing rules"],
        ["UC-28", "Owner/Staff", "Manage machines & materials", "Register machines, set status, adjust stock"],
        ["UC-29", "Shop Owner", "Manage staff", "Grant/revoke/list staff access"],
        ["UC-31..34", "Owner/Staff", "Operate orders", "View queue; accept/decline; produce; hand over"],
        ["UC-36/37", "Admin", "Govern shops", "Approve/reject applications; suspend/reinstate shops"],
        ["UC-38..42", "Admin", "Administer platform", "Manage users, catalogue/commission, vouchers, complaints, reports"],
    ])
    b.heading("3.2. Use Case Diagram", 2)
    b.para("[Insert use case diagrams per actor: Guest, Customer, Shop Owner, Shop Staff, Admin.]")

    # ===================================================================== #
    # 4. ERD and Database Schema
    # ===================================================================== #
    b.heading("4. ERD and Database Schema", 1)
    b.heading("4.1. Main Entities", 2)
    b.table([
        ["Entity", "Purpose"],
        ["User", "Authentication account, role, wallet balance, and profile"],
        ["RefreshToken", "Server-side refresh token for rotation and revocation"],
        ["Shop", "Shop storefront, address, status (onboarding state machine), and rating"],
        ["ShopStaff", "User×Shop membership with attributes (position) — scoped authorization"],
        ["ServiceType", "Platform service catalogue; pricing model selects the pricing strategy"],
        ["ShopService", "Shop×ServiceType rate card with attributes (unit price, lead time) — read on every quote"],
        ["PriceRule", "Option multiplier / surcharge / quantity tier on a rate card entry"],
        ["Machine / Material", "Shop production resources and stock"],
        ["DocumentFile", "A customer's uploaded file (library)"],
        ["Quote", "A computed price for a configuration at a shop, with expiry and breakdown"],
        ["Order / OrderItem", "The central transaction and its configured lines"],
        ["OrderStatusHistory", "Append-only transition log (actor, timestamp, reason)"],
        ["WalletTransaction", "Wallet ledger: top-up, payment, refund, adjustment"],
        ["Voucher / Review / Complaint", "Promotions, ratings, and the dispute workflow"],
        ["Favourite / Notification / AuditLog", "Saved shops, in-app notifications, administrative audit trail"],
    ])
    b.para(
        "The schema has one-to-many relationships throughout and three "
        "many-to-many relationships carrying their own attributes: ShopService "
        "(the rate card), ShopStaff (memberships), and Favourite. All 21 tables are "
        "generated code-first from the entities and Fluent API configuration.")

    b.heading("4.2. Entity Relationship Diagram", 2)
    b.para("[Insert the ERD (21 tables with PK/FK and multiplicities).]")

    b.heading("4.3. Important Database Constraints", 2)
    b.bullets([
        "User.Email, ServiceType.Code, Voucher.Code, Order.OrderCode, and WalletTransaction.RefCode are unique.",
        "ShopService(ShopId, ServiceTypeId), ShopStaff(ShopId, UserId), and Favourite(CustomerId, ShopId) have composite unique indexes.",
        "Review.OrderId is unique — one review per order.",
        "Delete behaviours are chosen to avoid multiple cascade paths on SQL Server; most references use Restrict.",
        "Soft delete on User/Shop/ServiceType/Machine/Material/DocumentFile via a global query filter preserves history.",
        "Monetary values use decimal(18,2); timestamps are stored in UTC.",
        "CreatedAt/UpdatedAt are stamped centrally by the DbContext, not by callers.",
    ])

    # ===================================================================== #
    # 5. Business Rules
    # ===================================================================== #
    b.heading("5. Business Rules", 1)
    b.table([
        ["ID", "Rule"],
        ["BR-01", "One account per email. New accounts are Customers with a zero wallet balance; passwords are BCrypt-hashed."],
        ["BR-02", "Login returns a single generic error so account existence cannot be probed; locked accounts are not issued a token."],
        ["BR-03", "A refresh token is revoked server-side on logout; a password change revokes every active refresh token."],
        ["BR-04", "A user may act only on their own records; identity comes from the token, never a client-supplied id."],
        ["BR-05", "Owner/staff may act only on a shop they belong to (scoped authorization); cross-shop access returns 403."],
        ["BR-06", "Only an Admin approves a shop; approval elevates the applicant to Shop Owner. Shop transitions follow the onboarding state machine."],
        ["BR-07", "Only the Shop Owner manages pricing; staff cannot view or change the rate card or revenue."],
        ["BR-08", "Rate card prices are non-negative and multipliers positive; quantity tiers on one entry must not overlap."],
        ["BR-09", "A machine producing an order cannot be taken offline; material stock cannot be negative."],
        ["BR-10", "A quote is computed only for eligible shops (offers every service and has a non-offline machine per group) and expires after 24 hours."],
        ["BR-11", "When the Quote Engine is unavailable the platform returns an indicative price and marks it as non-final (graceful degradation)."],
        ["BR-12", "An order is placed only against a valid, unexpired quote with a sufficient wallet balance; placement debits the wallet and snapshots the quote atomically."],
        ["BR-13", "Order status changes follow the order state machine; every transition is recorded in an append-only history with actor and reason."],
        ["BR-14", "Declining an order refunds the customer in full; cancellation before production refunds per the cancellation rule."],
        ["BR-15", "Production runs asynchronously through a message queue and never inside an HTTP request; event handling is idempotent."],
        ["BR-16", "Only a customer who owns a completed order may review it, once; only a completed order may raise a complaint within a window."],
        ["BR-17", "Every wallet transaction records the resulting balance so the ledger is independently verifiable; balances are never edited directly."],
        ["BR-18", "Administrative actions (approve/suspend/lock/adjudicate) require a reason and are written to the audit log."],
    ])

    # ===================================================================== #
    # 6. Workflows
    # ===================================================================== #
    b.heading("6. Workflows", 1)
    b.heading("6.1. Order Workflow", 2)
    b.table([
        ["Status", "Meaning", "Valid next status"],
        ["Draft / Quoted", "Configured; comparable quotes generated", "AwaitingAcceptance, Expired"],
        ["AwaitingAcceptance", "Placed and paid; awaiting the shop", "Accepted, Declined, Cancelled"],
        ["Accepted", "Shop committed capacity", "InProduction, Cancelled (with fee)"],
        ["InProduction", "Production job dispatched to the agent", "ReadyForPickup, OutForDelivery, ProductionFailed"],
        ["ProductionFailed", "Machine fault reported by the agent", "InProduction (retry), Declined (refund)"],
        ["ReadyForPickup / OutForDelivery", "Production complete", "Completed"],
        ["Completed", "Handed over; commission recorded", "Terminal (complaint may follow)"],
        ["Declined / Cancelled", "Refunded per rule", "Terminal"],
    ])
    b.para(
        "Production is asynchronous: on start, the API publishes a job message to "
        "RabbitMQ; a Production Agent consumes it, simulates the machine, and "
        "publishes progress/completion/failure events that the API applies to the "
        "order. A dead-letter queue and idempotent handlers make the pipeline safe "
        "under redelivery and failure.")

    b.heading("6.2. Shop Onboarding Workflow", 2)
    b.table([
        ["Status", "Meaning", "Valid next status"],
        ["Draft / PendingReview", "Application submitted, awaiting Admin review", "Active, Rejected"],
        ["Active", "Approved; discoverable and accepting orders", "Suspended"],
        ["Rejected", "Declined with a reason; may be revised", "PendingReview"],
        ["Suspended", "Removed from discovery; in-progress orders continue", "Active (reinstate)"],
    ])

    # ===================================================================== #
    # 7. System Architecture
    # ===================================================================== #
    b.heading("7. System Architecture", 1)
    b.heading("7.1. Architecture Diagram", 2)
    b.para(
        "[Insert the architecture diagram: Web (MVC+JS) and WPF Desktop clients → "
        "PrintHub.Api (REST + OData) → Application services → Infrastructure "
        "(EF Core/SQL Server); PrintHub.Api → PrintHub.QuoteEngine over gRPC; "
        "PrintHub.Api ↔ RabbitMQ ↔ PrintHub.ProductionAgent.]")

    b.heading("7.2. Layer Responsibilities", 2)
    b.table([
        ["Layer / Project", "Responsibility"],
        ["PrintHub.Api", "Controllers, OData, JWT auth, content negotiation, validation filter, global exception middleware, response wrapper"],
        ["PrintHub.Application", "Use-case services (Result pattern), DTOs, AutoMapper, FluentValidation, specifications, repository/UoW interfaces"],
        ["PrintHub.Domain", "Entities, enums, and base types (auditing, soft delete) — depends on nothing"],
        ["PrintHub.Infrastructure", "DbContext + EF configuration + migrations, repository/UoW/specification evaluator, BCrypt, JWT, gRPC client"],
        ["PrintHub.Contracts", "Shared gRPC proto and message contracts"],
        ["PrintHub.QuoteEngine", "gRPC pricing service (Strategy per pricing model)"],
        ["PrintHub.ProductionAgent", "Worker consuming RabbitMQ; simulates machines and publishes progress"],
        ["PrintHub.Web / PrintHub.Desktop", "ASP.NET Core MVC + JavaScript customer client; WPF (MVVM) shop/admin client"],
    ])
    b.para(
        "Controllers stay thin: they receive a request, call a service, and map the "
        "Result to an HTTP status. Business rules, workflow validation, ownership "
        "checks, and transactions live in the service layer. Queries flow through "
        "specifications, keeping repositories thin.")

    # ===================================================================== #
    # 8. Service Design
    # ===================================================================== #
    b.heading("8. Service Design", 1)
    b.table([
        ["Service", "Responsibility"],
        ["AuthService", "Register, login, refresh (rotating), logout, change password; issues JWTs with shop claims"],
        ["ShopCatalogService", "Public shop search and detail via specifications"],
        ["FavouriteService", "Add/remove/list a customer's favourite shops"],
        ["ShopOnboardingService / ShopAdminService", "Apply to open a shop; admin approve/reject/suspend/reinstate (state machine + audit)"],
        ["ShopProfileService / RateCardService", "Owner storefront profile; rate card (ShopService + PriceRule) with tier validation"],
        ["ShopStaffService / ShopResourceService", "Staff grant/revoke; machines and materials (owner or staff)"],
        ["QuoteService", "Eligibility, fan-out to the gRPC Quote Engine, persist quotes, rank, indicative fallback"],
        ["OrderService / WalletService", "Order state machine, atomic placement + wallet debit, refunds, commission"],
        ["ReviewService / ComplaintService", "Reviews and the complaint/resolution workflow"],
        ["ReportService", "Revenue, commission, service mix, and platform statistics"],
        ["NotificationService / AuditLogService", "In-app notifications from events; append-only audit of admin actions"],
    ])

    # ===================================================================== #
    # 9. API Endpoint List
    # ===================================================================== #
    b.heading("9. API Endpoint List", 1)
    b.para(
        "Convention: Guest endpoints require no token; others require a JWT. Shop "
        "endpoints are membership-scoped; admin endpoints require the Admin role. "
        "Responses use a consistent ApiResponse envelope; errors map to 400/401/403/404/409.")

    b.heading("9.1. Authentication and Users", 2)
    b.table([
        ["Method", "Route", "Access", "Description"],
        ["POST", "/api/auth/register", "Guest", "Register a Customer account"],
        ["POST", "/api/auth/login", "Guest", "Authenticate and return a JWT + refresh token"],
        ["POST", "/api/auth/refresh", "Guest", "Rotate an access token from a refresh token"],
        ["POST", "/api/auth/logout", "Authenticated", "Revoke the refresh token server-side"],
        ["POST", "/api/auth/change-password", "Authenticated", "Change password and revoke all tokens"],
        ["GET", "/api/auth/me", "Authenticated", "Identity, role, and shop claims from the token"],
    ])

    b.heading("9.2. Shop Discovery and Favourites", 2)
    b.table([
        ["Method", "Route", "Access", "Description"],
        ["GET", "/api/shops", "Guest", "Search/filter/sort/paginate active shops"],
        ["GET", "/api/shops/{id}", "Guest", "Shop profile, rate card, machines, reviews"],
        ["GET", "/api/favourites", "Customer", "List favourite shops"],
        ["POST", "/api/favourites/{shopId}", "Customer", "Add a favourite (idempotent)"],
        ["DELETE", "/api/favourites/{shopId}", "Customer", "Remove a favourite"],
    ])

    b.heading("9.3. Shop Onboarding and Management", 2)
    b.table([
        ["Method", "Route", "Access", "Description"],
        ["POST", "/api/shops/apply", "Customer", "Apply to open a shop"],
        ["GET", "/api/shops/mine", "Authenticated", "The applicant's own shops"],
        ["PUT", "/api/shops/{id}/profile", "owner", "Update the storefront profile"],
        ["GET/POST/PUT", "/api/shops/{id}/rate-card", "owner", "Manage services and pricing rules"],
        ["GET/POST/DELETE", "/api/shops/{id}/staff", "owner", "Manage staff access"],
        ["GET/POST/PUT/DELETE", "/api/shops/{id}/machines | materials", "owner/staff", "Manage machines and materials"],
        ["GET/PUT", "/api/admin/shops/applications | {id}/approve|reject|suspend|reinstate", "Admin", "Govern shops"],
    ])

    b.heading("9.4. Quoting and Orders", 2)
    b.table([
        ["Method", "Route", "Access", "Description"],
        ["POST", "/api/quotes/compare", "Customer", "Compare quotes across eligible shops (gRPC Quote Engine)"],
        ["POST", "/api/orders", "Customer", "Place an order from a selected quote (wallet payment)"],
        ["GET", "/api/orders/{id}", "owner/shop", "Track status, progress, and history"],
        ["PUT", "/api/orders/{id}/cancel | pickup", "Customer", "Cancel before production; confirm receipt"],
        ["GET/PUT", "/api/shops/{id}/orders | {orderId}/accept|decline|start|handover", "owner/staff", "Operate the order queue"],
        ["POST", "/api/wallet/topup ; GET /api/wallet/transactions", "Customer", "VietQR top-up and wallet statement"],
    ])

    b.heading("9.5. Reports", 2)
    b.table([
        ["Method", "Route", "Access", "Description"],
        ["GET", "/api/shops/{id}/reports/revenue", "owner", "Shop revenue, commission, and service mix"],
        ["GET", "/api/reports/platform", "Admin", "Platform transaction, revenue, and shop-performance reports"],
    ])

    # ===================================================================== #
    # 10. Security Matrix
    # ===================================================================== #
    b.heading("10. Security Matrix", 1)
    b.para("Legend: Yes = permitted; Own = own record / own shop only; No = denied.")
    b.table([
        ["Function", "Customer", "Shop Owner", "Shop Staff", "Admin"],
        ["Register / login / own profile", "Yes", "Yes", "Yes", "Yes"],
        ["Browse shops, compare quotes", "Yes", "Yes", "Yes", "View"],
        ["Place / track / cancel orders", "Own", "Own", "No", "View"],
        ["Manage shop profile & rate card", "No", "Own shop", "No", "No"],
        ["Manage machines / materials", "No", "Own shop", "Own shop", "No"],
        ["Manage shop staff", "No", "Own shop", "No", "No"],
        ["Operate order queue (accept/produce)", "No", "Own shop", "Own shop", "No"],
        ["Approve / suspend shops", "No", "No", "No", "Yes"],
        ["Manage users, catalogue, commission", "No", "No", "No", "Yes"],
        ["Adjudicate complaints", "No", "No", "No", "Yes"],
        ["View reports", "No", "Own shop", "No", "Platform"],
    ])
    b.para("Security controls:")
    b.bullets([
        "Passwords are hashed with BCrypt and never stored or logged as plaintext.",
        "JWTs contain sub, email, role, jti, shop-membership claims, issued-at, and expiration.",
        "The signing key is loaded from configuration and is not committed as a real secret.",
        "[Authorize] role policies plus service-layer ownership/shop-scope checks protect every operation.",
        "Response DTOs never expose PasswordHash, storage paths, or a shop's material cost.",
        "A validation filter and global exception handling prevent invalid data and stack-trace leakage.",
    ])

    # ===================================================================== #
    # 11. OData Demo
    # ===================================================================== #
    b.heading("11. OData Demo", 1)
    b.table([
        ["Endpoint", "Query model", "Supported options", "Access"],
        ["GET /odata/Shops", "Shop", "$filter, $orderby, $top, $skip, $select, $count", "Public; active shops only"],
        ["GET /odata/Orders", "Order", "$filter, $orderby, $top, $skip, $select, $expand", "Customer sees own orders; shop sees own"],
    ])
    b.para(
        "Safe limits: MaxTop = 100. A mandatory server-side filter (active shops; "
        "owner's own orders) is applied before the client query composes, so a "
        "crafted query cannot widen the result set. Only DTO projections are exposed "
        "— never EF entities.")
    b.mono(
        "GET /odata/Shops?$filter=RatingAverage gt 4&$orderby=Name&$top=10&$select=Name,RatingAverage\n"
        "Authorization: Bearer <token>\n"
        "Accept: application/json")
    b.mono(
        "GET /odata/Orders?$filter=Status eq 'Completed'&$orderby=PlacedAt desc&$expand=Items($select=ServiceTypeId,Quantity)\n"
        "Authorization: Bearer <token>")

    # ===================================================================== #
    # 12. Content Negotiation Demo
    # ===================================================================== #
    b.heading("12. Content Negotiation Demo", 1)
    b.para(
        "The API uses the default JSON formatter and enables XML with "
        "AddXmlSerializerFormatters(); report endpoints add a custom CSV output "
        "formatter. Accept selects the response representation; Content-Type "
        "identifies the request body. Report endpoints declare "
        "[Produces(\"application/json\",\"application/xml\",\"text/csv\")].")
    b.heading("12.1. JSON", 2)
    b.mono("GET /api/shops/1\nAccept: application/json\n\nHTTP/1.1 200 OK\nContent-Type: application/json; charset=utf-8")
    b.heading("12.2. XML", 2)
    b.mono("GET /api/shops/1\nAccept: application/xml\n\nHTTP/1.1 200 OK\nContent-Type: application/xml; charset=utf-8")
    b.heading("12.3. CSV (reports)", 2)
    b.mono("GET /api/reports/platform?from=2026-07-01&to=2026-07-31\nAccept: text/csv\n\nHTTP/1.1 200 OK\nContent-Type: text/csv; charset=utf-8")

    # ===================================================================== #
    # 13. gRPC Quote Engine Demo
    # ===================================================================== #
    b.heading("13. gRPC Quote Engine Demo", 1)
    b.heading("13.1. Purpose", 2)
    b.para(
        "A separate gRPC service (PrintHub.QuoteEngine) computes price and estimated "
        "completion time. It is a stateless calculator: the API gathers each shop's "
        "pricing data and asks the engine to price the order. This isolates pricing "
        "(and the Strategy pattern per pricing model — PerPage, PerUnit, "
        "MaterialAndTime) in a service that can evolve and scale independently. The "
        "call is made once per eligible shop when a customer compares quotes; if the "
        "engine is unavailable the API falls back to an indicative price and stays up.")
    b.heading("13.2. Proposed Contract", 2)
    b.mono(
        "syntax = \"proto3\";\n"
        "option csharp_namespace = \"PrintHub.Contracts.Quoting\";\n\n"
        "service QuoteEstimator {\n"
        "  rpc Estimate (EstimateRequest) returns (EstimateResponse);\n"
        "}\n\n"
        "message EstimateItem {\n"
        "  string pricing_model = 1; int32 quantity = 2; int32 page_count = 3;\n"
        "  double estimated_grams = 4; double unit_price = 5; double setup_fee = 6;\n"
        "  int32 lead_time_minutes = 7; repeated string selected_options = 8;\n"
        "  repeated PricingRuleInput rules = 9;\n"
        "}\n\n"
        "message EstimateResponse {\n"
        "  double subtotal = 1; int32 estimated_minutes = 2;\n"
        "  repeated LineBreakdown lines = 3;\n"
        "}")
    b.heading("13.3. Service Call Sequence", 2)
    b.para(
        "Customer → API /api/quotes/compare → for each eligible shop the API builds "
        "an EstimateRequest and calls QuoteEstimator.Estimate over gRPC → the engine "
        "selects the pricing strategy per item, applies the shop's rules, and returns "
        "a subtotal, minutes, and breakdown → the API persists a Quote per shop and "
        "returns a ranked comparison. [Insert the sequence diagram.]")

    # ===================================================================== #
    # 14. Project Setup and Run Guide
    # ===================================================================== #
    b.heading("14. Project Setup and Run Guide", 1)
    b.heading("14.1. Prerequisites", 2)
    b.bullets([
        ".NET 8 SDK.",
        "Microsoft SQL Server LocalDB (or SQL Server Express / Developer).",
        "RabbitMQ (local install or CloudAMQP) for the production pipeline.",
        "Visual Studio 2022 / Rider / VS Code.",
    ])
    b.heading("14.2. Solution Layout", 2)
    b.mono(
        "PrintHub.sln\n"
        "└─ src/\n"
        "   ├─ PrintHub.Domain / Application / Infrastructure / Contracts\n"
        "   ├─ PrintHub.Api             # REST + OData        (http://localhost:5080)\n"
        "   ├─ PrintHub.QuoteEngine     # gRPC (h2c)          (http://localhost:5090)\n"
        "   ├─ PrintHub.ProductionAgent # RabbitMQ worker\n"
        "   └─ PrintHub.Web / PrintHub.Desktop  # MVC+JS client / WPF client")
    b.heading("14.3. Configuration", 2)
    b.mono(
        "{\n"
        "  \"ConnectionStrings\": {\n"
        "    \"DefaultConnection\": \"Server=(localdb)\\\\MSSQLLocalDB;Database=PrintHubDb;Trusted_Connection=True;TrustServerCertificate=True\"\n"
        "  },\n"
        "  \"Jwt\": { \"Issuer\": \"PrintHub\", \"Audience\": \"PrintHubClients\", \"Key\": \"<dev-secret>\", \"AccessTokenMinutes\": 15, \"RefreshTokenDays\": 7 },\n"
        "  \"QuoteEngine\": { \"Address\": \"http://localhost:5090\" }\n"
        "}")
    b.heading("14.4. Create the Database", 2)
    b.mono(
        "dotnet ef database update --project src/PrintHub.Infrastructure --startup-project src/PrintHub.Infrastructure\n"
        "# Or simply run the API once: migrations are applied and the seed runs on startup.")
    b.para(
        "The seed creates 9 users (one per role plus shops/customers), 12 service "
        "types across the three pricing groups, 3 active shops with rate cards, "
        "machines and materials, vouchers, and sample orders for reports.")
    b.heading("14.5. Run the Services", 2)
    b.mono(
        "# Terminal 1 — gRPC Quote Engine:\n"
        "dotnet run --project src/PrintHub.QuoteEngine        # gRPC on :5090\n\n"
        "# Terminal 2 — Main Web API:\n"
        "dotnet run --project src/PrintHub.Api --urls http://localhost:5080\n"
        "# Swagger: http://localhost:5080/swagger   OData: http://localhost:5080/odata")
    b.heading("14.6. Seeded Demo Accounts", 2)
    b.table([
        ["Role", "Email", "Password"],
        ["Admin", "admin@printhub.vn", "Password123!"],
        ["Shop Owner", "owner.quickprint@printhub.vn", "Password123!"],
        ["Shop Staff", "staff.quickprint@printhub.vn", "Password123!"],
        ["Customer", "customer1@printhub.vn", "Password123!"],
    ])
    b.para("These credentials are for local demonstration only; the seed stores BCrypt hashes, never plaintext.")

    # ===================================================================== #
    # 15. Reports
    # ===================================================================== #
    b.heading("15. Reports", 1)
    b.table([
        ["Report", "Metrics"],
        ["Shop Revenue", "Gross revenue, platform commission, net revenue, order count, and average order value by day/service group"],
        ["Machine Utilisation & Failure", "Production time per machine and the decline / cancellation / production-failure rate"],
        ["Platform Summary", "Transaction volume, revenue and commission, shop-performance ranking, and service mix — exportable as JSON, XML, or CSV"],
    ])
