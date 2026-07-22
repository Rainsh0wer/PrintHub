# PrintHub — Complete API Endpoint List (all 42 use cases)

Convention: Guest endpoints need no token; others need a JWT. Shop endpoints are
membership-scoped (owner/staff of that shop); admin endpoints require the Admin
role. Responses use a consistent `ApiResponse` envelope; errors map to
400/401/403/404/409. **Status** column: ✔ = implemented, ◻ = designed (planned).

## 9.1 Authentication & Profile

| Method | Route | Access | UC | Description | Status |
|---|---|---|---|---|---|
| POST | /api/auth/register | Guest | UC-01 | Register a Customer account | ✔ |
| POST | /api/auth/login | Guest | UC-02 | Authenticate → JWT + refresh token | ✔ |
| POST | /api/auth/refresh | Guest | UC-02 | Rotate access token from refresh token | ✔ |
| POST | /api/auth/forgot-password | Guest | UC-03 | Request a password-reset link | ◻ |
| POST | /api/auth/reset-password | Guest | UC-03 | Reset password with the emailed token | ◻ |
| POST | /api/auth/logout | Authenticated | UC-04 | Revoke the refresh token server-side | ✔ |
| POST | /api/auth/change-password | Authenticated | UC-05 | Change password + revoke all tokens | ✔ |
| GET | /api/auth/me | Authenticated | — | Identity/role/shop claims from the token | ✔ |
| GET | /api/users/me | Authenticated | UC-06 | View own profile (full account info) | ◻ |
| PUT | /api/users/me | Authenticated | UC-07 | Update display name, phone, default address | ◻ |

## 9.2 Notifications

| Method | Route | Access | UC | Description | Status |
|---|---|---|---|---|---|
| GET | /api/notifications | Authenticated | UC-08 | List own notifications (paged, newest first) | ◻ |
| PUT | /api/notifications/read | Authenticated | UC-08 | Mark displayed notifications as read | ◻ |

## 9.3 Shop Discovery & Favourites

| Method | Route | Access | UC | Description | Status |
|---|---|---|---|---|---|
| GET | /api/shops | Guest | UC-09 | Search/filter/sort/paginate active shops | ✔ |
| GET | /api/shops/{id} | Guest | UC-10 | Shop profile, rate card, machines, reviews | ✔ |
| GET | /api/favourites | Customer | UC-11 | List favourite shops | ✔ |
| POST | /api/favourites/{shopId} | Customer | UC-11 | Add a favourite (idempotent) | ✔ |
| DELETE | /api/favourites/{shopId} | Customer | UC-11 | Remove a favourite | ✔ |

## 9.4 Document Library

| Method | Route | Access | UC | Description | Status |
|---|---|---|---|---|---|
| GET | /api/documents | Customer | UC-12 | List own uploaded files | ◻ |
| POST | /api/documents | Customer | UC-12 | Upload a file (type/size/rights validated) | ◻ |
| PUT | /api/documents/{id} | Customer | UC-12 | Rename a file | ◻ |
| DELETE | /api/documents/{id} | Customer | UC-12 | Soft-delete a file (blocked if on an active order) | ◻ |

## 9.5 Quoting

| Method | Route | Access | UC | Description | Status |
|---|---|---|---|---|---|
| POST | /api/quotes/compare | Customer | UC-13 | Compare quotes across eligible shops (gRPC) | ✔ |
| POST | /api/quotes/{id}/apply-voucher | Customer | UC-14 | Apply a voucher and recompute the total | ◻ |

## 9.6 Orders (Customer)

| Method | Route | Access | UC | Description | Status |
|---|---|---|---|---|---|
| POST | /api/orders | Customer | UC-15 | Place an order from a quote (wallet payment) | ◻ |
| GET | /api/orders | Customer | UC-20 | List/filter own order history (paged) | ◻ |
| GET | /api/orders/{id} | Customer/shop | UC-16 | Track status, progress, and full history | ◻ |
| PUT | /api/orders/{id}/cancel | Customer | UC-17 | Cancel before production (rule-based refund) | ◻ |
| POST | /api/orders/{id}/reorder | Customer | UC-18 | Create a new draft from a completed order | ◻ |
| PUT | /api/orders/{id}/confirm-receipt | Customer | UC-19 | Confirm collection → Completed | ◻ |

## 9.7 Wallet

| Method | Route | Access | UC | Description | Status |
|---|---|---|---|---|---|
| POST | /api/wallet/topup | Customer | UC-21 | Request a top-up → VietQR payload + ref code | ◻ |
| GET | /api/wallet/transactions | Customer | UC-22 | Balance + filterable transaction ledger | ◻ |
| PUT | /api/admin/wallet/topups/{refCode}/confirm | Admin | UC-21 | Confirm a pending top-up against the record | ◻ |

## 9.8 Reviews & Complaints (Customer)

| Method | Route | Access | UC | Description | Status |
|---|---|---|---|---|---|
| POST | /api/orders/{id}/review | Customer | UC-23 | Rate + review a completed order (one per order) | ◻ |
| GET | /api/shops/{id}/reviews | Guest | UC-23 | List a shop's public reviews | ◻ |
| POST | /api/complaints | Customer | UC-24 | Raise a complaint on a completed order | ◻ |
| GET | /api/complaints/mine | Customer | UC-24 | List own complaints and their status | ◻ |
| PUT | /api/complaints/{id}/accept | Customer | UC-24 | Accept the shop's proposed resolution | ◻ |
| PUT | /api/complaints/{id}/escalate | Customer | UC-24 | Reject and escalate to the platform | ◻ |

## 9.9 Shop Onboarding & Management (Owner)

| Method | Route | Access | UC | Description | Status |
|---|---|---|---|---|---|
| POST | /api/shops/apply | Customer | UC-25 | Apply to open a shop (→ PendingReview) | ✔ |
| GET | /api/shops/mine | Authenticated | UC-25 | The applicant's own shops + status | ✔ |
| PUT | /api/shops/{id}/profile | Owner | UC-26 | Update the storefront profile | ✔ |
| GET | /api/shops/{id}/rate-card | Owner | UC-27 | View the rate card (entries + rules) | ✔ |
| POST | /api/shops/{id}/rate-card | Owner | UC-27 | Add a service to the rate card | ✔ |
| PUT | /api/shops/{id}/rate-card/{entryId} | Owner | UC-27 | Update pricing on an entry | ✔ |
| POST | /api/shops/{id}/rate-card/{entryId}/rules | Owner | UC-27 | Add a pricing rule | ✔ |
| DELETE | /api/shops/{id}/rate-card/{entryId}/rules/{ruleId} | Owner | UC-27 | Remove a pricing rule | ✔ |
| GET/POST | /api/shops/{id}/machines | Owner/Staff | UC-28 | List / add machines | ✔ |
| PUT | /api/shops/{id}/machines/{mId}/status | Owner/Staff | UC-28 | Set machine status (Idle/Maintenance/Offline) | ✔ |
| DELETE | /api/shops/{id}/machines/{mId} | Owner/Staff | UC-28 | Soft-delete a machine | ✔ |
| GET/POST | /api/shops/{id}/materials | Owner/Staff | UC-28 | List / add materials | ✔ |
| PUT | /api/shops/{id}/materials/{matId}/stock | Owner/Staff | UC-28 | Adjust stock / low-stock threshold | ✔ |
| DELETE | /api/shops/{id}/materials/{matId} | Owner/Staff | UC-28 | Soft-delete a material | ✔ |
| GET | /api/shops/{id}/staff | Owner | UC-29 | List staff | ✔ |
| POST | /api/shops/{id}/staff | Owner | UC-29 | Grant staff access (by email) | ✔ |
| DELETE | /api/shops/{id}/staff/{staffId} | Owner | UC-29 | Revoke staff access (soft) | ✔ |
| GET | /api/shops/{id}/reports/revenue | Owner | UC-30 | Revenue / commission / service-mix report | ◻ |

## 9.10 Shop Operations (Owner / Staff)

| Method | Route | Access | UC | Description | Status |
|---|---|---|---|---|---|
| GET | /api/shops/{id}/orders | Owner/Staff | UC-31 | Order queue (filter by status) | ◻ |
| PUT | /api/orders/{id}/accept | Owner/Staff | UC-32 | Accept an incoming order | ◻ |
| PUT | /api/orders/{id}/decline | Owner/Staff | UC-32 | Decline with reason (auto full refund) | ◻ |
| PUT | /api/orders/{id}/start | Owner/Staff | UC-33 | Assign a machine + start production (RabbitMQ) | ◻ |
| PUT | /api/orders/{id}/handover | Owner/Staff | UC-34 | Record hand-over → Completed | ◻ |
| PUT | /api/complaints/{id}/respond | Owner/Staff | UC-35 | Propose a reprint / refund resolution | ◻ |

## 9.11 Platform Administration (Admin)

| Method | Route | Access | UC | Description | Status |
|---|---|---|---|---|---|
| GET | /api/admin/shops/applications | Admin | UC-36 | List pending shop applications | ✔ |
| PUT | /api/admin/shops/{id}/approve | Admin | UC-36 | Approve (activate + elevate owner) | ✔ |
| PUT | /api/admin/shops/{id}/reject | Admin | UC-36 | Reject with a mandatory reason | ✔ |
| PUT | /api/admin/shops/{id}/suspend | Admin | UC-37 | Suspend an active shop (reason) | ✔ |
| PUT | /api/admin/shops/{id}/reinstate | Admin | UC-37 | Reinstate a suspended shop | ✔ |
| GET | /api/admin/users | Admin | UC-38 | Search / list users | ◻ |
| PUT | /api/admin/users/{id}/lock | Admin | UC-38 | Lock an account (revokes tokens) | ◻ |
| PUT | /api/admin/users/{id}/unlock | Admin | UC-38 | Unlock an account | ◻ |
| GET/POST | /api/admin/service-types | Admin | UC-39 | List / add platform service types | ◻ |
| PUT/DELETE | /api/admin/service-types/{id} | Admin | UC-39 | Update / deactivate a service type | ◻ |
| PUT | /api/admin/commission | Admin | UC-39 | Configure the platform commission rate | ◻ |
| GET/POST | /api/admin/vouchers | Admin | UC-40 | List / create vouchers | ◻ |
| PUT/DELETE | /api/admin/vouchers/{id} | Admin | UC-40 | Edit / deactivate a voucher | ◻ |
| GET | /api/admin/complaints | Admin | UC-41 | List escalated complaints | ◻ |
| PUT | /api/admin/complaints/{id}/adjudicate | Admin | UC-41 | Final ruling (uphold refund / reject) | ◻ |
| GET | /api/admin/audit-logs | Admin | — | View the audit log | ◻ |

## 9.12 Reports

| Method | Route | Access | UC | Description | Status |
|---|---|---|---|---|---|
| GET | /api/shops/{id}/reports/revenue | Owner | UC-30 | Shop revenue / commission / service mix | ◻ |
| GET | /api/reports/platform | Admin | UC-42 | Platform transaction/revenue/ranking reports | ◻ |
| — | (all report endpoints support `Accept: application/json \| application/xml \| text/csv`) | | UC-42 | Content negotiation | ◻ |

## 9.13 OData (query endpoints)

| Method | Route | Access | UC | Options | Status |
|---|---|---|---|---|---|
| GET | /odata/Shops | Guest | UC-09 | $filter $orderby $top $skip $select $count | ✔ |
| GET | /odata/Orders | Customer/shop | UC-20 | $filter $orderby $top $skip $select $expand | ◻ |

---

**Summary:** 42 use cases → ~70 endpoints. Currently **implemented (✔): 33** (auth,
shop discovery, favourites, onboarding, admin-shops, shop management, quoting, OData
Shops). **Designed (◻): ~37** (profile, notifications, documents, orders, wallet,
reviews, complaints, shop operations, admin catalog/users/vouchers/complaints,
reports, OData Orders) — these are the endpoints missing from the current §9 draft.
