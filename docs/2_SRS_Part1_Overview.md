# PRN232 PROJECT REPORT

## Report 2: Software Requirement Specification — **Part 1 of 3**

# PrintHub: Web-Based Multi-Vendor Printing & Fabrication Service Marketplace

**SUPERVISOR**

| Full Name | Role | Email | Mobile |
|---|---|---|---|
| ChiLP | Supervisor | | |

**STUDENT**

| Full Name | Role | Email | Mobile |
|---|---|---|---|
| Ngô Minh Nhật | Individual Project | ngominhnhat10a3@gmail.com | |

*- Hanoi, July 2026 -*

> **Cấu trúc tài liệu SRS (3 file, ghép lại thành 1 document Word):**
> - **Part 1** (file này): I. Record of Changes · II. Definition · III.1 Product Overview (Context Diagram, Main Workflows, Actors, Use Case list, Screen Flow, Screen Authorization, Non-Screen Functions, ERD, Data Dictionary)
> - **Part 2**: III.2 Use Case Specifications (42 use case)
> - **Part 3**: III.3 Functional Requirements · III.4 Non-Functional Requirements · III.5 Requirement Append (Business Rules, Application Messages)

---

## Table of Contents

| Section | Part |
|---|---|
| I. Record of Changes | 1 |
| II. Definition | 1 |
| &nbsp;&nbsp;1. Acronyms | 1 |
| &nbsp;&nbsp;2. List Figures and Tables | 1 |
| &nbsp;&nbsp;3. Legend | 1 |
| III. Software Requirement Specification | |
| &nbsp;&nbsp;1. Product Overview | 1 |
| &nbsp;&nbsp;&nbsp;&nbsp;1.1 System Context Diagram | 1 |
| &nbsp;&nbsp;&nbsp;&nbsp;1.2 Main Workflows | 1 |
| &nbsp;&nbsp;&nbsp;&nbsp;1.3 User Requirements | 1 |
| &nbsp;&nbsp;&nbsp;&nbsp;1.4 System Functionalities | 1 |
| &nbsp;&nbsp;2. Use Case Specifications | 2 |
| &nbsp;&nbsp;3. Functional Requirements | 3 |
| &nbsp;&nbsp;4. Non-Functional Requirements | 3 |
| &nbsp;&nbsp;5. Requirement Append | 3 |

---

# I. Record of Changes

| Date | A*M, D | In charge | Change Description |
|---|---|---|---|
| 20/7/2026 | A | NhatNM | Add SRS template structure |
| 20/7/2026 | A | NhatNM | Add Acronyms, Legend, System Context Diagram |
| 20/7/2026 | A | NhatNM | Add Main Workflows (5 flows), Actors, Use Case list (42 UC) |
| 20/7/2026 | A | NhatNM | Add Screen Flow, Screen Authorization, Non-Screen Functions |
| 20/7/2026 | A | NhatNM | Add ERD + Data Dictionary (21 entities) |
| | | | |

*\*A - Added, M - Modified, D - Deleted*

---

# II. Definition

## 1. Acronyms

| Acronym | Definition |
|---|---|
| PHS | PrintHub System |
| UC | Use Case |
| FR | Functional Requirement |
| NFR | Non-Functional Requirement |
| BR | Business Rule |
| MSG | Application Message |
| FE | Feature Element |
| LI | Limitation |
| API | Application Programming Interface |
| REST | Representational State Transfer |
| OData | Open Data Protocol |
| JWT | JSON Web Token |
| RBAC | Role-Based Access Control |
| DTO | Data Transfer Object |
| UoW | Unit of Work |
| EF Core | Entity Framework Core |
| ERD | Entity Relationship Diagram |
| PK | Primary Key |
| FK | Foreign Key |
| CRUD | Create, Read, Update, Delete |
| gRPC | Google Remote Procedure Call |
| MQ | Message Queue |
| AMQP | Advanced Message Queuing Protocol |
| DLQ | Dead Letter Queue |
| MVC | Model-View-Controller |
| MVVM | Model-View-ViewModel |
| WPF | Windows Presentation Foundation |
| SPA | Single Page Application |
| VietQR | Vietnamese standard QR format for bank transfers |
| SMTP | Simple Mail Transfer Protocol |
| ETA | Estimated Time of Completion |
| SLA | Service Level Agreement |
| FDM | Fused Deposition Modeling |
| STL | Stereolithography (3D model file format) |
| DPI | Dots Per Inch |
| B/W | Black and White |
| VAT | Value Added Tax |

## 2. List Figures and Tables

### 2.1 List Figures

| No. | Figure Title |
|---|---|
| Figure 1 | System Context Diagram Legend |
| Figure 2 | Use Case Diagram Legend |
| Figure 3 | Screen Flow Legend |
| Figure 4 | Entity Relationship Diagram Legend |
| Figure 5 | Main Workflows Legend |
| Figure 6 | System Context Diagram – Detail |
| Figure 7 | Shop Onboarding Workflow |
| Figure 8 | Quote Comparison & Order Placement Workflow |
| Figure 9 | Order Production & Fulfilment Workflow |
| Figure 10 | Wallet Top-Up & Payment Workflow |
| Figure 11 | Complaint & Resolution Workflow |
| Figure 12 | Actor Generalization Diagram |
| Figure 13 | Use Case Diagram – Guest & Common User |
| Figure 14 | Use Case Diagram – Customer |
| Figure 15 | Use Case Diagram – Shop Owner |
| Figure 16 | Use Case Diagram – Shop Staff |
| Figure 17 | Use Case Diagram – Admin |
| Figure 18 | Screen Flow – Customer Web Client |
| Figure 19 | Screen Flow – Shop Desktop Client |
| Figure 20 | Screen Flow – Admin Desktop Client |
| Figure 21 | Entity Relationship Diagram – Detail |
| Figure 22+ | Screen prototypes (see Part 3, Section 3) |

> **[CẦN BỔ SUNG]** Figure 1–5 (legend) vẽ lại theo template WonderTales. Figure 6–21 vẽ bằng draw.io. Figure 21 (ERD) nên vẽ bằng dbdiagram.io rồi export PNG — nhanh hơn draw.io nhiều và đẹp hơn.

### 2.2 List Tables

| No. | Table Title |
|---|---|
| Table 1 | System Context Diagram Legend |
| Table 2 | Use Case Diagram Legend |
| Table 3 | Screen Flow Legend |
| Table 4 | Entity Relationship Diagram Legend |
| Table 5 | Main Workflows Legend |
| Table 6 | External Entity Data Flow Description |
| Table 7 | Order Status Definition |
| Table 8 | Actor Description |
| Table 9 | Use Case List |
| Table 10 | Screen Authorization Matrix |
| Table 11 | Non-Screen Function Description |
| Table 12 | Data Dictionary |
| Table 13+ | Use Case Specifications (Part 2) |
| Table N | Business Rules (Part 3) |
| Table N+1 | Application Messages List (Part 3) |

## 3. Legend

### 3.1 System Context Diagram

**Figure 1: System Context Diagram Legend**

| No. | Element | Notation | Description |
|---|---|---|---|
| 1 | System | Circle / rounded rectangle at centre | Represents the PrintHub System as a single black box. |
| 2 | External Entity | Rectangle | Represents an actor or external system that exchanges data with PrintHub. |
| 3 | Data Flow | Solid arrow (→) | Indicates the direction in which data moves between an external entity and the system. |
| 4 | Data Flow Label | Text on arrow | Describes the content of the data being exchanged. |

**Table 1. System Context Diagram Legend**

### 3.2 Use Case Diagram

**Figure 2: Use Case Diagram Legend**

| No. | Element | Notation | Description |
|---|---|---|---|
| 1 | Actor | Stick figure | Represents a role that interacts with the system. |
| 2 | Use Case | Ellipse | Represents a discrete function the system provides to an actor. |
| 3 | System Boundary | Rectangle enclosing use cases | Delimits the scope of the system under specification. |
| 4 | Association | Solid line | Connects an actor to a use case they participate in. |
| 5 | Include | Dashed arrow labelled `«include»` | The base use case always executes the included use case. |
| 6 | Extend | Dashed arrow labelled `«extend»` | The extending use case executes conditionally from the base. |
| 7 | Generalization | Solid line with hollow triangle | Indicates that one actor or use case inherits from another. |
| 8 | Note | Rectangle with folded corner | Provides additional explanation or annotation. |

**Table 2. Use Case Diagram Legend**

### 3.3 Screen Flow

**Figure 3: Screen Flow Legend**

| No. | Element | Notation | Description |
|---|---|---|---|
| 1 | Screen / Page | Rectangle | Represents a user interface screen or page. |
| 2 | Navigation Flow | Solid arrow (→) | Indicates a transition between screens via navigation or button. |
| 3 | Modal / Popup | Ellipse | Represents a dialog, confirmation, or feedback overlay rather than a full screen. |
| 4 | Conditional Navigation | Diamond | Indicates a navigation branch determined by role or state. |

**Table 3. Screen Flow Legend**

### 3.4 Entity Relationship Diagram

**Figure 4: Entity Relationship Diagram Legend**

| No. | Element | Notation | Description |
|---|---|---|---|
| 1 | One-to-Many | Single line at the "one" side, crow's foot at the "many" side | One record in the parent entity relates to many records in the child entity. |
| 2 | Many-to-One | Crow's foot at the "many" side, single line at the "one" side | Many records in the child entity relate to one record in the parent entity. |
| 3 | Many-to-Many with attributes | Two one-to-many relationships through a junction entity | Modelled explicitly as a junction entity carrying its own attributes. |
| 4 | Optional One-to-One | Single line with hollow circles at both ends | A record may optionally relate to exactly one other record. |
| 5 | Entity / Table | Rectangle with entity name at top and attributes below | Represents a persisted table in the database. |
| 6 | Primary Key (PK) | Attribute marked PK | Uniquely identifies each record in the table. |
| 7 | Foreign Key (FK) | Attribute marked FK | References the primary key of a related table. |

**Table 4. Entity Relationship Diagram Legend**

### 3.5 Main Workflows

**Figure 5: Main Workflows Legend**

| No. | Element | Notation | Description |
|---|---|---|---|
| 1 | Process / Step | Rectangle | Represents a step or activity in the workflow. |
| 2 | Condition | Diamond | Indicates a decision point where the flow branches on a condition. |
| 3 | Flow | Solid arrow (→) | Shows the direction of activity or data flow between steps. |
| 4 | Start | White circle | Indicates the entry point of the workflow. |
| 5 | End | Black filled circle | Indicates the termination point of the workflow. |
| 6 | Asynchronous Message | Dashed arrow | Indicates a message published to or consumed from the message broker. |

**Table 5. Main workflows Legend**

---

# III. Software Requirement Specification

## 1. Product Overview

PrintHub is a multi-vendor marketplace that connects customers who need printing and fabrication services with independent print shops operating in their area. The system replaces the prevailing practice of ordering through personal chat accounts with a structured digital workflow: the customer configures an order once, receives automatically computed and directly comparable quotes from multiple nearby shops, selects a shop on the basis of price, speed, distance, or rating, and tracks the order through production to collection.

The platform serves three service groups, each with a distinct pricing model — document services priced per page, finishing and small-format products priced per unit, and digital fabrication priced by material consumption and machine time. A shop publishes a rate card covering whichever service groups its equipment supports, and the platform's quoting service applies that shop's own rules when computing a price.

The system is delivered as a RESTful Web API consumed by two purpose-built clients: a web application serving customers, and a Windows desktop application serving shop counters and platform administrators. Two supporting services complete the platform: a dedicated Quote Engine invoked over gRPC, which computes price and estimated completion time across many shops in a single comparison request, and a Production Agent consuming a message queue, which executes long-running print jobs asynchronously and publishes progress events back to the platform.

### 1.1 System Context Diagram

**Figure 6: System Context Diagram – Detail**

> **[CẦN BỔ SUNG]** Vẽ context diagram: PrintHub System ở giữa, các external entity xung quanh với mũi tên hai chiều ghi nhãn theo Table 6 dưới đây.

PrintHub connects five categories of human actor with four external or supporting systems into a single transaction environment.

**Guests** are unauthenticated visitors. They may browse the shop directory, inspect published rate cards, and read reviews, but cannot upload files, obtain a personalised quote, or place an order. This open surface exists so that price transparency — the platform's central value proposition — is visible before a visitor commits to registering.

**Customers** are authenticated users who place orders. They maintain a personal document library, configure order items against a shop's service catalogue, request comparative quotes, pay from an internal wallet, track order progress, collect completed orders, and afterwards submit reviews or raise complaints.

**Shop Owners** are authenticated users who have been approved to operate a shop on the platform. They control everything commercial about that shop: its storefront profile, its rate card and pricing rules, its machine registry, its material stock, its staff roster, and its revenue reporting.

**Shop Staff** are authenticated users granted operational access to a specific shop by its owner. They work the daily queue — accepting or declining incoming orders, executing production, reporting results, and handing over completed orders — but cannot modify pricing, manage staff, or view full revenue figures. This distinction is enforced by the system rather than by convention.

**Administrators** govern the marketplace itself. They review and approve shop applications, suspend and reinstate shops, manage user accounts, maintain the platform service catalogue and commission rate, adjudicate complaints escalated beyond the shop, and produce platform-wide reports.

Four systems support the platform. The **Quote Engine** is an internal gRPC service that computes price breakdowns and completion estimates from order specifications and shop pricing rules. The **Production Agent** is a message-queue consumer that simulates machine execution of print jobs and publishes progress, completion, and failure events. **VietQR** provides the QR code payload used for wallet top-up by bank transfer. The **Routing Service** returns distance and estimated travel time between a customer's location and a shop. **Gmail SMTP** delivers transactional email.

**Table 6. External Entity Data Flow Description**

| External Entity | Input to System (Data flow) | Output from System (Data flow) |
|---|---|---|
| Guest | Shop search criteria, shop detail request, registration information | Shop directory, shop rate cards, reviews, public statistics |
| Customer | Login credentials, uploaded files, order item configuration, quote comparison request, order confirmation, cancellation request, wallet top-up request, review, complaint | Authentication token, comparative quote list, order confirmation, order status updates, wallet balance and statement, VietQR payload, notifications |
| Shop Owner | Shop application, storefront profile updates, rate card and pricing rules, machine registry, material stock, staff assignment, complaint response | Application review result, shop order statistics, revenue and commission report, low-stock alerts, complaint records |
| Shop Staff | Order acceptance or decline with reason, production start, production result, hand-over confirmation | Incoming order queue, order specifications and attached files, machine status, production progress events |
| Administrator | Shop application decision, suspension action, user account action, service catalogue and commission configuration, voucher definition, complaint adjudication, report parameters | Pending application list, platform statistics, transaction and commission reports, complaint records, audit log |
| Quote Engine (gRPC) | Computed price breakdown, estimated completion minutes, applicable pricing rule trace | Order item specifications, shop pricing rules, machine availability, service type definitions |
| Production Agent (MQ) | Production progress percentage, completion event, failure event with reason | Production job message containing order identifier, item specifications, and assigned machine |
| VietQR | — | Bank account identifier, amount, reference code (encoded into QR payload) |
| Routing Service | Distance in metres, estimated travel duration | Customer coordinates, shop coordinates |
| Gmail SMTP | Delivery status | Transactional emails: account verification, password reset, order status change, complaint update |

### 1.2 Main Workflows

The system implements five principal workflows. Two of these — Shop Onboarding and the Order Lifecycle — are state machines with persisted status, explicit transition rules, and recorded transition history.

#### 1.2.1 Shop Onboarding Workflow

**Figure 7: Shop Onboarding Workflow**

A registered Customer who wishes to sell services on the platform submits a shop application containing the shop name, physical address, contact details, operating hours, and the service groups they intend to offer. The application is saved in `Draft` status while the applicant completes it, and moves to `PendingReview` on submission.

An Administrator reviews the pending application against the platform's verification requirements. On approval, the shop moves to `Active`, the applicant's account is elevated to the Shop Owner role, and the shop becomes discoverable in search. On rejection, the shop moves to `Rejected` with a recorded reason, and the applicant may revise and resubmit, returning the shop to `PendingReview`.

An `Active` shop may be moved to `Suspended` by an Administrator for policy violation. A suspended shop disappears from search results and cannot receive new orders, but its existing in-progress orders continue to completion so that customers are not stranded. An Administrator may later reinstate a suspended shop to `Active`.

**Shop status transitions**

| From | Action | To | Actor |
|---|---|---|---|
| — | Create application | Draft | Customer |
| Draft | Submit | PendingReview | Customer |
| PendingReview | Approve | Active | Admin |
| PendingReview | Reject | Rejected | Admin |
| Rejected | Revise and resubmit | PendingReview | Shop Owner |
| Active | Suspend | Suspended | Admin |
| Suspended | Reinstate | Active | Admin |

#### 1.2.2 Quote Comparison & Order Placement Workflow

**Figure 8: Quote Comparison & Order Placement Workflow**

The customer begins by uploading one or more files, or selecting previously uploaded files from their personal document library. For each file the customer creates an order item by selecting a service type and configuring the options that service type requires — copies, page count, paper type, colour mode, and sides for document services; quantity and finishing options for finishing services; material, quality profile, and quantity for fabrication services.

When the customer requests a comparison, the system determines the set of eligible shops: those in `Active` status that offer every service type present in the order, whose machines for those service types are not offline, and whose materials are in stock. For each eligible shop, the system invokes the Quote Engine, passing the order item specifications together with that shop's pricing rules. The Quote Engine returns a price breakdown and an estimated completion time. The system enriches each result with the distance and travel time from the Routing Service and the shop's current rating, and returns the set as a ranked, comparable list.

Each returned quote is persisted with an expiry timestamp. The customer selects one quote, optionally applies a voucher, chooses a fulfilment method and pickup slot, and confirms. On confirmation the system validates that the quote has not expired, that the wallet balance is sufficient, and that the shop is still accepting orders; it then deducts the order total from the wallet, snapshots the quote onto the order so that subsequent price changes cannot alter agreed terms, sets the order to `AwaitingAcceptance`, and notifies the shop.

If the customer does not confirm before the quote expires, the quote moves to `Expired` and a fresh comparison must be requested — this exists because material and paper costs, and therefore shop rate cards, change over time.

#### 1.2.3 Order Production & Fulfilment Workflow

**Figure 9: Order Production & Fulfilment Workflow**

An order in `AwaitingAcceptance` appears in the shop's incoming queue. Shop Staff review the attached files and specifications and either accept or decline. A decline requires a reason from a defined set (capacity unavailable, material out of stock, file unreadable, specification not supported, copyright concern), moves the order to `Declined`, and triggers an automatic full refund to the customer's wallet.

On acceptance the order moves to `Accepted`. Shop Staff assign a machine and start production, at which point the system publishes a production job message to the queue and the order moves to `InProduction`. The Production Agent consumes the message, executes the job, and publishes progress events which the platform applies to the order's progress indicator and forwards to the customer.

Production terminates in one of two states. On success the Agent publishes a completion event and the order moves to `ReadyForPickup`, or to `OutForDelivery` when the fulfilment method is delivery. On failure — a simulated machine fault — the Agent publishes a failure event with a reason and the order moves to `ProductionFailed`; Shop Staff then either restart production at no charge to the customer or decline the order with an automatic refund.

The order reaches `Completed` when the customer confirms receipt, or when Shop Staff record hand-over at the counter. At completion the platform commission is calculated and recorded against the order.

A customer may cancel an order before production begins. Cancellation in `AwaitingAcceptance` is refunded in full; cancellation in `Accepted` incurs a cancellation fee retained by the shop, since the shop has already committed capacity. Once the order is `InProduction`, cancellation is not permitted.

**Table 7. Order Status Definition**

| Status | Meaning | Next possible statuses |
|---|---|---|
| Draft | Order is being configured by the customer; not yet quoted | Quoted, (deleted) |
| Quoted | Comparative quotes generated; awaiting customer confirmation | AwaitingAcceptance, Expired |
| Expired | Quote validity elapsed before confirmation | (terminal — requires new quote) |
| AwaitingAcceptance | Order placed and paid; awaiting shop decision | Accepted, Declined, Cancelled |
| Accepted | Shop has accepted and committed capacity | InProduction, Cancelled (with fee) |
| InProduction | Production job dispatched; agent executing | ReadyForPickup, OutForDelivery, ProductionFailed |
| ProductionFailed | Machine fault reported by agent | InProduction (retry), Declined (refund) |
| ReadyForPickup | Production complete; awaiting customer collection | Completed |
| OutForDelivery | Production complete; in delivery | Completed |
| Completed | Order handed over to customer; commission recorded | (terminal; complaint may be raised) |
| Declined | Shop refused the order; customer fully refunded | (terminal) |
| Cancelled | Customer cancelled; refund applied per cancellation rule | (terminal) |

#### 1.2.4 Wallet Top-Up & Payment Workflow

**Figure 10: Wallet Top-Up & Payment Workflow**

The customer requests a top-up for a specified amount. The system creates a wallet transaction in `Pending` status with a unique reference code and generates a VietQR payload encoding the platform's bank account, the amount, and that reference code. The customer transfers the amount using their banking application, entering the reference code in the transfer description.

Confirmation is performed against the platform record: an Administrator matches the incoming transfer to the pending reference code and confirms it, at which point the transaction moves to `Completed` and the customer's balance is credited. A pending transaction that is not confirmed within its validity window moves to `Expired` and does not affect the balance.

Order payment deducts the total from the balance at confirmation, recorded as a `Payment` transaction. Refunds — arising from shop decline, valid cancellation, production failure, or an upheld complaint — are recorded as `Refund` transactions crediting the balance. Every transaction records the resulting balance, so the ledger is independently verifiable against the stored balance at any point.

#### 1.2.5 Complaint & Resolution Workflow

**Figure 11: Complaint & Resolution Workflow**

A customer may raise a complaint against a `Completed` order within a defined window, selecting a reason and providing a description. The complaint opens in `Open` status and is routed to the shop.

The shop responds with a proposed resolution: a free reprint, which creates a linked replacement order at zero charge, or a refund, which credits the customer's wallet. The complaint moves to `ShopResponded`. If the customer accepts the resolution, the complaint moves to `Resolved` and closes.

If the customer rejects the shop's resolution, or if the shop does not respond within the response window, the complaint moves to `Escalated` and is routed to an Administrator. The Administrator adjudicates, imposing either a refund from the shop's account or a rejection of the complaint, and the complaint moves to `Closed`. Administrator adjudication is final within the system.

### 1.3 User Requirements

#### 1.3.1 Actors

**Figure 12: Actor Generalization Diagram**

In the PrintHub system, **User** is the general actor representing all individuals interacting with the platform. Users are categorised by authentication status into **Guest** (not logged in, limited to public content) and **Common User** (authenticated, holding a registered account). From Common User, four specialised roles are derived: **Customer**, **Shop Owner**, **Shop Staff**, and **Admin**.

Two aspects of this model deserve emphasis. First, Shop Owner and Shop Staff are separated because their authority differs materially: an owner controls pricing, staffing, and revenue visibility, while staff perform operational work on the daily queue. Modelling them as one role would grant a part-time counter operator the ability to alter the shop's rate card. Second, authorization for both shop roles is **scoped to a specific shop** rather than global — a Shop Staff member of one shop must not be able to view or act on another shop's orders. This scope check is applied in addition to the role check on every shop-context operation.

A Customer may simultaneously hold the Shop Owner role, since a shop proprietor may also order from other shops. Role and shop membership are therefore modelled as separate concerns rather than a single mutually exclusive field.

**Table 8. Actor Description**

| No. | Actor | Description |
|---|---|---|
| 1 | Guest | Unauthenticated visitors. May browse the shop directory, view published rate cards, read reviews, and view platform statistics. Cannot upload files, obtain a personalised quote, or place an order. |
| 2 | Common User | Registered and authenticated users. This is the base actor for all specialised roles and owns account-level functions: logout, password change, profile view and update, and notifications. |
| 3 | Customer | Authenticated users who order services. They maintain a document library, configure orders, request comparative quotes, pay from an internal wallet, track and collect orders, and submit reviews and complaints. |
| 4 | Shop Owner | Authenticated users approved to operate a shop. They control the storefront profile, rate card and pricing rules, machine registry, material stock, staff roster, and revenue reporting for their own shop only. |
| 5 | Shop Staff | Authenticated users granted operational access to a specific shop by its owner. They process the order queue — accept, decline, produce, hand over — and respond to complaints, but cannot modify pricing, manage staff, or view full revenue. |
| 6 | Admin | Platform operators with full system privileges. They approve and suspend shops, manage user accounts, maintain the service catalogue and commission rate, manage vouchers, adjudicate escalated complaints, produce platform reports, and inspect the audit log. |
| 7 | Quote Engine | Internal supporting service invoked over gRPC. Computes itemised price breakdowns and estimated completion times from order specifications and a shop's pricing rules, for one or many shops in a single request. |
| 8 | Production Agent | Internal supporting service consuming production job messages from the broker. Simulates machine execution, publishing progress, completion, and failure events back to the platform. |
| 9 | VietQR | External payment standard. Provides the QR payload encoding bank account, amount, and reference code used for wallet top-up by bank transfer. |
| 10 | Routing Service | External geospatial service. Returns distance and estimated travel duration between customer and shop coordinates, used to rank quote comparison results. |
| 11 | Gmail SMTP | External email delivery service. Sends transactional email including account verification, password reset, order status change, and complaint updates. |

#### 1.3.2 Use Cases (UC)

**Table 9. Use Case List**

| ID | Use Case | Feature | Actor | Use Case Description |
|---|---|---|---|---|
| UC-01 | Register Account | Authentication | Guest | Allows a guest to create a new account using full name, email, phone number, and password. |
| UC-02 | Log In | Authentication | Guest | Allows a registered user to authenticate and receive an access token and refresh token. |
| UC-03 | Forgot Password | Authentication | Guest | Allows a user to reset a forgotten password via a verification link sent to their registered email. |
| UC-04 | Log Out | Profile Management | Common User | Allows an authenticated user to terminate their session and revoke the refresh token server-side. |
| UC-05 | Change Password | Profile Management | Common User | Allows a user to update their password after verifying the current one. |
| UC-06 | View Personal Profile | Profile Management | Common User | Allows a user to view their own account information. |
| UC-07 | Update Personal Profile | Profile Management | Common User | Allows a user to edit their display name, phone number, and default address. |
| UC-08 | View Notifications | Notification | Common User | Allows a user to view system notifications relating to their orders, complaints, and account. |
| UC-09 | Browse & Search Shops | Shop Discovery | Guest, Customer | Allows a visitor to browse the shop directory with filtering, sorting, and pagination by service group, area, rating, and price. |
| UC-10 | View Shop Detail | Shop Discovery | Guest, Customer | Allows a visitor to view a shop's profile, published rate card, machine list, and customer reviews. |
| UC-11 | Manage Favourite Shops | Shop Discovery | Customer | Allows a customer to add or remove shops from a personal favourites list and view that list. |
| UC-12 | Manage Document Library | Order Preparation | Customer | Allows a customer to upload, view, rename, and delete files in their personal document library. |
| UC-13 | Configure Order & Compare Quotes | Quoting | Customer | Allows a customer to configure order items against service types and receive comparable quotes from all eligible shops. |
| UC-14 | Apply Voucher | Quoting | Customer | Allows a customer to apply a promotional voucher code to a quoted order and see the recalculated total. |
| UC-15 | Place Order | Ordering | Customer | Allows a customer to confirm a selected quote, choose fulfilment method and pickup slot, and pay from the wallet. |
| UC-16 | Track Order Status | Ordering | Customer | Allows a customer to view the current status, progress, and full transition history of their order. |
| UC-17 | Cancel Order | Ordering | Customer | Allows a customer to cancel an order before production begins, with a refund computed by the cancellation rule. |
| UC-18 | Reorder Previous Order | Ordering | Customer | Allows a customer to create a new order pre-populated from a previous completed order. |
| UC-19 | Confirm Order Receipt | Ordering | Customer | Allows a customer to confirm collection of a completed order, moving it to Completed. |
| UC-20 | View Order History | Ordering | Customer | Allows a customer to browse, filter, and search their own past orders. |
| UC-21 | Top Up Wallet | Wallet | Customer | Allows a customer to request a wallet top-up and receive a VietQR payload with a unique reference code. |
| UC-22 | View Wallet Statement | Wallet | Customer | Allows a customer to view their balance and a filterable ledger of all wallet transactions. |
| UC-23 | Submit Shop Review | Review & Complaint | Customer | Allows a customer who has completed an order to rate and review the fulfilling shop. |
| UC-24 | Raise Order Complaint | Review & Complaint | Customer | Allows a customer to raise a complaint against a completed order and respond to the shop's proposed resolution. |
| UC-25 | Apply to Open Shop | Shop Onboarding | Customer | Allows a customer to submit an application to operate a shop on the platform. |
| UC-26 | Manage Shop Profile | Shop Management | Shop Owner | Allows an owner to maintain the shop storefront profile, address, contact details, and operating hours. |
| UC-27 | Manage Service & Pricing | Shop Management | Shop Owner | Allows an owner to publish and maintain the shop's rate card: offered service types, unit prices, pricing rules, lead times, and minimum quantities. |
| UC-28 | Manage Machines & Materials | Shop Management | Shop Owner, Shop Staff | Allows shop personnel to maintain the machine registry and material stock levels, and to set machine status. |
| UC-29 | Manage Shop Staff | Shop Management | Shop Owner | Allows an owner to grant, revoke, and list operational access for staff members of their shop. |
| UC-30 | View Shop Revenue Report | Shop Reporting | Shop Owner | Allows an owner to view revenue, commission, service mix, and machine utilisation reports for their own shop. |
| UC-31 | View Order Queue | Shop Operations | Shop Staff, Shop Owner | Allows shop personnel to view incoming and in-progress orders for their shop, filtered by status and priority. |
| UC-32 | Accept or Decline Order | Shop Operations | Shop Staff, Shop Owner | Allows shop personnel to accept an incoming order or decline it with a recorded reason, triggering an automatic refund. |
| UC-33 | Execute Production | Shop Operations | Shop Staff, Shop Owner | Allows shop personnel to assign a machine and start production, and to monitor progress reported by the Production Agent. |
| UC-34 | Hand Over Order | Shop Operations | Shop Staff, Shop Owner | Allows shop personnel to record hand-over of a completed order to the customer at the counter. |
| UC-35 | Respond to Complaint | Shop Operations | Shop Owner, Shop Staff | Allows shop personnel to respond to a customer complaint with a reprint or refund resolution. |
| UC-36 | Review Shop Application | Platform Administration | Admin | Allows an administrator to review a pending shop application and approve or reject it with a recorded reason. |
| UC-37 | Suspend or Reinstate Shop | Platform Administration | Admin | Allows an administrator to suspend an active shop for policy violation and later reinstate it. |
| UC-38 | Manage User Accounts | Platform Administration | Admin | Allows an administrator to view, search, lock, and unlock platform user accounts. |
| UC-39 | Manage Service Catalog & Commission | Platform Administration | Admin | Allows an administrator to maintain the platform-wide service type catalogue and configure the commission rate. |
| UC-40 | Manage Vouchers | Platform Administration | Admin | Allows an administrator to create, edit, deactivate, and monitor usage of promotional vouchers. |
| UC-41 | Adjudicate Escalated Complaint | Platform Administration | Admin | Allows an administrator to make a final ruling on a complaint escalated beyond the shop. |
| UC-42 | View & Export Platform Reports | Platform Reporting | Admin | Allows an administrator to view platform-wide reports and export them as JSON, XML, or CSV. |

**Use case diagrams by actor** — Figures 13 to 17.

> **[CẦN BỔ SUNG]** Vẽ 5 use case diagram tương ứng: (13) Guest & Common User: UC-01→UC-10; (14) Customer: UC-11→UC-25; (15) Shop Owner: UC-26→UC-35; (16) Shop Staff: UC-28, UC-31→UC-35; (17) Admin: UC-36→UC-42. Nhớ thể hiện `«include»` cho các luồng dùng chung, ví dụ UC-15 Place Order `«include»` UC-14 Apply Voucher là `«extend»` (tuỳ chọn), và UC-32 Decline `«include»` refund.

### 1.4 System Functionalities

#### 1.4.1 Screen Flow

**Figure 18: Screen Flow – Customer Web Client**
**Figure 19: Screen Flow – Shop Desktop Client**
**Figure 20: Screen Flow – Admin Desktop Client**

> **[CẦN BỔ SUNG]** Vẽ 3 screen flow theo danh sách màn hình bên dưới.

**Customer web client (ASP.NET Core MVC + JavaScript)**

`Landing Page` → `Shop Search` → `Shop Detail` are reachable without authentication. `Login` and `Register` lead to `Customer Dashboard`, from which the customer reaches `Document Library`, `Order Builder` → `Quote Comparison` → `Checkout` → `Order Tracking`, plus `My Orders`, `Wallet` → `Top Up`, `Favourites`, `Notifications`, `Profile`, and `Shop Application`. `Order Tracking` leads to `Submit Review` and `Raise Complaint` once the order is completed.

**Shop desktop client (WPF, MVVM)**

`Desktop Login` → `Shop Dashboard`, which is the operational home showing the order queue and machine status. From it: `Order Detail & Production Panel`, `Complaint Handling`, and — for Shop Owner only — `Shop Profile`, `Service & Pricing`, `Machines & Materials`, `Staff Management`, and `Revenue Report`.

**Admin desktop client (WPF, MVVM)**

`Desktop Login` → `Admin Dashboard`, leading to `Shop Application Review`, `Shop Management`, `User Management`, `Service Catalog & Commission`, `Voucher Management`, `Escalated Complaints`, `Platform Reports & Export`, and `Audit Log Viewer`.

#### 1.4.2 Screen Authorization

**Table 10. Screen Authorization Matrix**

| Screen | Guest | Customer | Shop Owner | Shop Staff | Admin |
|---|:---:|:---:|:---:|:---:|:---:|
| Landing Page | x | x | x | x | x |
| Shop Search | x | x | x | x | x |
| Shop Detail | x | x | x | x | x |
| Register | x | | | | |
| Login (web) | x | | | | |
| Forgot / Reset Password | x | x | x | x | x |
| Customer Dashboard | | x | x | | |
| Personal Profile | | x | x | x | x |
| Change Password | | x | x | x | x |
| Notifications | | x | x | x | x |
| Document Library | | x | x | | |
| Order Builder | | x | x | | |
| Quote Comparison | | x | x | | |
| Checkout | | x | x | | |
| Order Tracking (own order) | | x | x | | |
| My Orders | | x | x | | |
| Wallet & Statement | | x | x | | |
| Top Up (VietQR) | | x | x | | |
| Favourites | | x | x | | |
| Submit Review | | x | x | | |
| Raise Complaint | | x | x | | |
| Shop Application Form | | x | | | |
| Desktop Login | | | x | x | x |
| Shop Dashboard / Order Queue | | | x | x | |
| Order Detail & Production Panel | | | x | x | |
| Complaint Handling (shop) | | | x | x | |
| Shop Profile Management | | | x | | |
| Service & Pricing Management | | | x | | |
| Machines & Materials Management | | | x | x | |
| Staff Management | | | x | | |
| Shop Revenue Report | | | x | | |
| Admin Dashboard | | | | | x |
| Shop Application Review | | | | | x |
| Shop Management (suspend/reinstate) | | | | | x |
| User Management | | | | | x |
| Service Catalog & Commission | | | | | x |
| Voucher Management | | | | | x |
| Escalated Complaint Adjudication | | | | | x |
| Platform Reports & Export | | | | | x |
| Audit Log Viewer | | | | | x |

> **Ghi chú về scoped authorization:** đối với mọi màn hình có ngữ cảnh shop (Shop Dashboard, Order Detail, Machines & Materials, Complaint Handling, Revenue Report), dấu `x` chỉ có nghĩa là **shop của chính người dùng đó**. Hệ thống áp thêm một lớp kiểm tra membership: `ShopStaff` hoặc `Shop.OwnerId` phải khớp với shop của bản ghi đang truy cập. Truy cập chéo shop trả về `403 Forbidden`. Đây là điểm khác biệt so với RBAC phẳng và cần được nêu rõ khi vấn đáp.

#### 1.4.3 Non-Screen Functions

**Table 11. Non-Screen Function Description**

| No. | Feature | System Function | Description |
|---|---|---|---|
| **1. Authentication & Authorization** | | | |
| 1.1 | User Authentication | Credential Validation | Validate email and password against the stored BCrypt hash; never store or compare plain text. |
| 1.2 | User Authentication | Token Generation | Issue a JWT access token containing user identifier, role, and shop membership claims, plus an opaque refresh token persisted server-side. |
| 1.3 | User Authentication | Token Refresh & Revocation | Exchange a valid refresh token for a new access token; revoke the refresh token server-side on logout, password change, or account lock. |
| 1.4 | Password Management | Password Hashing | Hash all passwords using BCrypt with a per-user salt. |
| 1.5 | Access Control | Role Permission Check | Enforce endpoint-level permissions by role (Customer, Shop Owner, Shop Staff, Admin). |
| 1.6 | Access Control | Ownership Scope Check | Enforce that a user may act only on records they own — a customer on their own orders, wallet, and files; shop personnel on their own shop's records. |
| 1.7 | Access Control | OData Query Scope Injection | Inject a mandatory server-side filter into OData queries so that a customer's query is constrained to their own records and a shop's query to its own shop, regardless of the `$filter` supplied by the client. |
| **2. Quoting** | | | |
| 2.1 | Quote Computation | Determine Eligible Shops | Select shops that are Active, offer every service type in the order, have a non-offline machine for those service types, and have sufficient material stock. |
| 2.2 | Quote Computation | Invoke Quote Engine | Send order item specifications and the shop's pricing rules to the Quote Engine over gRPC and receive an itemised price breakdown and estimated completion minutes. |
| 2.3 | Quote Computation | Apply Pricing Strategy | Select the pricing strategy matching the service type's pricing model: per-page, per-unit with quantity tiers, or material-and-machine-time. |
| 2.4 | Quote Computation | Enrich with Distance | Call the Routing Service to obtain distance and travel duration between the customer's location and each candidate shop. |
| 2.5 | Quote Computation | Rank Comparison Results | Order the returned quotes by the customer's chosen criterion (price, completion time, distance, or rating). |
| 2.6 | Quote Computation | Persist Quote with Expiry | Store each generated quote with its breakdown and expiry timestamp so the agreed terms are reproducible. |
| 2.7 | Quote Computation | Graceful Degradation | If the Quote Engine is unavailable, return the shop's published indicative rates with a clear indication that the price is not final, rather than failing the request. |
| **3. Order Processing** | | | |
| 3.1 | Order Workflow | Validate State Transition | Check every requested status change against the transition table for the actor's role; reject invalid transitions with a business error. |
| 3.2 | Order Workflow | Record Transition History | Append every status transition to the order history with actor, role, timestamp, and reason. |
| 3.3 | Order Workflow | Snapshot Quote onto Order | Copy the selected quote's breakdown onto the order at confirmation so later rate card changes cannot alter agreed terms. |
| 3.4 | Order Workflow | Generate Order Code | Produce a unique, human-readable order code for counter reference. |
| 3.5 | Order Workflow | Compute Cancellation Refund | Apply the cancellation rule matching the order's current status to determine the refundable amount. |
| 3.6 | Order Workflow | Calculate Commission | On completion, compute and record the platform commission from the configured rate and the order total. |
| **4. Asynchronous Production** | | | |
| 4.1 | Production Dispatch | Publish Production Job | Publish a production job message to the broker containing the order identifier, item specifications, and assigned machine. |
| 4.2 | Production Dispatch | Consume Progress Events | Consume progress events published by the Production Agent and update the order's progress indicator. |
| 4.3 | Production Dispatch | Consume Completion Event | Consume the completion event and transition the order to ReadyForPickup or OutForDelivery. |
| 4.4 | Production Dispatch | Consume Failure Event | Consume the failure event, transition the order to ProductionFailed, and notify shop personnel. |
| 4.5 | Production Dispatch | Retry & Dead Letter | Retry message processing on transient failure up to a configured limit; route messages that exhaust retries to a dead letter queue for inspection. |
| 4.6 | Production Dispatch | Idempotent Event Handling | Ignore duplicate progress or completion events for an order already in the target state, so redelivery cannot corrupt order state. |
| **5. Wallet & Payment** | | | |
| 5.1 | Wallet | Generate VietQR Payload | Construct the VietQR payload encoding the platform bank account, amount, and unique reference code. |
| 5.2 | Wallet | Generate Reference Code | Produce a collision-free reference code for each pending top-up, enforced by a unique constraint. |
| 5.3 | Wallet | Apply Balance Change Atomically | Apply every balance change and its ledger entry within a single database transaction so balance and ledger cannot diverge. |
| 5.4 | Wallet | Expire Pending Top-Up | Move a pending top-up transaction to Expired when its validity window elapses without confirmation. |
| 5.5 | Wallet | Automatic Refund | Issue a refund transaction automatically on shop decline, valid cancellation, production failure, and upheld complaint. |
| **6. Notification** | | | |
| 6.1 | Notification | Publish Notification Event | Publish a notification event on every order status change, complaint update, and shop application decision. |
| 6.2 | Notification | Create In-App Notification | Consume notification events and persist an in-app notification for the affected user. |
| 6.3 | Notification | Send Transactional Email | Send email through SMTP for account verification, password reset, and order status changes. |
| **7. Reporting & Data Exchange** | | | |
| 7.1 | Reporting | Aggregate Report Data | Compute revenue, commission, service mix, machine utilisation, order volume, and failure rate over a requested date range. |
| 7.2 | Content Negotiation | Serialize by Accept Header | Serialize responses as JSON, XML, or CSV according to the client's HTTP `Accept` header, using the registered output formatters. |
| 7.3 | Content Negotiation | Validate Content-Type | Reject request bodies whose `Content-Type` is not among the media types declared as consumable by the endpoint. |
| 7.4 | Pagination | Standardised Paged Result | Return list responses in a consistent envelope carrying page number, page size, total count, and total pages. |
| **8. Cross-Cutting** | | | |
| 8.1 | Validation | Request Validation Pipeline | Validate every incoming command against its declared validation rules before it reaches business logic. |
| 8.2 | Transaction | Transaction Boundary | Wrap each business command in a single unit of work so that multi-entity operations commit or roll back atomically. |
| 8.3 | Error Handling | Global Exception Handling | Translate unhandled and domain exceptions into a consistent error response with an appropriate HTTP status code, without leaking internal detail. |
| 8.4 | Audit | Audit Log Recording | Record administrative and security-relevant actions with actor, action, target entity, before and after values, and timestamp. |
| 8.5 | Soft Delete | Logical Deletion | Mark user, shop, service type, machine, material, and document records as deleted rather than removing them, and exclude them from queries by a global filter. |
| 8.6 | Logging | Structured Logging | Emit structured logs for requests, business operations, external calls, and message handling with a correlation identifier. |

#### 1.4.4 Entity Relationship Diagram

**Figure 21: Entity Relationship Diagram – Detail**

> **[CẦN BỔ SUNG]** Vẽ ERD bằng dbdiagram.io từ data dictionary bên dưới rồi export PNG. Nhớ thể hiện rõ PK, FK và bội số quan hệ — đề bài Mục 6 yêu cầu đích danh.

**Relationship summary**

```
User 1───n RefreshToken
User 1───n Shop                    (as owner)
User 1───n ShopStaff               ─┐  n───n Shop  (junction WITH attributes)
Shop 1───n ShopStaff               ─┘
Shop 1───n ShopService             ─┐  n───n ServiceType (junction WITH attributes) ⭐
ServiceType 1───n ShopService      ─┘
ShopService 1───n PriceRule
Shop 1───n Machine
Shop 1───n Material
User 1───n DocumentFile
User 1───n Quote                   (as customer)
Shop 1───n Quote
Quote 0..1───0..1 Order            (selected quote snapshotted onto order)
User 1───n Order                   (as customer)
Shop 1───n Order
Order 1───n OrderItem
ServiceType 1───n OrderItem
DocumentFile 1───n OrderItem       (optional)
Order 1───n OrderStatusHistory
Order 1───0..1 Review
Order 1───n Complaint
Order 0..1───n WalletTransaction
User 1───n WalletTransaction
Voucher 1───n Order                (optional)
User 1───n Favourite               ─┐  n───n Shop (junction)
Shop 1───n Favourite               ─┘
User 1───n Notification
User 1───n AuditLog                (as actor)
Machine 0..1───n Order             (assigned machine, optional)
```

**Many-to-many relationships carrying attributes** (đề bài Mục 4 yêu cầu ít nhất một):

1. **`ShopService`** ⭐ — junction between `Shop` and `ServiceType`, carrying `UnitPrice`, `SetupFee`, `MinQuantity`, `LeadTimeMinutes`, `IsActive`. This is the central table of the platform: it *is* the shop's rate card, and it is read on every quote computation.
2. **`ShopStaff`** — junction between `User` and `Shop`, carrying `Position`, `JoinedAt`, `IsActive`. This is what makes scoped authorization possible.
3. **`Favourite`** — junction between `User` and `Shop`, carrying `CreatedAt`.

#### 1.4.5 Data Dictionary

**Table 12. Data Dictionary**

> Mọi entity đều kế thừa các trường audit chung: `CreatedAt`, `UpdatedAt`, và `IsDeleted` cho các entity áp dụng soft delete.

**User**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier, auto-increment |
| FullName | | Display name of the user, 2–100 characters |
| Email | UQ | Login identifier; unique across the platform |
| PhoneNumber | | Contact number, Vietnamese format |
| PasswordHash | | BCrypt hash with per-user salt; never returned by any API |
| Role | | User role: 0=Customer, 1=ShopOwner, 2=ShopStaff, 3=Admin |
| WalletBalance | | Current wallet balance in VND; changed only through WalletTransaction |
| DefaultAddress | | Default delivery address |
| Latitude, Longitude | | Default coordinates used for distance ranking |
| Status | | 0=Active, 1=Locked |
| EmailVerifiedAt | | Timestamp of email verification; null if unverified |
| IsDeleted | | Soft-delete flag |

**RefreshToken**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| UserId | FK | Owning user → User.Id |
| Token | UQ | Opaque refresh token value |
| ExpiresAt | | Expiry timestamp |
| RevokedAt | | Revocation timestamp; null while valid |

**Shop**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| OwnerId | FK | Shop owner → User.Id |
| Name | | Shop display name |
| Description | | Free-text shop description shown on the detail page |
| AddressLine, District, City | | Physical address of the shop |
| Latitude, Longitude | | Coordinates used for distance computation |
| PhoneNumber | | Shop contact number |
| OpenTime, CloseTime | | Daily operating hours |
| Status | | 0=Draft, 1=PendingReview, 2=Active, 3=Rejected, 4=Suspended |
| ReviewNote | | Administrator's reason on rejection or suspension |
| RatingAverage | | Aggregated rating, 1.0–5.0; recomputed on each new review |
| RatingCount | | Number of reviews contributing to the average |
| ApprovedAt, ApprovedBy | FK | Approval timestamp and approving administrator → User.Id |
| IsDeleted | | Soft-delete flag |

**ShopStaff** ⭐ *(n-n junction with attributes)*

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| ShopId | FK | Shop granting access → Shop.Id |
| UserId | FK | Staff member → User.Id |
| Position | | Free-text position label, e.g. "Counter", "Production" |
| JoinedAt | | Timestamp access was granted |
| IsActive | | Whether the access grant is currently in effect |

**ServiceType**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| Code | UQ | Stable code, e.g. `DOC_BW_A4`, `BIND_SPIRAL`, `PRINT_3D_FDM` |
| Name | | Display name of the service |
| ServiceGroup | | 0=Document, 1=Finishing, 2=Fabrication |
| PricingModel | | 0=PerPage, 1=PerUnit, 2=MaterialAndTime — selects the pricing strategy |
| UnitOfMeasure | | Unit in which quantity is expressed: page, unit, gram |
| RequiresFile | | Whether an order item of this type must reference an uploaded file |
| Description | | Description shown to customers |
| IsActive | | Whether the service type is available platform-wide |
| IsDeleted | | Soft-delete flag |

**ShopService** ⭐ *(n-n junction with attributes — the shop's rate card)*

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| ShopId | FK | Offering shop → Shop.Id |
| ServiceTypeId | FK | Offered service → ServiceType.Id |
| UnitPrice | | Price per unit of measure in VND |
| SetupFee | | Fixed fee added once per order item, e.g. machine setup |
| MinQuantity | | Minimum orderable quantity for this service at this shop |
| LeadTimeMinutes | | Baseline production time per unit, used to estimate completion |
| IsActive | | Whether the shop currently offers this service |
| UpdatedAt | | Last rate change timestamp |

**PriceRule**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| ShopServiceId | FK | Owning rate card entry → ShopService.Id |
| RuleType | | 0=PaperType, 1=ColorMode, 2=Sides, 3=BindingType, 4=Material, 5=QualityProfile, 6=QuantityTier |
| OptionKey | | Option this rule applies to, e.g. `A3`, `Color`, `Duplex`, `PLA`, `Fine` |
| Multiplier | | Multiplicative factor applied to the base price; 1.0 for no effect |
| FlatExtra | | Additive amount in VND applied when this option is selected |
| MinQuantity, MaxQuantity | | Quantity band for tier rules; null for non-tier rules |
| IsActive | | Whether the rule is currently applied |

**Machine**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| ShopId | FK | Owning shop → Shop.Id |
| Name | | Machine label used at the counter |
| MachineType | | 0=Printer, 1=Plotter, 2=Printer3D, 3=LaserCutter, 4=Finishing |
| ServiceGroup | | Service group this machine can serve |
| Status | | 0=Idle, 1=Busy, 2=Maintenance, 3=Offline |
| IsDeleted | | Soft-delete flag |

**Material**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| ShopId | FK | Owning shop → Shop.Id |
| Name | | Material name, e.g. "A4 80gsm", "PLA White" |
| MaterialType | | 0=Paper, 1=Filament, 2=Sheet, 3=Consumable |
| Unit | | Stock unit: sheet, gram, piece |
| StockQuantity | | Current quantity on hand |
| LowStockThreshold | | Level at which a low-stock alert is raised |
| UnitCost | | Shop's cost per unit, used in margin reporting |
| IsDeleted | | Soft-delete flag |

**DocumentFile**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| OwnerUserId | FK | Uploading customer → User.Id |
| FileName | | Original file name as uploaded |
| StoragePath | | Server-side storage path; never exposed directly to clients |
| ContentType | | MIME type of the stored file |
| FileSizeKb | | File size in kilobytes |
| DeclaredPageCount | | Page count declared by the customer at upload |
| RightsDeclared | | Whether the customer accepted the intellectual property declaration |
| UploadedAt | | Upload timestamp |
| IsDeleted | | Soft-delete flag |

**Quote**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| CustomerId | FK | Requesting customer → User.Id |
| ShopId | FK | Shop this quote was computed for → Shop.Id |
| SubTotal | | Computed price before discount, in VND |
| EstimatedMinutes | | Estimated production time returned by the Quote Engine |
| DistanceMeters | | Distance from the customer to the shop |
| BreakdownJson | | Itemised computation trace: which rules applied and their effect |
| IsIndicative | | True when produced by fallback because the Quote Engine was unavailable |
| ExpiresAt | | Quote validity deadline |
| CreatedAt | | Generation timestamp |

**Order**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| OrderCode | UQ | Human-readable code used at the counter, e.g. `PH-260720-0041` |
| CustomerId | FK | Ordering customer → User.Id |
| ShopId | FK | Fulfilling shop → Shop.Id |
| QuoteId | FK | Selected quote snapshotted onto this order → Quote.Id |
| MachineId | FK | Machine assigned at production start → Machine.Id; null before assignment |
| VoucherId | FK | Applied voucher → Voucher.Id; null if none |
| Status | | Order status per Table 7 |
| FulfilmentMethod | | 0=Pickup, 1=Delivery |
| PickupSlotStart, PickupSlotEnd | | Requested collection window |
| DeliveryAddress | | Delivery address when fulfilment method is Delivery |
| SubTotal | | Sum of order item line totals |
| DiscountAmount | | Discount applied from the voucher |
| TotalAmount | | Amount charged to the wallet |
| CommissionRate | | Platform commission rate snapshotted at completion |
| CommissionAmount | | Commission computed at completion |
| ProgressPercent | | Production progress reported by the agent, 0–100 |
| CustomerNote | | Free-text note from the customer to the shop |
| DeclineReason | | Reason recorded when the shop declines |
| PlacedAt, AcceptedAt, CompletedAt, CancelledAt | | Lifecycle timestamps |

**OrderItem** ⭐

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| OrderId | FK | Parent order → Order.Id |
| ServiceTypeId | FK | Service requested → ServiceType.Id |
| DocumentFileId | FK | Source file → DocumentFile.Id; null for services not requiring a file |
| Quantity | | Number of copies or units |
| PageCount | | Page count for document services |
| PaperType | | Paper option, e.g. `A4_80`, `A3_100` |
| ColorMode | | 0=BlackWhite, 1=Color |
| Sides | | 0=Simplex, 1=Duplex |
| BindingType | | Binding option for finishing services; null if not applicable |
| MaterialName | | Material selected for fabrication services |
| QualityProfile | | Quality profile for fabrication services: Draft, Standard, Fine |
| EstimatedGrams | | Estimated material consumption for fabrication services |
| UnitPrice | | Effective unit price after pricing rules, snapshotted |
| LineTotal | | Computed line total for this item |
| ItemNote | | Free-text note for this item |

**OrderStatusHistory**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| OrderId | FK | Order → Order.Id |
| FromStatus | | Status before the transition |
| ToStatus | | Status after the transition |
| ActorUserId | FK | User who performed the transition → User.Id; null for system transitions |
| ActorRole | | Role under which the transition was performed |
| Reason | | Reason or note recorded with the transition |
| CreatedAt | | Transition timestamp; records are append-only and never updated |

**WalletTransaction**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| UserId | FK | Wallet owner → User.Id |
| OrderId | FK | Related order → Order.Id; null for top-ups |
| Type | | 0=TopUp, 1=Payment, 2=Refund, 3=Adjustment |
| Amount | | Signed amount in VND; positive credits, negative debits |
| BalanceAfter | | Resulting balance, enabling independent ledger verification |
| RefCode | UQ | Unique reference code used to match a bank transfer |
| Status | | 0=Pending, 1=Completed, 2=Expired, 3=Failed |
| ConfirmedBy | FK | Administrator confirming a top-up → User.Id |
| CreatedAt, ConfirmedAt | | Lifecycle timestamps |

**Voucher**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| Code | UQ | Voucher code entered by the customer |
| DiscountType | | 0=Percent, 1=FixedAmount |
| DiscountValue | | Percentage or fixed VND amount |
| MinOrderAmount | | Minimum order total required for eligibility |
| MaxDiscountAmount | | Cap on the discount when the type is Percent |
| UsageLimit, UsedCount | | Total redemption limit and current redemption count |
| ValidFrom, ValidTo | | Validity window |
| IsActive | | Whether the voucher is currently redeemable |

**Review**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| OrderId | FK, UQ | Reviewed order → Order.Id; unique, enforcing one review per order |
| CustomerId | FK | Reviewing customer → User.Id |
| ShopId | FK | Reviewed shop → Shop.Id |
| Rating | | Integer rating 1–5 |
| Comment | | Free-text review body |
| ShopReply | | Shop's public reply to the review |
| CreatedAt, RepliedAt | | Timestamps |

**Complaint**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| OrderId | FK | Order complained about → Order.Id |
| CustomerId | FK | Complaining customer → User.Id |
| ShopId | FK | Shop complained about → Shop.Id |
| Reason | | 0=WrongOutput, 1=MissingPages, 2=QualityDefect, 3=Late, 4=Other |
| Description | | Customer's description of the problem |
| Status | | 0=Open, 1=ShopResponded, 2=Resolved, 3=Escalated, 4=Closed |
| ProposedResolution | | 0=Reprint, 1=Refund, 2=Rejected |
| ShopResponse | | Shop's written response |
| AdminRuling | | Administrator's final ruling when escalated |
| RefundAmount | | Amount refunded as part of the resolution |
| ReplacementOrderId | FK | Linked zero-charge reprint order → Order.Id; null unless resolution is Reprint |
| ResolvedBy | FK | User who resolved the complaint → User.Id |
| CreatedAt, RespondedAt, ResolvedAt | | Lifecycle timestamps |

**Favourite**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| CustomerId | FK | Customer → User.Id |
| ShopId | FK | Favourited shop → Shop.Id |
| CreatedAt | | Timestamp; composite unique on (CustomerId, ShopId) |

**Notification**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| UserId | FK | Recipient → User.Id |
| Title | | Notification headline |
| Content | | Notification body |
| Type | | 0=OrderStatus, 1=Complaint, 2=ShopApplication, 3=Wallet, 4=System |
| RelatedOrderId | FK | Related order → Order.Id; nullable |
| IsRead | | Whether the recipient has viewed the notification |
| CreatedAt | | Timestamp |

**AuditLog**

| Name | Key | Description |
|---|---|---|
| Id | PK | Unique identifier |
| ActorUserId | FK | User who performed the action → User.Id |
| Action | | Action performed, e.g. `ApproveShop`, `LockUser`, `UpdateCommission` |
| EntityName | | Name of the affected entity type |
| EntityId | | Identifier of the affected record |
| OldValue, NewValue | | Serialized before and after state |
| IpAddress | | Originating IP address |
| CreatedAt | | Timestamp; records are append-only |

---

**Kết thúc Part 1.** Tiếp tục ở `2_SRS_Part2_UseCases.md`.
