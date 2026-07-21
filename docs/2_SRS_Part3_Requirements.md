# Report 2: Software Requirement Specification — **Part 3 of 3**

# 3. Functional Requirements

> Each subsection describes one screen: its purpose, its main features, and the fields it presents. Screen prototype figures are referenced as `Figure 22+`; capture them from the implemented UI and insert before submission.

## 3.1 Authentication

### 3.1.1 Register Screen

**Figure 22: Register Screen**

**Description:** Screen allowing a guest to create a PrintHub account.
**Main Features:** Collects identity and credentials, validates format and password strength client- and server-side, and dispatches a verification email on success.

| Field Name | Description |
|---|---|
| Full Name | Input field for the user's display name. Validates length 2–100 characters. |
| Email | Input field for the login identifier. Validates email format and uniqueness. |
| Phone Number | Input field for the contact number. Validates Vietnamese number format. |
| Password | Masked input with a show/hide toggle. Strength requirements displayed inline. |
| Confirm Password | Masked input validated for equality with Password. |
| Terms Checkbox | Mandatory acceptance of platform terms before the submit button activates. |
| Register Button | Primary action. Enabled only when all fields are valid. |
| Log In Link | Navigates to the login screen for existing users. |

### 3.1.2 Login Screen (Web)

**Figure 23: Login Screen – Web Client**

**Description:** Authentication screen for returning customers on the web client.
**Main Features:** Authenticates by email and password, stores the issued tokens, and routes to the customer dashboard.

| Field Name | Description |
|---|---|
| Email | Input field for the registered email address. Validates format. |
| Password | Masked input with a show/hide toggle. |
| Remember Me | Checkbox controlling whether the refresh token persists across browser restarts. |
| Log In Button | Primary action submitting the credentials. |
| Forgot Password Link | Navigates to the password recovery screen. |
| Register Link | Navigates to the registration screen. |

### 3.1.3 Login Window (Desktop)

**Figure 24: Login Window – Desktop Client**

**Description:** Authentication window for Shop Owner, Shop Staff, and Admin roles in the WPF client.
**Main Features:** Authenticates and routes to the Shop Dashboard or Admin Dashboard by role; rejects Customer-only accounts.

| Field Name | Description |
|---|---|
| Email | Input field for the registered email address. |
| Password | Masked input. |
| Shop Selector | Visible only when the authenticated user belongs to more than one shop; selects the active shop context for the session. |
| Log In Button | Primary action. |
| Status Bar | Displays connection state to the API and to the message broker. |

### 3.1.4 Forgot Password Screen

**Figure 25: Forgot Password Screen**

**Description:** Screen allowing a user to request a password reset link.
**Main Features:** Accepts an email address and returns a neutral acknowledgement regardless of whether the address is registered.

| Field Name | Description |
|---|---|
| Email | Input field for the registered email address. |
| Send Reset Link Button | Primary action triggering the reset email. |
| Back to Login Link | Returns to the login screen. |

## 3.2 Profile Management

### 3.2.1 Personal Profile Screen

**Figure 26: Personal Profile Screen**

**Description:** Screen displaying and editing the authenticated user's own account information.
**Main Features:** Views identity and account data, edits editable fields, and links to password change.

| Field Name | Description |
|---|---|
| Full Name | Editable display name. |
| Email | Read-only; changing the email requires administrator action. |
| Phone Number | Editable contact number. |
| Default Address | Editable address used as the delivery default and for distance ranking. |
| Role Badge | Read-only indicator of the account role. |
| Wallet Balance | Read-only current balance, linking to the wallet screen. |
| Shop Membership | Visible for shop roles; displays the shop name and position. |
| Save Button | Persists the edited fields. |
| Change Password Link | Navigates to the change password screen. |

### 3.2.2 Change Password Screen

**Figure 27: Change Password Screen**

**Description:** Screen allowing an authenticated user to replace their password.
**Main Features:** Verifies the current password, validates the new one, and revokes all sessions on success.

| Field Name | Description |
|---|---|
| Current Password | Masked input verified against the stored hash. |
| New Password | Masked input validated against strength rules and required to differ from the current password. |
| Confirm New Password | Masked input validated for equality with New Password. |
| Save Button | Primary action. |

### 3.2.3 Notification Panel

**Figure 28: Notification Panel**

**Description:** Panel listing notifications addressed to the authenticated user.
**Main Features:** Lists newest first with pagination, filters unread, and navigates to the related order.

| Field Name | Description |
|---|---|
| Unread Indicator | Badge showing the count of unread notifications. |
| Notification List | Each row shows type icon, title, content preview, and relative timestamp. |
| Unread Filter | Toggle restricting the list to unread notifications. |
| Notification Row Action | Navigates to the related order or complaint when one is linked. |

## 3.3 Shop Discovery

### 3.3.1 Shop Search Screen

**Figure 29: Shop Search Screen**

**Description:** The primary discovery screen, listing Active shops with filtering, sorting, and pagination.
**Main Features:** Keyword search, multi-criteria filtering, sorting by distance, rating, price, or name, and paginated results.

| Field Name | Description |
|---|---|
| Keyword Search | Text input matching shop name and description. |
| Service Group Filter | Multi-select over Document, Finishing, and Fabrication. |
| Service Type Filter | Multi-select over specific service types offered. |
| District Filter | Select over districts. |
| Minimum Rating Filter | Selector restricting results to shops at or above a rating threshold. |
| Price Range Filter | Range control over the shop's indicative price level. |
| Sort Selector | Orders results by distance, rating, price, or name. |
| Shop Card | Displays shop name, district, offered service groups, rating with review count, indicative price, and distance. |
| Favourite Toggle | Adds or removes the shop from favourites; visible only to authenticated customers. |
| Pagination Control | Page navigation showing current page and total result count. |

### 3.3.2 Shop Detail Screen

**Figure 30: Shop Detail Screen**

**Description:** Screen presenting a single shop's public profile, rate card, machines, and reviews.
**Main Features:** Inspects published pricing before ordering, reads reviews, and starts an order from this shop.

| Field Name | Description |
|---|---|
| Shop Header | Shop name, rating, review count, district, and open/closed indicator. |
| Description | Free-text shop description. |
| Address & Map | Physical address with distance and estimated travel time from the customer. |
| Operating Hours | Daily open and close times. |
| Rate Card | Table of active services grouped by service group, showing unit price, minimum quantity, and lead time. |
| Machine List | Machine type and availability indicator per machine. |
| Review List | Rating, comment, reviewer name, date, and shop reply where present; paginated. |
| Order From This Shop Button | Opens the order builder pre-selected to this shop. |
| Favourite Toggle | Adds or removes the shop from favourites. |

### 3.3.3 Document Library Screen

**Figure 31: Document Library Screen**

**Description:** Screen listing the customer's uploaded files for reuse across orders.
**Main Features:** Uploads, renames, and deletes files; displays declared page counts.

| Field Name | Description |
|---|---|
| Upload Area | Drag-and-drop or file picker; validates file type and size. |
| Declared Page Count | Numeric input required for document services. |
| Rights Declaration | Mandatory checkbox asserting the customer holds the right to reproduce the file. |
| File List | Each row shows file name, type icon, size, declared page count, and upload date. |
| Rename Action | Edits the display name of a stored file. |
| Delete Action | Soft-deletes the file; disabled when the file is attached to an active order. |
| Use in Order Action | Attaches the file to a new order item. |

## 3.4 Quoting and Ordering

### 3.4.1 Order Builder Screen

**Figure 32: Order Builder Screen**

**Description:** Screen where the customer configures order items before requesting a quote comparison.
**Main Features:** Adds multiple items, presents only the option fields required by each item's service type, and validates before comparison.

| Field Name | Description |
|---|---|
| Add Item Button | Adds a new order item row. |
| Service Type Selector | Selects the service; determines which option fields are displayed. |
| File Attachment | Selects a file from the library or uploads a new one; required for services with `RequiresFile`. |
| Quantity | Number of copies or units; validated against the shop's minimum quantity at comparison time. |
| Page Count | Page count for document services; pre-filled from the file's declared count. |
| Paper Type | Select over paper options for document and finishing services. |
| Colour Mode | Select between black-and-white and colour. |
| Sides | Select between single-sided and double-sided. |
| Binding Type | Select over binding options for finishing services. |
| Material | Select over materials for fabrication services. |
| Quality Profile | Select over Draft, Standard, and Fine for fabrication services. |
| Item Note | Free-text note for this item. |
| Remove Item Action | Deletes the item from the order. |
| Compare Quotes Button | Primary action initiating the quote comparison. |

### 3.4.2 Quote Comparison Screen

**Figure 33: Quote Comparison Screen**

**Description:** Screen presenting comparable quotes from all eligible shops for the configured order — the platform's distinguishing capability.
**Main Features:** Ranks results by the chosen criterion, shows an expandable price breakdown, and proceeds to checkout with the selected quote.

| Field Name | Description |
|---|---|
| Ranking Selector | Orders results by total price, estimated completion, distance, or rating. |
| Quote Card | Per shop: shop name, total price, estimated completion time, distance and travel time, rating with review count. |
| Best-Value Indicator | Highlights the cheapest and the fastest results. |
| Price Breakdown Expander | Reveals the itemised computation: base unit price, applied pricing rules and their effect, setup fee, and line totals. |
| Indicative Price Warning | Displayed when the quote came from the fallback path because the Quote Engine was unavailable. |
| Quote Expiry Countdown | Remaining validity of the generated quotes. |
| Select Shop Button | Proceeds to checkout with the chosen quote. |
| Edit Order Link | Returns to the order builder, invalidating the current quotes. |

### 3.4.3 Checkout Screen

**Figure 34: Checkout Screen**

**Description:** Final confirmation screen before an order is placed and paid from the wallet.
**Main Features:** Confirms items and price, selects fulfilment, applies a voucher, and places the order.

| Field Name | Description |
|---|---|
| Order Summary | Selected shop, item list with configurations, and line totals. |
| Fulfilment Method | Select between pickup and delivery. |
| Pickup Slot | Date and time window selector constrained to the shop's operating hours; required for pickup. |
| Delivery Address | Address input pre-filled from the profile default; required for delivery. |
| Voucher Code | Input applying a promotional code, with an apply and a remove action. |
| Price Summary | Subtotal, discount, and total payable. |
| Wallet Balance | Current balance with an inline warning and top-up link when insufficient. |
| Customer Note | Free-text note to the shop. |
| Place Order Button | Primary action. Disabled when the balance is insufficient or a required field is missing. |

### 3.4.4 Order Tracking Screen

**Figure 35: Order Tracking Screen**

**Description:** Screen showing a single order's current state, production progress, and complete transition history.
**Main Features:** Displays the status timeline, live progress during production, and the actions available in the current status.

| Field Name | Description |
|---|---|
| Order Header | Order code, shop name, placement date, and current status badge. |
| Status Timeline | Ordered list of transitions with timestamp, actor role, and recorded reason. |
| Production Progress Bar | Progress percentage; visible only while the order is in production. |
| Item List | Each item's service type, configuration, quantity, and line total. |
| Price Breakdown | Subtotal, discount, and total charged. |
| Pickup Information | Shop address, operating hours, and the order code to present; shown when ready for collection. |
| Cancel Order Button | Visible only while cancellation is permitted; displays the refundable amount before confirming. |
| Confirm Receipt Button | Visible when the order is ready for collection or out for delivery. |
| Write Review Button | Visible after completion when no review exists. |
| Report a Problem Button | Visible after completion while the complaint window is open. |

### 3.4.5 My Orders Screen

**Figure 36: My Orders Screen**

**Description:** Screen listing the customer's own orders with filtering and pagination.
**Main Features:** Filters by status, shop, service group, and date range; sorts and paginates; opens an order.

| Field Name | Description |
|---|---|
| Status Filter | Multi-select over order statuses. |
| Shop Filter | Select over shops the customer has ordered from. |
| Date Range Filter | Start and end date bounding the placement date. |
| Sort Selector | Orders by placement date or total amount. |
| Order Row | Order code, shop, item summary, total, status badge, and placement date. |
| Reorder Action | Creates a new draft order from a completed order. |
| Pagination Control | Page navigation with total result count. |

## 3.5 Wallet

### 3.5.1 Wallet Screen

**Figure 37: Wallet Screen**

**Description:** Screen showing the current balance and the full transaction ledger.
**Main Features:** Displays balance, filters the ledger, links transactions to orders, and exports the statement.

| Field Name | Description |
|---|---|
| Balance Card | Current balance with a prominent top-up action. |
| Type Filter | Multi-select over top-up, payment, refund, and adjustment. |
| Date Range Filter | Start and end date bounding the transaction date. |
| Transaction Row | Type icon, signed amount, resulting balance, related order code, status, and timestamp. |
| Export Action | Downloads the filtered statement as CSV. |
| Pagination Control | Page navigation with total result count. |

### 3.5.2 Top Up Screen

**Figure 38: Top Up Screen**

**Description:** Screen generating a VietQR payload for wallet top-up by bank transfer.
**Main Features:** Accepts an amount, generates a unique reference code, and displays a scannable QR with transfer instructions.

| Field Name | Description |
|---|---|
| Amount Input | Numeric input validated against the permitted minimum and maximum. |
| Quick Amount Buttons | Preset amounts for convenience. |
| Generate QR Button | Creates the pending transaction and renders the QR code. |
| QR Code Display | Scannable VietQR image encoding account, amount, and reference code. |
| Bank Account Details | Account number, account name, and bank name shown as text for manual transfer. |
| Reference Code | The code the customer must enter in the transfer description, with a copy action. |
| Expiry Countdown | Remaining validity of the pending transaction. |
| Pending Status Indicator | Shows that confirmation is awaited and updates when the transfer is confirmed. |

## 3.6 Review and Complaint

### 3.6.1 Submit Review Screen

**Figure 39: Submit Review Screen**

**Description:** Screen allowing a customer to rate and review a shop after a completed order.
**Main Features:** Collects a rating and optional comment; enforces one review per order.

| Field Name | Description |
|---|---|
| Order Reference | Read-only summary of the reviewed order. |
| Rating Selector | Star selector from one to five; mandatory. |
| Comment | Free-text review body with a character limit. |
| Submit Button | Primary action; disabled until a rating is selected. |

### 3.6.2 Raise Complaint Screen

**Figure 40: Raise Complaint Screen**

**Description:** Screen allowing a customer to report a problem with a completed order.
**Main Features:** Collects a reason and description, tracks the shop's proposed resolution, and offers acceptance or escalation.

| Field Name | Description |
|---|---|
| Order Reference | Read-only summary of the order complained about. |
| Reason Selector | Select over wrong output, missing pages, quality defect, late, and other; mandatory. |
| Description | Free-text description of the problem; mandatory. |
| Submit Button | Creates the complaint and routes it to the shop. |
| Shop Response Panel | Displays the shop's written response and proposed resolution once submitted. |
| Accept Resolution Button | Applies the proposed resolution and closes the complaint. |
| Reject and Escalate Button | Escalates the complaint to platform administration. |
| Complaint Status Badge | Current status of the complaint. |

## 3.7 Shop Management (Desktop)

### 3.7.1 Shop Dashboard

**Figure 41: Shop Dashboard**

**Description:** The operational home screen for shop personnel, showing the live order queue and shop state.
**Main Features:** Groups orders by status, highlights overdue orders, and surfaces machine and stock alerts.

| Field Name | Description |
|---|---|
| Queue Columns | Orders grouped by status: awaiting acceptance, accepted, in production, failed, ready for pickup. |
| Order Tile | Order code, customer name, item summary, total, requested pickup slot, and elapsed waiting time. |
| Overdue Highlight | Visual emphasis on orders approaching or exceeding their estimated completion. |
| Machine Status Panel | Each machine with type and current status. |
| Low Stock Alerts | Materials at or below their low-stock threshold. |
| Search Box | Locates an order by code or customer name. |
| Refresh Indicator | Shows the time of the last automatic refresh. |

### 3.7.2 Order Detail & Production Panel

**Figure 42: Order Detail & Production Panel**

**Description:** Screen where shop personnel review an order, decide on it, and drive production.
**Main Features:** Inspects specifications and files, accepts or declines with a reason, assigns a machine and starts production, and records hand-over.

| Field Name | Description |
|---|---|
| Order Header | Order code, customer, status badge, placement time, and requested pickup slot. |
| Item Specification List | Each item's service type and full configuration. |
| File Preview / Download | Opens or downloads the attached file for verification. |
| Customer Note | Free-text note supplied by the customer. |
| Accept Button | Transitions the order to accepted. |
| Decline Button | Opens the decline dialog. |
| Decline Reason Selector | Mandatory select over the defined decline reasons, with an optional note. |
| Machine Selector | Lists idle machines of the required type for assignment. |
| Start Production Button | Publishes the production job and transitions the order. |
| Progress Indicator | Live progress percentage reported by the Production Agent. |
| Retry Production Button | Visible when production has failed; restarts at no charge. |
| Hand Over Button | Records collection and completes the order. |

### 3.7.3 Service & Pricing Management

**Figure 43: Service & Pricing Management**

**Description:** Shop Owner screen for publishing and maintaining the shop's rate card.
**Main Features:** Adds services from the platform catalogue, sets prices and lead times, and defines pricing rules.

| Field Name | Description |
|---|---|
| Service Group Tabs | Separates document, finishing, and fabrication services. |
| Add Service Button | Opens the platform service catalogue for selection. |
| Unit Price | Price per unit of measure for the selected service. |
| Setup Fee | Fixed amount added once per order item. |
| Minimum Quantity | Smallest orderable quantity for this service at this shop. |
| Lead Time | Baseline production minutes per unit, used in completion estimates. |
| Active Toggle | Controls whether the service appears publicly and in quote eligibility. |
| Pricing Rule Grid | Rows defining rule type, option key, multiplier, flat extra, and quantity band. |
| Add Rule Button | Adds a pricing rule to the selected service. |
| Save Button | Validates and persists the rate card entry and its rules. |

### 3.7.4 Machines & Materials Management

**Figure 44: Machines & Materials Management**

**Description:** Screen for maintaining the shop's machine registry and material stock.
**Main Features:** Registers machines and sets status; adjusts stock quantities and low-stock thresholds.

| Field Name | Description |
|---|---|
| Machine List | Machine name, type, served service group, and current status. |
| Add Machine Button | Registers a new machine. |
| Machine Status Selector | Sets idle, maintenance, or offline; blocked while the machine is assigned to an in-production order. |
| Material List | Material name, type, unit, current stock, and low-stock threshold. |
| Add Material Button | Registers a new material. |
| Stock Adjustment | Numeric input adjusting the current quantity; rejects negative results. |
| Low Stock Threshold | Level at which an alert is raised on the dashboard. |

### 3.7.5 Staff Management

**Figure 45: Staff Management**

**Description:** Shop Owner screen for granting and revoking operational access to the shop.
**Main Features:** Grants access by email, assigns a position, and revokes access.

| Field Name | Description |
|---|---|
| Staff List | Name, email, position, join date, and active indicator per staff member. |
| Grant Access Email | Input locating an existing platform account by email. |
| Position | Free-text label describing the staff member's role at the shop. |
| Grant Button | Creates the membership and elevates the account role. |
| Revoke Action | Deactivates the membership, immediately removing operational access. |

### 3.7.6 Shop Revenue Report

**Figure 46: Shop Revenue Report**

**Description:** Shop Owner screen presenting revenue and operational performance for the shop.
**Main Features:** Aggregates over a date range with grouping, charts the result, and exports it.

| Field Name | Description |
|---|---|
| Date Range Selector | Start and end date bounding the report period. |
| Grouping Selector | Groups by day, service group, or service type. |
| Summary Cards | Gross revenue, platform commission, net revenue, order count, and average order value. |
| Revenue Chart | Time series or breakdown chart matching the selected grouping. |
| Service Mix Chart | Distribution of revenue across service groups. |
| Machine Utilisation Table | Production time and job count per machine over the period. |
| Failure & Cancellation Rate | Proportion of orders declined, cancelled, or failed in production. |
| Export Button | Downloads the report as CSV or XML. |

## 3.8 Platform Administration (Desktop)

### 3.8.1 Admin Dashboard

**Figure 47: Admin Dashboard**

**Description:** Platform-level operational overview for administrators.
**Main Features:** Surfaces pending work items and headline platform metrics.

| Field Name | Description |
|---|---|
| Pending Applications Card | Count of shop applications awaiting review, linking to the review screen. |
| Escalated Complaints Card | Count of complaints awaiting adjudication, linking to the adjudication queue. |
| Pending Top-Ups Card | Count of wallet top-ups awaiting confirmation. |
| Platform Metrics | Active shops, registered customers, orders today, and revenue today. |
| Recent Activity Feed | Latest significant platform events. |

### 3.8.2 Shop Application Review

**Figure 48: Shop Application Review**

**Description:** Screen for reviewing and deciding on pending shop applications.
**Main Features:** Presents application detail, approves or rejects with a mandatory reason, and corrects unresolved coordinates.

| Field Name | Description |
|---|---|
| Application List | Applicant name, shop name, district, intended service groups, and submission date. |
| Application Detail | Full submitted information including address and operating hours. |
| Coordinate Correction | Editable latitude and longitude when address resolution failed. |
| Approve Button | Activates the shop and elevates the applicant's role. |
| Reject Button | Opens the rejection dialog. |
| Reason Input | Mandatory justification recorded with the decision. |

### 3.8.3 Shop Management

**Figure 49: Shop Management**

**Description:** Screen listing all shops with suspension and reinstatement controls.
**Main Features:** Filters and searches shops, reviews performance history, and suspends or reinstates.

| Field Name | Description |
|---|---|
| Status Filter | Multi-select over shop statuses. |
| Search Box | Locates a shop by name or owner email. |
| Shop Row | Shop name, owner, district, status, rating, order count, and complaint count. |
| Performance Detail | Complaint history, decline rate, and production failure rate for the selected shop. |
| Suspend Button | Opens the suspension dialog with a mandatory reason and an in-progress order count warning. |
| Reinstate Button | Returns a suspended shop to active status. |

### 3.8.4 User Management

**Figure 50: User Management**

**Description:** Screen for administering platform user accounts.
**Main Features:** Filters and searches users, locks and unlocks accounts, and records wallet adjustments.

| Field Name | Description |
|---|---|
| Role Filter | Multi-select over customer, shop owner, shop staff, and admin. |
| Status Filter | Select over active and locked. |
| Search Box | Locates a user by name or email. |
| User Row | Name, email, role, status, wallet balance, order count, and registration date. |
| Lock / Unlock Button | Changes account status; requires a mandatory reason. |
| Wallet Adjustment | Records an adjustment transaction with a mandatory reason; never edits the balance directly. |

### 3.8.5 Service Catalog & Commission

**Figure 51: Service Catalog & Commission Configuration**

**Description:** Screen maintaining the platform service type catalogue and the commission rate.
**Main Features:** Creates and deactivates service types, and configures the commission applied at order completion.

| Field Name | Description |
|---|---|
| Service Type List | Code, name, service group, pricing model, unit of measure, and active indicator. |
| Add Service Type Button | Creates a new catalogue entry. |
| Code | Unique stable identifier for the service type. |
| Service Group | Select over document, finishing, and fabrication. |
| Pricing Model | Select over per-page, per-unit, and material-and-time; determines the pricing strategy. |
| Requires File | Toggle indicating whether an order item of this type must reference an uploaded file. |
| Active Toggle | Controls availability platform-wide. |
| Commission Rate | Percentage applied to order totals at completion; validated against the permitted range. |
| Shops Offering Count | Number of shops currently offering the selected service type. |

### 3.8.6 Voucher Management

**Figure 52: Voucher Management**

**Description:** Screen for creating and monitoring promotional vouchers.
**Main Features:** Defines discount terms and validity, and tracks redemption against the usage limit.

| Field Name | Description |
|---|---|
| Voucher List | Code, discount, validity window, usage limit, redemption count, and status. |
| Code | Unique code entered by customers. |
| Discount Type | Select between percentage and fixed amount. |
| Discount Value | Percentage or fixed VND amount. |
| Minimum Order Amount | Order subtotal required for eligibility. |
| Maximum Discount Cap | Upper bound applied to percentage discounts. |
| Usage Limit | Total permitted redemptions. |
| Validity Window | Start and end date of redeemability. |
| Active Toggle | Controls whether the voucher can currently be applied. |

### 3.8.7 Escalated Complaint Adjudication

**Figure 53: Escalated Complaint Adjudication**

**Description:** Screen for making final rulings on complaints escalated beyond the shop.
**Main Features:** Presents the full evidence record and records a binding ruling with a mandatory justification.

| Field Name | Description |
|---|---|
| Escalation Queue | Complaints in escalated status with order code, shop, customer, reason, and age. |
| Order Evidence Panel | Order items, price breakdown, and the complete immutable status history. |
| Customer Statement | The complaint reason and description as submitted. |
| Shop Response | The shop's written response and proposed resolution, if any. |
| Shop History | The shop's prior complaint and adjudication record. |
| Ruling Selector | Select between upholding with a refund and rejecting the complaint. |
| Refund Amount | Amount to credit; validated not to exceed the order total. |
| Justification | Mandatory written rationale recorded with the ruling. |
| Submit Ruling Button | Applies the ruling, notifies both parties, and closes the complaint. |

### 3.8.8 Platform Reports & Export

**Figure 54: Platform Reports & Export**

**Description:** Screen presenting platform-wide reports with multi-format export.
**Main Features:** Aggregates across all shops, charts the result, and serves the report as JSON, XML, or CSV by content negotiation.

| Field Name | Description |
|---|---|
| Report Selector | Select over transaction and revenue summary, shop performance ranking, service mix, and failure analysis. |
| Date Range Selector | Start and end date bounding the report period. |
| Summary Cards | Order volume, gross transaction value, platform commission, and average order value. |
| Report Chart | Visualisation appropriate to the selected report. |
| Report Table | Tabular figures supporting the chart, sortable by column. |
| Format Selector | Chooses the export media type: JSON, XML, or CSV. |
| Export Button | Requests the report with the corresponding `Accept` header and downloads the response. |

### 3.8.9 Audit Log Viewer

**Figure 55: Audit Log Viewer**

**Description:** Read-only screen presenting the record of administrative and security-relevant actions.
**Main Features:** Filters by actor, action, entity, and date range; inspects before and after values.

| Field Name | Description |
|---|---|
| Actor Filter | Select over users who performed actions. |
| Action Filter | Select over recorded action types. |
| Entity Filter | Select over affected entity types. |
| Date Range Filter | Start and end date bounding the action timestamp. |
| Log Row | Timestamp, actor, action, entity name and identifier, and originating IP address. |
| Value Diff Panel | Serialized before and after state for the selected entry. |

---

# 4. Non-Functional Requirements

## 4.1 External Interfaces

**Table: External Interface Description**

| Interface Type | System / Provider | Description |
|---|---|---|
| Quoting | Quote Engine (internal, gRPC) | Receives order item specifications and shop pricing rules; returns itemised price breakdowns and estimated completion times. Invoked once per candidate shop during a comparison request. Failure is handled by falling back to published indicative rates. |
| Asynchronous Production | Message Broker (RabbitMQ, AMQP) | Carries production job messages from the platform to the Production Agent, and progress, completion, and failure events back. Configured with retry and a dead letter queue. |
| Payment | VietQR | Provides the QR payload standard encoding bank account, amount, and reference code for wallet top-up by bank transfer. |
| Geospatial | Routing Service | Returns distance and estimated travel duration between customer and shop coordinates, and resolves addresses to coordinates. |
| Communication | Gmail SMTP | Delivers transactional email: account verification, password reset, order status change, and complaint updates. |
| Data Exchange | Content Negotiation (JSON / XML / CSV) | Serves the same resources in multiple representations selected by the HTTP `Accept` header, supporting client consumption, accounting integration, and spreadsheet bookkeeping. |
| Database | SQL Server via Entity Framework Core | Persists all platform data. Schema is generated code-first and versioned through migrations. |

## 4.2 Quality Attributes

### 4.2.1 Safety & Security

**Credential protection.** All passwords must be hashed using BCrypt with a per-user salt. Plain-text passwords must never be stored, logged, or returned by any endpoint. Password fields must never appear in any API response, even in masked form.

**Token security.** Access tokens must be short-lived and carry only the claims required for authorization — user identifier, role, and shop membership. Refresh tokens must be persisted server-side and revocable, and must be revoked on logout, password change, and account lock. Client-side token removal alone must not be treated as session termination.

**Role-based and scoped access control.** The system must enforce permissions by role at every endpoint, and additionally enforce ownership scope: a customer may act only on their own orders, wallet, files, and complaints; shop personnel may act only on records belonging to a shop they are a member of. Cross-shop and cross-customer access must return 403 Forbidden.

**Query scope enforcement.** Server-side ownership filters must be applied to OData and list endpoints before any client-supplied query is evaluated, so that a crafted `$filter` cannot widen a result set beyond the caller's authorized scope.

**Uploaded document confidentiality.** Customer-uploaded files frequently contain personal data. Stored file paths must never be exposed to clients; files must be served only through an endpoint that verifies either ownership or an active fulfilment relationship. File access by shop personnel must be recorded.

**Sensitive data exposure.** Responses must never include password hashes, internal storage paths, refresh token values, or a shop's material unit cost.

**Auditability.** Administrative and security-relevant actions — shop approval and suspension, account lock, commission change, wallet adjustment, complaint adjudication — must be recorded with actor, action, target entity, before and after values, IP address, and timestamp, in an append-only log.

### 4.2.2 Performance & Reliability

**Quote comparison responsiveness.** A comparison request fans out across all eligible shops. Quote computation must be parallelised across candidate shops rather than executed sequentially, and the total response must remain within an acceptable interactive latency for a typical candidate set.

**Asynchronous long-running work.** Production jobs occupy machines for minutes to hours and must never be executed within the lifetime of an HTTP request. All production work must be dispatched through the message broker.

**Message durability and idempotency.** Production job messages must survive broker and consumer restarts. Event handlers must be idempotent: a redelivered progress or completion event for an order already in the target state must be ignored rather than producing a duplicate transition.

**Retry and dead lettering.** Message processing failures must be retried up to a configured limit, after which the message is routed to a dead letter queue for inspection. The affected order must remain in its last valid state and shop personnel must be alerted.

**Transactional integrity.** Operations spanning multiple entities — order placement with wallet debit, cancellation with refund, decline with refund, completion with commission — must execute within a single unit of work so that partial application is impossible. Wallet balance and ledger must never diverge.

**Graceful degradation.** Failure of the Quote Engine must not render the platform unusable: the system must fall back to published indicative rates and clearly mark the result as non-final. Failure of the Routing Service must return results without distance rather than failing the request.

**Concurrency safety.** Wallet balance changes and voucher redemption counts must be protected against concurrent modification so that a balance cannot be double-spent nor a usage limit exceeded.

### 4.2.3 Availability & Maintainability

**Layered architecture.** Business rules must reside in the application service layer. Controllers must be limited to receiving requests, invoking services, and mapping results to HTTP responses. Data access must be mediated through repository abstractions with a unit of work boundary.

**Data access abstraction.** Business logic must depend on repository and unit-of-work interfaces rather than on the persistence provider, so that the database provider can be substituted without changes to business code.

**Configuration externalisation.** Connection strings, broker endpoints, external service addresses, token lifetimes, commission defaults, and business thresholds must be supplied through configuration rather than embedded in code.

**Structured logging.** Requests, business operations, external service calls, and message handling must emit structured logs carrying a correlation identifier so that a single transaction can be traced across the API, the Quote Engine, and the Production Agent.

**Soft deletion.** User, shop, service type, machine, material, and document records must be logically deleted rather than physically removed, and excluded from queries by a global filter, so that historical orders retain resolvable references.

**Schema versioning.** The database schema must be generated code-first and evolved exclusively through versioned migrations, with seed data sufficient to demonstrate every workflow state.

**Reproducible environment.** Database and message broker must be provisioned through container composition so that the full system can be started on a clean machine without manual setup.

### 4.2.4 Usability

**Price transparency.** Every quote must be presented with an expandable breakdown showing which pricing rules were applied and their effect. A customer must never be shown a total they cannot decompose.

**Comparability.** Quote results must be directly comparable on the dimensions customers actually decide by — total price, completion time, distance, and rating — and re-rankable without recomputation.

**Progressive disclosure of options.** The order builder must present only the option fields required by the selected service type, so that a customer ordering a photocopy is not shown material and quality profile fields.

**Actionable error messages.** Business rule violations must produce messages that state what is wrong and what the customer can do about it — the required minimum, the permitted range, the reason for refusal — rather than generic failure text.

**Counter-optimised desktop workflow.** The shop client must support the operational rhythm of a physical counter: order lookup by code, a queue view grouped by status, and the ability to complete an order in a small number of interactions.

**Consistent list behaviour.** All list endpoints must return a standardised paged envelope carrying page number, page size, total count, and total pages, so that pagination behaves identically across every screen.

---

# 5. Requirement Append

## 5.1 Business Rules

| BR-ID | Rule Definition |
|---|---|
| BR-1 | Each email address may be associated with exactly one account. |
| BR-2 | A newly registered account is assigned the Customer role and a zero wallet balance. |
| BR-3 | Passwords must be stored as a BCrypt hash with a per-user salt; plain text must never be persisted. |
| BR-4 | A password must be at least 8 characters and contain at least one uppercase letter, one lowercase letter, and one digit. |
| BR-5 | Authentication failure must return a generic message that does not reveal whether the email exists. |
| BR-6 | An account with Locked status must not be issued an access token. |
| BR-7 | The desktop client must accept only Shop Owner, Shop Staff, and Admin roles; Customer-only accounts are rejected. |
| BR-8 | A password reset request must return the same neutral acknowledgement whether or not the email is registered. |
| BR-9 | A successful password reset or change must revoke all refresh tokens for the account. |
| BR-10 | Logout must revoke the refresh token server-side; client-side removal alone is not sufficient. |
| BR-11 | All logout and password change events must be recorded in the audit log. |
| BR-12 | A new password must not be identical to the current password. |
| BR-13 | A user may view and act on only their own records; identity is derived from the access token and never from a client-supplied parameter. |
| BR-14 | Password hashes must never be returned by any endpoint, even in masked form. |
| BR-15 | Display name must be between 2 and 100 characters. |
| BR-16 | Email address and account role cannot be changed by the account holder; both require administrator action. |
| BR-17 | All profile changes must be recorded in the audit log with user identifier and timestamp. |
| BR-18 | Notifications must be ordered newest first. |
| BR-19 | Notifications are marked as read once displayed to the recipient. |
| BR-20 | A user may view only notifications addressed to their own account. |
| BR-21 | Only shops with Active status are discoverable in search and eligible for quoting. |
| BR-22 | List endpoints must return a standardised paged envelope containing page number, page size, total count, and total pages. |
| BR-23 | Server-side scope filters must be applied before any client-supplied OData query is evaluated. |
| BR-24 | Only rate card entries marked active are displayed publicly; a shop's material unit cost is never exposed. |
| BR-25 | A shop may appear at most once in a customer's favourites; the pair is uniquely constrained. |
| BR-26 | A customer may view only their own favourites. |
| BR-27 | Uploaded files must match the permitted file type list and must not exceed the configured size limit. |
| BR-28 | A file cannot be uploaded without the customer accepting the intellectual property declaration. |
| BR-29 | A file attached to an order that is not yet Completed cannot be deleted. |
| BR-30 | Stored file paths must never be exposed to clients; files are served only through an endpoint verifying ownership or an active fulfilment relationship. |
| BR-31 | A shop is eligible for a quote only if it is Active, offers every service type in the order, has a non-offline machine for those service types, and has the required materials in stock. |
| BR-32 | The pricing strategy applied is determined by the service type's pricing model: per-page, per-unit with quantity tiers, or material-and-machine-time. |
| BR-33 | Every generated quote must record an itemised breakdown identifying which pricing rules were applied and their effect. |
| BR-34 | Every quote carries an expiry timestamp; an expired quote cannot be used to place an order. |
| BR-35 | When the Quote Engine is unavailable, the system returns published indicative rates marked as non-final rather than failing the request. |
| BR-36 | A voucher may be applied only within its validity window and while it is active. |
| BR-37 | A voucher may be applied only when the order subtotal meets its minimum order amount, and a percentage discount is capped at its maximum discount amount. |
| BR-38 | A voucher's redemption count is incremented only on successful order placement, not when the code is applied at checkout. |
| BR-39 | An order can be placed only against a valid, unexpired quote. |
| BR-40 | An order can be placed only when the wallet balance is at least the order total. |
| BR-41 | The selected quote's breakdown is snapshotted onto the order at placement; subsequent rate card changes never alter agreed terms. |
| BR-42 | Order placement, wallet debit, voucher increment, and history recording must occur within a single transaction. |
| BR-43 | A pickup slot must fall within the shop's operating hours. |
| BR-44 | Every order status transition must be recorded in an append-only history with actor, role, timestamp, and reason. |
| BR-45 | Order status history records must never be modified or deleted. |
| BR-46 | An order may be cancelled by the customer only while its status is AwaitingAcceptance or Accepted. |
| BR-47 | Cancellation in AwaitingAcceptance is refunded in full; cancellation in Accepted is refunded less the cancellation fee, which is retained by the shop. |
| BR-48 | An order that has entered production cannot be cancelled by the customer. |
| BR-49 | Only a Completed order may be used as the source for a reorder. |
| BR-50 | A reorder copies configuration but never copies price; the new order must be quoted at current rates. |
| BR-51 | An order reaches Completed when either the customer confirms receipt or shop personnel record hand-over. |
| BR-52 | Platform commission is computed and recorded at order completion using the commission rate current at that moment. |
| BR-53 | A customer's order queries are constrained server-side to their own orders regardless of client-supplied filters. |
| BR-54 | A top-up amount must fall within the configured minimum and maximum. |
| BR-55 | Wallet top-up requires a verified account email. |
| BR-56 | Every pending top-up carries a unique reference code enforced by a database constraint. |
| BR-57 | A pending top-up that is not confirmed within its validity window moves to Expired and does not affect the balance. |
| BR-58 | Every wallet transaction records the resulting balance, so the ledger can be verified independently against the stored balance. |
| BR-59 | Only a customer who owns a Completed order may review the fulfilling shop. |
| BR-60 | At most one review may exist per order, enforced by a unique constraint. |
| BR-61 | A shop's rating average and review count must be recomputed within the same transaction that creates a review. |
| BR-62 | A complaint may be raised only against a Completed order and only within the configured complaint window. |
| BR-63 | At most one open complaint may exist per order at any time. |
| BR-64 | A complaint resolution is either a zero-charge replacement order or a wallet refund; a refund may not exceed the order total. |
| BR-65 | A complaint escalates automatically when the shop does not respond within the configured response window. |
| BR-66 | A user may hold at most one shop in PendingReview or Active status at any time. |
| BR-67 | A shop application must contain all required fields before it may be submitted for review. |
| BR-68 | A shop may not receive orders until an administrator has approved it. |
| BR-69 | A shop's close time must be later than its open time. |
| BR-70 | Shop status cannot be changed by the shop owner; suspension and reinstatement are administrator actions. |
| BR-71 | Unit prices and setup fees must be non-negative; pricing multipliers must be positive. |
| BR-72 | Quantity tier bands within a single rate card entry must not overlap. |
| BR-73 | Deactivating a rate card entry removes the service from public display and quote eligibility but does not affect existing orders. |
| BR-74 | Only the Shop Owner may view or modify pricing; Shop Staff have no access to the rate card. |
| BR-75 | A machine currently assigned to an in-production order cannot be set to Offline. |
| BR-76 | Material stock quantity may not be negative. |
| BR-77 | A low-stock alert is raised when stock reaches or falls below the configured threshold. |
| BR-78 | Staff access may be granted only to an existing, Active platform account. |
| BR-79 | A shop owner cannot add their own account as staff of their own shop. |
| BR-80 | Revoking staff access removes operational access immediately but preserves the actor identity recorded on historical actions. |
| BR-81 | Revenue reports include only Completed orders. |
| BR-82 | Only the Shop Owner may view revenue reports; Shop Staff have no access to revenue figures. |
| BR-83 | Shop personnel may view only orders belonging to a shop they are a member of; scope is enforced server-side. |
| BR-84 | A staff member belonging to more than one shop operates within one active shop context per session. |
| BR-85 | Declining an order requires a reason selected from the defined set. |
| BR-86 | Declining an order triggers an automatic, unconditional full refund to the customer's wallet. |
| BR-87 | An order may be accepted or declined only while its status is AwaitingAcceptance. |
| BR-88 | Production may be started only on an order in Accepted status, and only with an Idle machine of the required type. |
| BR-89 | Production work must be dispatched through the message broker and must never execute within an HTTP request. |
| BR-90 | Production event handlers must be idempotent; a redelivered event for an order already in the target state is ignored. |
| BR-91 | Restarting production after a failure incurs no additional charge to the customer. |
| BR-92 | An order may be handed over only while its status is ReadyForPickup or OutForDelivery. |
| BR-93 | A shop may respond to a complaint only while its status is Open. |
| BR-94 | A rejection, suspension, lock, or adjudication action requires a mandatory recorded reason. |
| BR-95 | A shop may not resolve a complaint against another shop; scope is enforced server-side. |
| BR-96 | Approving a shop application elevates the applicant's role to Shop Owner while preserving their ability to order as a customer. |
| BR-97 | Shop approval and rejection decisions must record the deciding administrator and timestamp in the audit log. |
| BR-98 | A shop application may be decided only while its status is PendingReview. |
| BR-99 | A suspended shop is excluded from search and quote eligibility, but its in-progress orders continue to completion. |
| BR-100 | Shop suspension and reinstatement must be recorded in the audit log. |
| BR-101 | Suspension is reversible; permanent shop removal is not supported. |
| BR-102 | An administrator cannot lock their own account. |
| BR-103 | Locking an account revokes all its refresh tokens. |
| BR-104 | A wallet balance may never be edited directly; corrections are recorded as Adjustment transactions with a mandatory reason. |
| BR-105 | Service type codes must be unique across the platform catalogue. |
| BR-106 | A service type's pricing model cannot be changed once orders exist for that service type. |
| BR-107 | Service types referenced by existing orders are deactivated rather than deleted, preserving referential integrity. |
| BR-108 | Voucher codes must be unique; percentage discounts must not exceed one hundred and discount values must be positive. |
| BR-109 | A voucher's validity end date must be later than its start date. |
| BR-110 | Only complaints in Escalated status may be adjudicated by an administrator. |
| BR-111 | An administrator's ruling on an escalated complaint is final within the system. |
| BR-112 | An upheld complaint refund is credited to the customer's wallet and recorded against the shop. |
| BR-113 | Reports are read-only aggregations computed from Completed orders and never modify data. |
| BR-114 | Report resources must be serialisable as JSON, XML, and CSV, selected by the HTTP `Accept` header. |

## 5.2 Application Messages List

| No | Message code | Message Type | Message Content |
|---|---|---|---|
| 1 | MSG-1 | Error | "Incorrect email or password." |
| 2 | MSG-2 | Error | "This email address is already registered. Please log in or reset your password." |
| 3 | MSG-3 | Error | "Password must be at least 8 characters and include an uppercase letter, a lowercase letter, and a digit." |
| 4 | MSG-4 | Success | "Account created. Please check your email to verify your address." |
| 5 | MSG-5 | Error | "Password confirmation does not match." |
| 6 | MSG-6 | Error | "This account has been locked. Please contact support." |
| 7 | MSG-7 | Error | "This application is for shop and administrator accounts. Please use the website to order." |
| 8 | MSG-8 | Info | "If an account with this email exists, a reset link has been sent." |
| 9 | MSG-9 | Success | "Your password has been reset. Please log in." |
| 10 | MSG-10 | Error | "This reset link has expired or has already been used. Please request a new one." |
| 11 | MSG-11 | Info | "You have been signed out on this device. Your session may remain active elsewhere until it expires." |
| 12 | MSG-12 | Success | "Your password has been changed. You will need to sign in again on other devices." |
| 13 | MSG-13 | Error | "The current password is incorrect." |
| 14 | MSG-14 | Error | "The new password must be different from your current password." |
| 15 | MSG-15 | Error | "Your profile could not be loaded. Please try again." |
| 16 | MSG-16 | Success | "Changes saved." |
| 17 | MSG-17 | Error | "Display name must be between 2 and 100 characters." |
| 18 | MSG-18 | Error | "Please enter a valid phone number." |
| 19 | MSG-19 | Info | "No notifications yet." |
| 20 | MSG-20 | Error | "Notifications could not be loaded. Please try again." |
| 21 | MSG-21 | Info | "No shops match your filters. Try widening your search." |
| 22 | MSG-22 | Error | "This shop could not be found." |
| 23 | MSG-23 | Info | "This shop is not accepting orders at the moment." |
| 24 | MSG-24 | Info | "You have not saved any shops yet." |
| 25 | MSG-25 | Success | "File uploaded to your library." |
| 26 | MSG-26 | Error | "This file type is not supported. Accepted types: PDF, DOCX, JPG, PNG, STL, DXF." |
| 27 | MSG-27 | Error | "This file exceeds the maximum size limit." |
| 28 | MSG-28 | Error | "This file is attached to an order in progress and cannot be deleted." |
| 29 | MSG-29 | Error | "You must confirm that you have the right to reproduce this file." |
| 30 | MSG-30 | Info | "No shop offers all the services in this order. Try removing an item or splitting the order." |
| 31 | MSG-31 | Warning | "Prices shown are indicative. The quoting service is temporarily unavailable and the final price may differ." |
| 32 | MSG-32 | Error | "Please complete all required options for this service." |
| 33 | MSG-33 | Success | "Voucher applied." |
| 34 | MSG-34 | Error | "This voucher code is not valid." |
| 35 | MSG-35 | Error | "This voucher is not currently valid." |
| 36 | MSG-36 | Error | "This voucher requires a minimum order value." |
| 37 | MSG-37 | Error | "This voucher is no longer available." |
| 38 | MSG-38 | Success | "Order placed. Your order code is {orderCode}." |
| 39 | MSG-39 | Error | "This quote has expired. Please compare prices again." |
| 40 | MSG-40 | Error | "Your wallet balance is not sufficient for this order." |
| 41 | MSG-41 | Error | "This shop is no longer accepting orders. Please choose another shop." |
| 42 | MSG-42 | Error | "The selected pickup time is outside this shop's operating hours." |
| 43 | MSG-43 | Error | "You do not have permission to access this record." |
| 44 | MSG-44 | Error | "This order could not be found." |
| 45 | MSG-45 | Success | "Order cancelled. {amount} has been refunded to your wallet." |
| 46 | MSG-46 | Error | "This order has already entered production and can no longer be cancelled." |
| 47 | MSG-47 | Error | "This order has already been closed." |
| 48 | MSG-48 | Error | "Only completed orders can be reordered." |
| 49 | MSG-49 | Error | "The files from this order are no longer in your library." |
| 50 | MSG-50 | Success | "Receipt confirmed. Would you like to review this shop?" |
| 51 | MSG-51 | Error | "This order is not ready for collection." |
| 52 | MSG-52 | Info | "No orders match your filters." |
| 53 | MSG-53 | Success | "Top-up confirmed. {amount} has been added to your wallet." |
| 54 | MSG-54 | Error | "Top-up amount must be between {min} and {max}." |
| 55 | MSG-55 | Error | "Please verify your email address before topping up your wallet." |
| 56 | MSG-56 | Info | "No transactions yet." |
| 57 | MSG-57 | Success | "Thank you. Your review has been posted." |
| 58 | MSG-58 | Error | "This action is only available for completed orders." |
| 59 | MSG-59 | Error | "You have already reviewed this order." |
| 60 | MSG-60 | Success | "Complaint resolved." |
| 61 | MSG-61 | Error | "The complaint window for this order has closed." |
| 62 | MSG-62 | Error | "A complaint is already open for this order." |
| 63 | MSG-63 | Success | "Your application has been submitted and is under review." |
| 64 | MSG-64 | Error | "You already operate a shop on the platform." |
| 65 | MSG-65 | Error | "You already have an application under review." |
| 66 | MSG-66 | Error | "Closing time must be later than opening time." |
| 67 | MSG-67 | Error | "You do not have permission to access this shop's data." |
| 68 | MSG-68 | Success | "Rate card updated." |
| 69 | MSG-69 | Error | "Prices must be zero or greater and multipliers must be greater than zero." |
| 70 | MSG-70 | Error | "Quantity tiers overlap. Please review the affected bands." |
| 71 | MSG-71 | Error | "This service type is not available in the platform catalogue." |
| 72 | MSG-72 | Error | "This machine is currently producing order {orderCode} and cannot be taken offline." |
| 73 | MSG-73 | Error | "Stock quantity cannot be negative." |
| 74 | MSG-74 | Success | "Staff access granted." |
| 75 | MSG-75 | Error | "No account was found with this email address." |
| 76 | MSG-76 | Error | "This person is already a staff member of your shop." |
| 77 | MSG-77 | Error | "You already have full access to your own shop." |
| 78 | MSG-78 | Info | "No data for the selected period." |
| 79 | MSG-79 | Info | "No orders in the queue." |
| 80 | MSG-80 | Success | "Order accepted." |
| 81 | MSG-81 | Success | "Order declined. The customer has been refunded in full." |
| 82 | MSG-82 | Error | "This order's status has changed. Current status: {status}." |
| 83 | MSG-83 | Error | "Please select a reason for declining this order." |
| 84 | MSG-84 | Success | "Order {orderCode} is ready for collection." |
| 85 | MSG-85 | Error | "No idle machine is available for this service." |
| 86 | MSG-86 | Error | "The production service is temporarily unavailable. Please try again shortly." |
| 87 | MSG-87 | Success | "Hand-over recorded. Order completed." |
| 88 | MSG-88 | Error | "No order was found with this code." |
| 89 | MSG-89 | Success | "Resolution recorded and sent to the customer." |
| 90 | MSG-90 | Error | "This complaint is no longer open for this action." |
| 91 | MSG-91 | Error | "The refund amount cannot exceed the order total." |
| 92 | MSG-92 | Success | "Shop approved and activated." |
| 93 | MSG-93 | Error | "This record's status has changed and no longer permits this action." |
| 94 | MSG-94 | Error | "A reason is required for this action." |
| 95 | MSG-95 | Success | "Shop suspended. In-progress orders will continue to completion." |
| 96 | MSG-96 | Success | "Account locked." |
| 97 | MSG-97 | Error | "You cannot lock your own account." |
| 98 | MSG-98 | Success | "Configuration updated." |
| 99 | MSG-99 | Error | "This code is already in use." |
| 100 | MSG-100 | Error | "Commission rate must be between {min} and {max} percent." |
| 101 | MSG-101 | Success | "Voucher created." |
| 102 | MSG-102 | Error | "The end date must be later than the start date." |
| 103 | MSG-103 | Error | "Discount value must be greater than zero, and percentage discounts must not exceed 100." |
| 104 | MSG-104 | Success | "Ruling recorded. Both parties have been notified." |
| 105 | MSG-105 | Error | "The requested format is not supported. Available formats: JSON, XML, CSV." |

---

**Kết thúc SRS.**
