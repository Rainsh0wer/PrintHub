# PRN232 PROJECT REPORT

## Report 1: Project Introduction

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

---

## Table of Contents

| Section | Page |
|---|---|
| I. Record of Changes | 3 |
| II. Glossary and Legend | 4 |
| &nbsp;&nbsp;&nbsp;&nbsp;1/ Glossary | 4 |
| III. List Figures and Tables | 5 |
| &nbsp;&nbsp;&nbsp;&nbsp;2.1 List Figures | 5 |
| &nbsp;&nbsp;&nbsp;&nbsp;2.2 List Tables | 5 |
| IV. Project Introduction | 6 |
| &nbsp;&nbsp;&nbsp;&nbsp;1. Overview | 6 |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;1.1 Project Information | 6 |
| &nbsp;&nbsp;&nbsp;&nbsp;2. Project Background | 7 |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;2.1 National Context | 7 |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;2.2 Local Context | 8 |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;2.3 Why PrintHub Was Built: The Customer and Shop Problem | 9 |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;2.3.1 Opaque Pricing and Unpredictable Waiting Time | 9 |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;2.3.2 Manual, Chat-Based Order Intake Loses Work and Time | 10 |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;2.3.3 Small Print Shops Have No Affordable Digital Channel | 11 |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;2.4 Introduction about PrintHub | 12 |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;2.5 Legal & Regulatory Compliance | 13 |
| &nbsp;&nbsp;&nbsp;&nbsp;3. Existing Systems | 15 |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;3.1 Xometry | 15 |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;3.2 Hubs (formerly 3D Hubs) | 16 |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;3.3 Current Practice in Vietnam: Zalo/Facebook Ordering | 17 |
| &nbsp;&nbsp;&nbsp;&nbsp;4. Business Opportunity | 18 |
| &nbsp;&nbsp;&nbsp;&nbsp;5. Software Product Vision | 19 |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;5.1 Architectural Approach | 21 |
| &nbsp;&nbsp;&nbsp;&nbsp;6. Project Scope & Limitations | 22 |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;6.1 Major Features | 23 |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;6.2 Limitations & Exclusions | 25 |
| V. References | 26 |

---

# I. Record of Changes

| Date | A*M, D | In charge | Change Description |
|---|---|---|---|
| 20/7/2026 | A | NhatNM | Add template structure & content v1.0 |
| | | | |
| | | | |

*\*A - Added, M - Modified, D - Deleted*

> **Ghi chú sử dụng:** mỗi lần bạn sửa tài liệu, thêm một dòng vào bảng này. Giảng viên FPT thường kiểm tra Record of Changes để đánh giá quá trình làm việc, không chỉ kết quả cuối.

---

# II. Glossary and Legend

## 1/ Glossary

| Acronym | Definition |
|---|---|
| PHS | PrintHub System |
| FE | Feature Element |
| LI | Limitation |
| UC | Use Case |
| BR | Business Rule |
| MVP | Minimum Viable Product |
| SME | Small and Medium-sized Enterprise |
| API | Application Programming Interface |
| REST | Representational State Transfer |
| OData | Open Data Protocol |
| JWT | JSON Web Token |
| DTO | Data Transfer Object |
| UoW | Unit of Work |
| ORM | Object-Relational Mapping |
| EF Core | Entity Framework Core |
| gRPC | Google Remote Procedure Call |
| MQ | Message Queue |
| AMQP | Advanced Message Queuing Protocol |
| WPF | Windows Presentation Foundation |
| MVC | Model-View-Controller |
| MVVM | Model-View-ViewModel |
| SLA | Service Level Agreement |
| ETA | Estimated Time of Arrival / Completion |
| FDM | Fused Deposition Modeling (3D printing technology) |
| STL | Stereolithography file format (3D model) |
| PDF | Portable Document Format |
| QR | Quick Response code |
| VietQR | Vietnamese standard QR code format for bank transfers |
| RBAC | Role-Based Access Control |
| CRUD | Create, Read, Update, Delete |
| DLQ | Dead Letter Queue |

---

# III. List Figures and Tables

## 2.1 List Figures

| No. | Figure Title | Page |
|---|---|---|
| Figure 1 | Xometry Instant Quoting Interface | 15 |
| Figure 2 | Hubs Manufacturing Network Interface | 16 |
| Figure 3 | Current Zalo-based Print Ordering Practice | 17 |
| Figure 4 | PrintHub Feature Tree | 23 |

> **[CẦN BỔ SUNG]** Figure 1–3 là ảnh chụp màn hình bạn phải tự chụp từ website thật. Figure 4 vẽ bằng draw.io hoặc XMind.

## 2.2 List Tables

| No. | Table Title | Page |
|---|---|---|
| Table 1 | List of Feature Elements (FE) in the PrintHub System | 23 |
| Table 2 | List of Limitations (LI) | 25 |

---

# IV. Project Introduction

## 1. Overview

PrintHub is a web-based multi-vendor marketplace that connects customers who need printing and fabrication services with independent print shops in their area. Instead of physically visiting shops, sending files through chat applications, and waiting for a manually calculated price, a customer uploads their files once, configures the printing options, and immediately receives comparable quotes from multiple nearby shops — including price, estimated completion time, distance, and shop rating. The customer chooses a shop, places the order, and tracks its progress in real time until pickup or delivery.

The project was born from a problem that is familiar to almost every university student in Vietnam: during assignment and thesis deadlines, print shops around campus become bottlenecks. Students queue physically without knowing how long they will wait, send documents through Zalo and hope the shop does not lose or mix them up, and only learn the final price when they arrive to pay. On the other side of the counter, shop owners handle intake through personal chat accounts, calculate prices with a hand calculator, and have no systematic record of their own orders.

PrintHub reframes this transaction as a structured, auditable digital workflow. Every order carries an explicit price breakdown produced by a dedicated quoting service, moves through a well-defined state machine, and leaves a complete history that both the customer and the shop can inspect. The platform serves three distinct user groups through two purpose-built clients: a web application for customers, and a Windows desktop application for shop counters and platform administrators.

### 1.1 Project Information

- **Project name:** PrintHub – Web-Based Multi-Vendor Printing & Fabrication Service Marketplace
- **Vietnamese name:** PrintHub – Sàn kết nối dịch vụ in ấn & chế tạo
- **Project code:** PRN232
- **Student:** Ngô Minh Nhật (individual project)
- **Software type:**
  - ASP.NET Core Web API (backend, RESTful + OData)
  - ASP.NET Core MVC + JavaScript (customer web client)
  - WPF Desktop Application, MVVM (shop counter & platform admin console)
  - gRPC Service (Quote Engine)
  - .NET Worker Service + RabbitMQ (Production Agent)
- **Target users:**
  - **Customers** – students, office workers, and individuals who need document printing, finishing products, or digital fabrication services (web)
  - **Shop Owners** – proprietors of independent print and fabrication shops, who manage pricing, machines, staff, and revenue (desktop)
  - **Shop Staff** – counter operators who process the daily order queue but do not control pricing or view full revenue (desktop)
  - **Administrators** – platform operators who approve shops, adjudicate complaints, and produce platform-wide reports (desktop)

- **Service scope:** the marketplace covers three service groups, each with a distinct pricing model — **(A) Document services** priced per page, **(B) Finishing and small-format products** priced per unit, and **(C) Digital fabrication** priced by material consumption and machine time. A shop may offer any subset of these groups.

---

## 2. Project Background

### 2.1 National Context

Vietnam is in the middle of a broad shift of everyday commercial transactions onto digital platforms. Ride-hailing, food delivery, and e-commerce marketplaces have normalised a specific interaction pattern for Vietnamese consumers: browse providers, see a transparent price before committing, place an order in-app, track its status, and rate the provider afterwards. This pattern is now the baseline expectation for on-demand services.

The printing and reprographics sector has largely not followed. Printing is a mature, highly fragmented industry dominated by small independent shops — typically family-run businesses with one to five machines, operating in a fixed physical neighbourhood such as a university district, an office cluster, or a residential street. These businesses are exactly the segment that the Government's national digital transformation programme identifies as needing support, since they lack the capital and technical staff to build a digital channel on their own.

At the same time, two enabling conditions have matured. First, cashless payment infrastructure is now universally available: the VietQR standard allows any business to accept bank transfers by displaying a QR code, without a merchant contract or a card terminal. Second, digital fabrication has moved from industrial settings into small commercial shops — desktop 3D printers and small-format laser cutters are now affordable enough that neighbourhood print shops increasingly offer them alongside conventional document printing.

The result is a sector where demand is digital-native, supply is fragmented and offline, and the technical barriers to connecting the two have effectively disappeared.

> **[CẦN BỔ SUNG]** Chèn số liệu thật ở đoạn này để tăng sức nặng, ví dụ: quy mô ngành in Việt Nam, số lượng cơ sở in đăng ký, tỉ lệ thanh toán không tiền mặt. Nguồn gợi ý: Cục Xuất bản – In và Phát hành (Bộ TT&TT) báo cáo thường niên; Ngân hàng Nhà nước về thanh toán không dùng tiền mặt; Sách trắng Thương mại điện tử Việt Nam (Bộ Công Thương). **Chỉ trích dẫn số liệu bạn đã tự mở nguồn và đọc được** — không dùng số liệu chưa kiểm chứng.

### 2.2 Local Context

In Hanoi and other university-dense urban areas, printing demand is sharply concentrated in both time and space. Around each campus there is a cluster of print shops serving thousands of students, and demand spikes violently at the end of each term when assignments, reports, and theses are due simultaneously. During these peaks, the physical shop becomes a queue: students wait in line holding USB drives, and shop staff process jobs strictly in arrival order regardless of job size or urgency.

The intake channel is almost universally a personal chat account. Students send files to the shop through Zalo or Facebook Messenger, describe the requirements in free text ("in 2 mặt, giấy thường, đóng gáy lò xo, 3 bản"), and wait for a reply. This channel has no structure: requirements are expressed in natural language and can be misread, files arrive interleaved with unrelated conversations, and there is no order identifier that either side can reference later.

Pricing is equally unstructured. Each shop maintains its own rate sheet — usually on paper or in the owner's memory — combining page count, paper type, colour, single- or double-sided printing, binding method, and quantity discounts. Two shops on the same street can differ substantially in price for an identical job, but a customer has no practical way to compare without visiting or messaging each one individually.

Meanwhile, the same shops are beginning to offer 3D printing and laser cutting. These services introduce a fundamentally different operational profile: a single job can occupy a machine for many hours, jobs must be scheduled rather than served first-come-first-served, and pricing depends on material consumption and machine time rather than page count. Shops that manage document printing informally are poorly equipped to manage this second category at all.

PrintHub addresses this local reality directly: it gives customers a single place to compare and order across multiple nearby shops, and gives shops a structured intake and production queue that works for both fast document jobs and long-running fabrication jobs.

### 2.3 Why PrintHub Was Built: The Customer and Shop Problem

#### 2.3.1 Opaque Pricing and Unpredictable Waiting Time

The core frustration for customers is that neither the price nor the completion time is known before committing. A student who needs a 120-page report printed double-sided with spiral binding cannot determine the cost without asking a shop, and cannot determine when it will be ready without physically observing the queue. Because comparing shops requires repeating this process for each one, most customers do not compare at all — they default to the nearest shop and accept whatever price and wait they are given.

This produces two distinct harms. The first is financial: without comparison, price dispersion between shops persists, and customers systematically overpay at convenient locations. The second is time: because the completion time is unknown, the customer must either wait physically at the shop or make repeated trips. During deadline periods this waiting time is precisely when it is most costly.

The problem is symmetrical for the shop. Because the shop cannot signal its current load, it receives orders it cannot fulfil on time and must renegotiate or disappoint customers. A shop with idle machines three hundred metres away has no way to make that spare capacity visible to a customer standing in a queue.

> **[CẦN BỔ SUNG]** Đây là chỗ nên có khảo sát của chính bạn. Đề xuất: một Google Form ngắn (10 câu, n≈40–60) gửi cho sinh viên cùng trường, hỏi về: tần suất đi in, thời gian chờ trung bình mùa deadline, có từng so sánh giá giữa các tiệm không, đã từng gặp sự cố mất file/in sai chưa. Trích dẫn như tài liệu tham khảo `[n]` giống template WonderTales dùng khảo sát n=50. Khảo sát tự thực hiện là bằng chứng mạnh nhất và không thể bị bắt bẻ là bịa.

#### 2.3.2 Manual, Chat-Based Order Intake Loses Work and Time

Chat applications were not designed to be order management systems, and using them as one produces predictable failure modes.

**Requirements are ambiguous.** Printing options form a structured configuration — paper size, colour mode, sides, copies, binding, page ranges — but chat forces them into free-form prose. Ambiguity is resolved by the staff member's interpretation, and misinterpretation is only discovered after the job is printed, when the paper and time have already been consumed.

**Files and conversations get lost.** A shop account receives files from many customers in parallel, interleaved with unrelated messages. There is no index, no association between a file and an order, and no way to retrieve last semester's document when a customer asks for a reprint.

**There is no shared state.** Neither party has an authoritative view of the order's progress. The customer's only way to check status is to send another message; the shop's only way to track its own workload is memory and a stack of paper. Nothing is auditable: when a dispute arises about what was requested or what was promised, there is no record beyond a chat scroll.

**Nothing accumulates.** Because no structured data is retained, the shop cannot answer basic operational questions — which service earns the most, which day is busiest, how many jobs failed and had to be reprinted. The business runs on impression rather than measurement.

#### 2.3.3 Small Print Shops Have No Affordable Digital Channel

From the shop's perspective, the obvious remedy — build a website with online ordering — is out of reach. A small print shop cannot commission custom software, does not have staff to operate it, and would not generate enough traffic to justify it even if it did. The existing alternatives do not fit either: general e-commerce platforms are built for selling stock items from a catalogue, not for accepting a customer-supplied file and quoting a price computed from that file's properties.

The consequence is that shops compete almost exclusively on physical proximity. A shop's addressable market is the set of people who walk past it. Capacity cannot be traded across shops: one shop's overflow during a deadline peak is not another shop's revenue, it is simply demand that goes unserved or served badly.

A marketplace resolves this asymmetry. It gives each shop a digital storefront and structured order intake at effectively zero marginal cost, while giving customers the comparison and reach that no individual shop can provide alone. The shop keeps its own pricing, its own machines, and its own operating decisions; what it gains is visibility beyond its street and a system of record for its own operations.

These three problems — opaque pricing and waiting, unstructured chat-based intake, and the absence of an affordable digital channel for small shops — are the direct foundation on which PrintHub was designed. Every feature, every workflow state, and every architectural decision in this project traces back to solving one or more of them.

### 2.4 Introduction about PrintHub

PrintHub is a multi-vendor service marketplace consisting of three tightly integrated components: a web application for customers, a Windows desktop application for shop counters and platform administrators, and a backend platform composed of a RESTful Web API, a dedicated quoting service, and an asynchronous production pipeline.

The platform covers three groups of services, deliberately chosen because each requires a fundamentally different pricing model. **Group A – Document services** (black-and-white and colour printing, photocopying, large-format drawing plots) is priced per page, as a function of copies, page count, paper type, colour mode, and single- or double-sided output. **Group B – Finishing and small-format products** (spiral and thermal binding, lamination, business cards, decals, photo prints) is priced per unit with quantity-tier discounts. **Group C – Digital fabrication** (FDM 3D printing, laser cutting and engraving) is priced from material consumption and machine time. A shop offers whichever subset of these groups its equipment supports: a neighbourhood copy shop typically offers A and B, a makerspace offers C, and a larger shop offers all three.

The customer journey begins with discovery. A customer browses or searches shops filtered by service group, area, rating, and price, and inspects each shop's published rate card, machines, and reviews. When ready to order, the customer uploads one or more files and configures each item independently — paper size, colour mode, sides, copies, and binding for document and finishing services; material, quality profile, and quantity for fabrication services.

The distinguishing capability of the platform is **multi-shop quote comparison**. Rather than requesting a price from one shop at a time, the customer submits the configured order once, and the system computes a quote at every eligible shop using that shop's own pricing rules and current machine availability. The customer receives a ranked, comparable list — price, estimated completion time, distance, and rating — and selects the shop that best fits their constraint, whether that is cost, speed, or convenience.

Once an order is placed, it enters a defined workflow. The shop accepts or declines it, produces it, and reports completion; the customer tracks each transition and is notified as it happens. Because fabrication jobs can occupy a machine for hours, production is handled asynchronously through a message queue rather than synchronous request handling — progress events flow back from the production agent to the platform and onward to the customer without either side polling.

Payment is handled through an internal wallet. Customers top up via VietQR bank transfer and pay from their balance at order time, which keeps the transaction record inside the system and makes refunds — for cancellations, declines, and upheld complaints — a first-class operation rather than an off-platform negotiation.

Behind the customer-facing experience, shops manage their own storefront: rate cards, machine registry, incoming order queue, production status updates, and revenue reporting. Platform administrators govern the marketplace itself — reviewing and approving shop applications, suspending non-compliant shops, resolving escalated complaints, and producing platform-wide reports.

### 2.5 Legal & Regulatory Compliance

PrintHub operates as an e-commerce intermediary connecting independent service providers with consumers, and is therefore subject to a specific set of Vietnamese regulations. The following legislation directly shapes the platform's design decisions, data handling practices, and commercial operations.

> **[CẦN KIỂM CHỨNG]** Các văn bản dưới đây là có thật, nhưng **bạn phải tự mở và đọc điều khoản cụ thể trước khi nộp**, đặc biệt là số điều/khoản. Đừng trích dẫn điều luật mà bạn chưa mở nguồn — đây là loại lỗi giảng viên bắt được ngay khi hỏi lại.

#### 2.5.1 E-Commerce Platform Operation

Nghị định 52/2013/NĐ-CP on e-commerce, as amended by Nghị định 85/2021/NĐ-CP, establishes the obligations of an operator of a *sàn giao dịch thương mại điện tử* (e-commerce trading platform). These include publishing an operating regulation for the platform, verifying the identity of sellers registered on the platform, providing a mechanism for handling customer complaints, and ensuring price information is disclosed transparently before a transaction is concluded [1][2].

This regulation is the direct legal basis for three design decisions in PrintHub: the mandatory **shop application and administrator approval workflow** (FE-02) — no shop may receive orders without verification; the **complaint and resolution workflow** (FE-06) with administrator escalation; and the requirement that a **complete, itemised price breakdown** be presented and stored before the customer confirms an order (FE-03, FE-04). Price transparency is enforced at the data model level: an order cannot exist without an associated immutable quote snapshot.

#### 2.5.2 Personal Data Protection

Nghị định 13/2023/NĐ-CP on Personal Data Protection requires explicit consent before collecting, processing, or storing personal data, and obliges data controllers to provide deletion mechanisms and to limit processing to the stated purpose [3].

For PrintHub this has a sharper implication than for a typical marketplace, because the platform handles **customer-supplied documents** which frequently contain personal data — CVs, academic transcripts, identity documents, medical records. The platform's design therefore restricts file access to the customer who uploaded it and the specific shop fulfilling the order; establishes a retention policy after which uploaded files are purged; and records file access in an audit trail. These constraints shape FE-04 and the platform's authorization model.

#### 2.5.3 Electronic Invoicing

Nghị định 123/2020/NĐ-CP on invoices and documents, together with Thông tư 78/2021/TT-BTC, governs the issuance of electronic invoices in Vietnam. The prescribed format for an electronic invoice is **XML**, with a defined schema for invoice data [4][5].

This regulation is the reason PrintHub implements **content negotiation** as a first-class capability rather than a technical demonstration: the same order and revenue resources are served as JSON for the platform's own clients and as XML for invoice and accounting integration, selected by the client through the HTTP `Accept` header. Report resources additionally support CSV for spreadsheet-based bookkeeping, which is how most small shops actually maintain their accounts.

#### 2.5.4 Electronic Transactions

Luật Giao dịch điện tử 2023 establishes the legal validity of electronic contracts and electronic records, and sets requirements for the integrity and retrievability of electronic data messages used as evidence of a transaction [6].

PrintHub reflects this through its **immutable order history**: every state transition of an order is recorded with actor, timestamp, and reason in an append-only history table, and quotes are snapshotted at confirmation time rather than recomputed. This ensures that the terms the customer agreed to remain retrievable and unaltered for the life of the record — which is also what makes the complaint resolution workflow evidentially meaningful.

#### 2.5.5 Intellectual Property

Luật Sở hữu trí tuệ 2005, as amended in 2022, protects literary and artistic works against unauthorised reproduction [7]. A printing service is, mechanically, a reproduction service, and both the shop and the platform therefore carry exposure when customers submit copyrighted material — a concern that is concrete rather than theoretical in a university context, where wholesale reproduction of textbooks is common.

PrintHub addresses this through a **declaration of rights at upload time**, which the customer must accept before an order can be submitted, and through a **shop-side rejection reason** for copyright concerns that allows a shop to decline an order without penalty. The platform does not perform automated copyright detection; this is documented as an explicit limitation (LI-6).

---

## 3. Existing Systems

### 3.1 Xometry

**Figure 1: Xometry Instant Quoting Interface**

> **[CẦN BỔ SUNG]** Chụp màn hình trang instant quote của Xometry.

Xometry is an on-demand manufacturing marketplace that connects customers with a network of manufacturing partners for services including 3D printing, CNC machining, sheet metal fabrication, and injection moulding. Its defining feature is an instant quoting engine: a customer uploads a CAD file and receives an automated price and lead time without human involvement, computed from the geometry of the uploaded model together with the selected process, material, and finish.

**Link:** https://www.xometry.com/

**System Actors:**
- Customer — uploads designs, receives quotes, places orders
- Manufacturing Partner — accepts and produces jobs
- Admin — manages the partner network and platform operations

**Features:**
- Instant automated quoting from uploaded CAD geometry
- Broad process coverage across multiple manufacturing technologies
- Automated matching of jobs to suitable partners
- Order tracking and quality assurance programme

**Strengths:**
- Instant quoting removes the single largest source of friction in custom manufacturing
- Very large partner network provides capacity and redundancy
- Mature quality and dispute handling processes
- Proven marketplace economics at industrial scale

**Weakness / Gap:**
- Targets industrial and B2B customers; not designed for individual consumers or small jobs
- Does not cover document printing, which is the highest-frequency everyday printing need
- Partner selection is performed by the platform algorithm, so the customer does not compare and choose the provider
- No presence in Vietnam and no localisation for the Vietnamese market
- Physical proximity is not a selection factor, since fulfilment is by shipping rather than local pickup

### 3.2 Hubs (formerly 3D Hubs)

**Figure 2: Hubs Manufacturing Network Interface**

> **[CẦN BỔ SUNG]** Chụp màn hình trang quote của Hubs.

Hubs began as a distributed network of local 3D printing providers, allowing customers to find and order from a printer owner near them, and later consolidated into a curated manufacturing network offering online quoting for 3D printing, CNC machining, and injection moulding. Its original model — connecting customers to nearby independent machine owners — is the closest existing analogue to PrintHub's design.

**Link:** https://www.hubs.com/

**System Actors:**
- Customer — uploads models, orders parts
- Local Provider / Hub — offers machine capacity and fulfils jobs
- Admin — curates the network and manages quality

**Features:**
- Online quoting for multiple manufacturing processes
- Provider network with capability and location attributes
- Design-for-manufacturing feedback on uploaded models
- Order management and tracking

**Strengths:**
- Originally validated the exact premise PrintHub adopts: independent local machine owners can be aggregated into a marketplace
- Strong technical quoting capability
- Good customer-facing guidance for non-expert users

**Weakness / Gap:**
- Moved away from the local, peer-to-peer model toward a curated industrial network, abandoning the neighbourhood-scale use case
- Restricted to fabrication; no document printing
- No Vietnamese market presence or Vietnamese-language interface
- No support for local pickup workflows, walk-in counter operations, or the mixed service portfolio typical of a Vietnamese print shop
- Payment and invoicing model does not align with Vietnamese practice (VietQR transfer, electronic invoice requirements)

### 3.3 Current Practice in Vietnam: Zalo/Facebook Ordering

**Figure 3: Current Zalo-based Print Ordering Practice**

> **[CẦN BỔ SUNG]** Chụp màn hình một đoạn hội thoại đặt in qua Zalo (che thông tin cá nhân), hoặc ảnh chụp hàng đợi tại tiệm photo mùa deadline. Đây là hình ảnh thuyết phục nhất trong toàn bộ tài liệu vì nó cho thấy vấn đề thật.

The realistic baseline that PrintHub must improve upon is not a competing software product — it is the absence of one. In Vietnam, the overwhelming majority of print orders are placed through personal chat accounts on Zalo or Facebook Messenger, or in person at the counter.

**System Actors:**
- Customer — sends files and requirements as chat messages
- Shop Staff — reads messages, interprets requirements, quotes by hand, produces the job
- (No administrator, no platform, no system of record)

**Characteristics:**
- File transfer through a general-purpose chat application
- Requirements expressed as free-form natural language
- Price calculated manually per order from a paper or memorised rate card
- Payment in cash or by direct bank transfer, typically at pickup
- Order status communicated by ad-hoc messages on request

**Strengths:**
- Zero adoption cost — every customer and shop already has the application installed
- Extremely flexible: unusual requests are handled by simply describing them
- Direct human contact builds trust and allows negotiation
- Works without any technical skill on the shop's part

**Weakness / Gap:**
- No price transparency before ordering and no ability to compare shops
- No structured order record; disputes cannot be resolved from evidence
- Files are unindexed and frequently lost or confused between customers
- No queue visibility, so completion time is unpredictable for both sides
- No accumulated business data, so the shop cannot measure or improve its operations
- Requirements ambiguity causes rework — reprints consume paper, time, and goodwill
- Shop reach is limited to physical proximity; spare capacity cannot be discovered by customers

This comparison establishes PrintHub's position precisely. Xometry and Hubs prove that automated multi-provider quoting works, but serve industrial customers in Western markets and exclude document printing. Chat-based ordering serves the actual Vietnamese use case but provides no structure, transparency, or record. PrintHub occupies the gap between them: the automated quoting and provider comparison of the former, applied to the everyday, local, mixed-service printing need that the latter currently serves badly.

---

## 4. Business Opportunity

The printing services market in Vietnam is large, highly fragmented, and almost entirely undigitised at the point of customer interaction. This combination — high transaction volume, many small independent providers, and no incumbent platform — is the structural profile in which marketplace models have repeatedly succeeded in Vietnam, as demonstrated by ride-hailing, food delivery, and home services.

> **[CẦN BỔ SUNG]** Bổ sung số liệu thị trường có nguồn kiểm chứng được ở đây. Gợi ý nguồn: Sách trắng Thương mại điện tử Việt Nam (Bộ Công Thương, hằng năm); báo cáo ngành in của Cục Xuất bản – In và Phát hành; báo cáo thị trường in 3D toàn cầu (Grand View Research / Statista). Nếu không tìm được số liệu chắc chắn, **hãy viết định tính thay vì bịa số** — một đoạn định tính chặt chẽ tốt hơn một con số sai.

The demand side is favourable for three reasons. First, printing is a **recurring, high-frequency need** with a captive user base: a university student prints throughout every term, every year, and the demand renews with each incoming cohort. Second, demand is **acutely time-sensitive and geographically concentrated**, which means the value of price and availability transparency is highest exactly when volume is highest. Third, the target users are **digitally native** and already habituated to the browse–compare–order–track pattern from other service platforms, so adoption requires no behavioural change.

The supply side is equally favourable. Print shops are numerous, independent, and clustered around predictable locations, which makes them straightforward to onboard geographically. They have a concrete unmet need — visibility beyond their immediate street and a system of record for their own operations — and effectively no alternative means of obtaining it. Critically, onboarding cost is near zero: a shop needs no new equipment, only a rate card and a desktop application at the counter.

The emergence of **digital fabrication in small shops** adds a second, higher-value service tier on top of this base. Document printing provides transaction frequency and habit formation; 3D printing and laser cutting provide higher order values and a customer segment — engineering and design students, hobbyists, small product developers — for whom no convenient local channel currently exists at all.

The proposed product offers the following key opportunities:

1. **Aggregate fragmented local supply** into a single searchable marketplace, allowing customers to compare price, speed, and distance across nearby shops for the first time.
2. **Eliminate the quoting bottleneck** through automated, rule-based price computation, converting an interaction that currently takes hours of chat into seconds.
3. **Give small shops a zero-capital digital channel**, extending their reach beyond physical proximity and providing them with operational data they have never had.
4. **Serve both everyday and specialist printing needs** in one platform, using high-frequency document printing to build habit and higher-value fabrication to build margin.
5. **Generate sustainable revenue** through a commission on completed orders, aligning platform income directly with fulfilled transactions rather than upfront fees that small shops cannot pay.

The absence of any structured digital intermediary in a market this large and this fragmented represents both a clear commercial gap and a genuine efficiency gain for both sides of the transaction.

---

## 5. Software Product Vision

PrintHub is envisioned as the default way printing and fabrication services are discovered, priced, and ordered in Vietnam — a platform where a customer can find the right shop for a job in seconds instead of walking a street, and where a small print shop can operate with the transparency and reach of a much larger business.

The system is delivered through two specialised clients over a shared backend. This split is deliberate rather than incidental: the customer-facing experience is discovery- and comparison-heavy and must be reachable from any device without installation, which makes a web application the correct choice; the shop counter and platform administration experience is operational and repetitive — scanning order codes, updating production status, managing rate cards, monitoring machine state throughout a working day — which is better served by a persistent desktop application with dense information display and keyboard-driven workflows.

- **For customers:** Provide a transparent, comparable view of local printing supply — upload once, see real prices and real completion times from multiple shops, choose on the basis that matters to you, and track the order to completion without a single chat message. Provide a permanent record of what was ordered, at what price, from whom, so that reordering is one click and disputes are resolvable by evidence.

- **For shops:** Provide a complete counter operating system at no capital cost. Publish a structured rate card once and let the platform quote automatically and consistently; receive orders with unambiguous, machine-readable specifications instead of free-text chat; manage a production queue that accommodates both minute-scale document jobs and hour-scale fabrication jobs; and see, for the first time, measured data on revenue, service mix, machine utilisation, and failure rates.

- **For administrators:** Provide governance tooling for the marketplace itself — review and approve shop applications against verification requirements, suspend shops that violate platform policy, adjudicate complaints escalated beyond the shop, and produce platform-wide reporting on transaction volume, revenue, commission, and shop performance.

Unlike Xometry and Hubs, which apply automated quoting to industrial manufacturing for B2B customers in Western markets, PrintHub applies the same mechanism to the everyday, local, consumer-scale printing need — and unlike the chat-based ordering it replaces, it produces structure, transparency, and a durable record. In the longer term, PrintHub envisions expanding into scheduled and recurring printing for organisations, integrated delivery, and a shop-side analytics product built on the operational data the platform accumulates.

### 5.1 Architectural Approach

PrintHub is built on a **layered architecture with an asynchronous production pipeline**. The synchronous path — discovery, quoting, ordering, administration — is served by a RESTful Web API organised into distinct API, Application, Domain, and Infrastructure layers, with business rules concentrated in an application service layer and data access mediated by the Repository and Unit of Work patterns. The production path — the actual execution of print jobs — is handled asynchronously through a message broker.

This division is a direct consequence of the problem domain rather than a technology preference. A document printing job occupies a machine for minutes and a 3D printing job for hours; neither can be completed within the lifetime of an HTTP request. Modelling production as a queue of messages consumed by a production agent, which publishes progress events back to the platform, is the architecturally correct representation of work that is long-running, may fail partway, and must survive process restarts without being lost. The same mechanism naturally supports notification fan-out, since a single order event can be consumed independently by the customer notification path and the shop's queue display.

The quoting capability is separated into a dedicated service invoked over gRPC. This separation is likewise driven by the domain: quoting is a computation performed against many shops' pricing rules simultaneously when a customer requests a comparison, its rules change frequently as material and paper costs move, and it is the one component whose failure must degrade gracefully rather than take the platform down. Isolating it behind a defined contract allows pricing logic to evolve independently of the ordering system, and allows the platform to remain available — falling back to shop-published indicative rates — when the quoting service is unavailable.

Key benefits of this architecture for PrintHub include: **resilience**, since long-running production work is durable and retryable rather than tied to a request; **loose coupling**, since notification, production, and reporting consume order events independently without modifying the ordering logic; **auditability**, since every order transition and every quote is recorded as a discrete, traceable record; and **evolvability**, since pricing rules, additional service types, and new consumers of order events can be added without changes to the core ordering workflow.

---

## 6. Project Scope & Limitations

This project is developed individually over an approximately six-week period as the final project for PRN232. During the initial analysis phase, a considerably wider feature set was identified — including automated order dispatch and shop bidding, real-time customer–shop messaging, courier-integrated delivery, recurring scheduled printing for organisations, and automated shop payout and settlement. To ensure the project remains achievable, complete, and defensible within the available time, these were deliberately excluded.

The project adopts a **value-driven MVP approach**, concentrating on delivering one complete, high-quality vertical: a customer can discover shops, compare real quotes across them, place an order, and track it through production to completion; a shop can register, be approved, publish pricing, receive orders, produce them, and see its own performance; and an administrator can govern the marketplace and report on it. Every feature retained exists because it is required by that end-to-end path or by the regulatory obligations described in Section 2.5.

Two design decisions define the boundary of the scope. First, **the customer selects the shop explicitly** rather than the platform dispatching automatically — this preserves the price-comparison capability that is the product's core value while avoiding an entire matching, timeout, and reassignment subsystem. Second, **production is simulated** by a software agent rather than integrated with physical printer drivers — the message-driven pipeline, its state transitions, its progress reporting, and its failure handling are fully implemented and functionally complete; only the final hardware boundary is stubbed.

### 6.1 Major Features

**Figure 4: PrintHub Feature Tree - Detail**

> **[CẦN BỔ SUNG]** Vẽ feature tree bằng draw.io / XMind: gốc là "PrintHub", 7 nhánh FE-01..FE-07, mỗi nhánh liệt kê các chức năng con.

**Table 1: List of Feature Elements (FE) in the PrintHub System**

| FE Code | Feature Name | Description |
|---|---|---|
| **FE-01** | Authentication & Account Management | Handles account entry for all roles. Guests can browse shops and view public rate cards before registering. Includes registration (email, password, full name, phone), login issuing a JWT access token and refresh token, logout with server-side token revocation, password change, and personal profile view/update. Role-based access control distinguishes Customer, Shop, and Admin privileges. |
| **FE-02** | Shop Onboarding & Management | Covers the full lifecycle of a shop on the platform. A registered user applies to open a shop by submitting shop name, address, contact details, and offered service groups; an Administrator reviews and approves or rejects the application; approved shops become discoverable and may receive orders; non-compliant shops may be suspended and later reinstated. Shop Owners manage their own storefront profile, operating hours, rate card, machine registry, material stock indicators, and staff roster — granting counter staff operational access without exposing pricing control or revenue data. |
| **FE-03** | Shop Discovery & Quote Comparison | The customer-facing discovery experience. Browse and search shops with filtering, sorting, and pagination by service group, area, rating, and price; view a shop's detail page including its published rate card, machines, and customer reviews; save shops to a personal favourites list. The distinguishing capability is multi-shop quote comparison: the customer configures an order once and receives comparable quotes — itemised price, estimated completion time, distance, and rating — computed at every eligible shop using that shop's own pricing rules for the relevant service group. |
| **FE-04** | Order Placement & Fulfilment | The primary business workflow. Customers upload one or more files and configure each order item independently against the selected service type (paper size, colour mode, sides, copies, binding for Groups A and B; material, quality, quantity for Group C), select a fulfilment method and pickup slot, apply a voucher, and confirm the order against an immutable quote snapshot. The order then progresses through a defined state machine: the shop accepts or declines, production is executed asynchronously with progress reporting, the shop reports completion, and the customer collects. Customers may cancel under defined conditions, reorder a previous order, and access a personal document library of previously uploaded files. Every state transition is recorded with actor, timestamp, and reason. |
| **FE-05** | Wallet & Payment | Manages the internal customer wallet. Customers top up their balance by bank transfer using a generated VietQR code with a unique reference code, confirmed against the platform record. Order payment is deducted from the wallet balance at confirmation; refunds are issued automatically on shop decline, valid cancellation, production failure, or upheld complaint. Customers view a full wallet statement; shops and administrators view revenue and platform commission reporting. |
| **FE-06** | Review, Complaint & Resolution | Post-transaction features. Customers who have received a completed order may submit a rating and review of the shop, which aggregates into the shop's public rating used in discovery and comparison. Customers may also raise a complaint against a completed order (wrong output, missing pages, quality defect); the shop responds with a resolution — reprint at no charge or refund — and unresolved complaints are escalated to an Administrator for adjudication. |
| **FE-07** | Platform Administration & Reporting | Back-office module for Administrators. Manage platform user accounts including locking and unlocking; review, approve, reject, and suspend shops; adjudicate escalated complaints; configure the platform commission rate and voucher programmes; and produce platform-wide reports on transaction volume, revenue and commission, shop performance and ranking, and service mix. Reports are exportable in JSON, XML, and CSV through content negotiation. Administrative actions are recorded in an audit log. |

### 6.2 Limitations & Exclusions

**Table 2: List of Limitations (LI)**

**LI-1:** Payment is implemented as a wallet-based simulation. Top-up generates a valid VietQR code carrying the correct bank account and reference code, but confirmation of receipt is performed against a platform record rather than through a live banking API integration. No real funds are transferred.

**LI-2:** Shop payout and settlement are not implemented. Platform commission is calculated and reported per order and in aggregate, but the transfer of net revenue from the platform to shops is out of scope for this phase.

**LI-3:** Production is executed by a software agent that simulates machine behaviour — including elapsed time, progress reporting, and probabilistic failure — rather than integrating with physical printer or fabrication machine drivers. The message-driven pipeline, state transitions, progress events, retry handling, and failure compensation are fully implemented; only the hardware boundary is stubbed.

**LI-4:** Automated order dispatch and shop bidding are excluded. The customer selects the fulfilling shop explicitly. The platform does not automatically assign orders to shops, and shops do not compete through a bidding mechanism.

**LI-5:** Real-time text messaging between customer and shop is excluded. Communication is limited to structured order events, status notifications, decline reasons, and complaint correspondence.

**LI-6:** Automated content inspection of uploaded files is not performed. The platform does not detect copyrighted material, prohibited content, or page count from file contents. Page count and specifications are declared by the customer at upload and verified by the shop at acceptance; rights are asserted by the customer through an explicit declaration.

**LI-7:** Delivery is recorded as a fulfilment status only. No courier or logistics provider integration is implemented, and no delivery tracking beyond the shop-reported status is available.

**LI-8:** Distance and estimated travel time are computed for a single metropolitan area (Hanoi) using a public routing service. Multi-city operation, cross-region shipping, and international fulfilment are out of scope.

**LI-9:** No mobile application is developed. The customer client is a responsive web application accessible through a standard browser; the shop and administrator client is a Windows desktop application and is not available on other operating systems.

**LI-10:** Shops cannot publish rate cards directly to production without validation. Rate card changes take effect only after passing platform validation rules, and quotes already snapshotted onto confirmed orders are never retroactively modified by a subsequent price change.

---

# V. References

> **[QUAN TRỌNG — ĐỌC KỸ]**
> Danh sách dưới đây là các văn bản pháp luật và nguồn **có thật**, nhưng bạn **bắt buộc phải tự mở từng link, đọc, và xác nhận** trước khi nộp. Hai lý do:
> 1. Link văn bản pháp luật trên cổng thông tin Chính phủ thay đổi khá thường xuyên.
> 2. Nếu giảng viên hỏi "điều nào của nghị định này nói vậy?" mà bạn chưa đọc, mất điểm nặng hơn là không trích dẫn.
>
> Các mục `[8]`–`[12]` là chỗ trống cho số liệu thị trường và khảo sát — **chỉ điền khi bạn đã thực sự có nguồn**. Tài liệu không có số liệu vẫn đạt; tài liệu có số liệu bịa thì hỏng.

[1] Chính phủ Việt Nam. (2013). *Nghị định 52/2013/NĐ-CP về thương mại điện tử*. Hà Nội: Chính phủ.

[2] Chính phủ Việt Nam. (2021). *Nghị định 85/2021/NĐ-CP sửa đổi, bổ sung một số điều của Nghị định 52/2013/NĐ-CP về thương mại điện tử*. Hà Nội: Chính phủ.

[3] Chính phủ Việt Nam. (2023). *Nghị định 13/2023/NĐ-CP về bảo vệ dữ liệu cá nhân*. Hà Nội: Chính phủ.

[4] Chính phủ Việt Nam. (2020). *Nghị định 123/2020/NĐ-CP quy định về hóa đơn, chứng từ*. Hà Nội: Chính phủ.

[5] Bộ Tài chính. (2021). *Thông tư 78/2021/TT-BTC hướng dẫn thực hiện một số điều của Luật Quản lý thuế và Nghị định 123/2020/NĐ-CP về hóa đơn điện tử*. Hà Nội: Bộ Tài chính.

[6] Quốc hội Việt Nam. (2023). *Luật Giao dịch điện tử 2023 (Luật số 20/2023/QH15)*. Hà Nội: Quốc hội.

[7] Quốc hội Việt Nam. (2022). *Luật Sở hữu trí tuệ sửa đổi 2022 (Luật số 07/2022/QH15)*. Hà Nội: Quốc hội.

[8] **[CẦN BỔ SUNG]** Bộ Công Thương. *Sách trắng Thương mại điện tử Việt Nam [năm gần nhất]*. — dùng cho số liệu quy mô TMĐT và hành vi mua sắm trực tuyến ở Mục 2.1 và Mục 4.

[9] **[CẦN BỔ SUNG]** Cục Xuất bản, In và Phát hành – Bộ Thông tin và Truyền thông. *Báo cáo hoạt động ngành in [năm gần nhất]*. — dùng cho số liệu quy mô ngành in ở Mục 2.1 và Mục 4.

[10] **[CẦN BỔ SUNG]** Ngân hàng Nhà nước Việt Nam. *Số liệu thanh toán không dùng tiền mặt [năm gần nhất]*. — dùng cho lập luận về hạ tầng thanh toán ở Mục 2.1.

[11] **[CẦN BỔ SUNG]** Nguồn báo cáo thị trường in 3D (ví dụ: Grand View Research, Statista, hoặc Fortune Business Insights). — dùng cho Mục 4.

[12] **[CẦN BỔ SUNG]** Ngô Minh Nhật. (2026). *Khảo sát nhu cầu và trải nghiệm sử dụng dịch vụ in ấn của sinh viên* [Survey data, n=__]. Google Forms. — **đây là nguồn giá trị nhất trong danh sách**; xem ghi chú ở Mục 2.3.1.

[13] Xometry Inc. *Xometry — On-Demand Manufacturing*. https://www.xometry.com/ (truy cập __/__/2026)

[14] Hubs B.V. *Hubs — Custom Parts Manufacturing*. https://www.hubs.com/ (truy cập __/__/2026)

---

## Ghi chú chuyển sang Word

1. Copy từng phần vào file `.docx` dùng đúng style Heading 1/2/3 của template gốc để mục lục tự sinh được (`References → Table of Contents → Update Table`).
2. Bảng trong Markdown dán vào Word sẽ mất khung — dùng **Insert → Table** rồi paste nội dung, hoặc paste vào Excel trước rồi copy sang Word.
3. Đánh số Figure/Table bằng **Insert → Caption** để hai bảng ở Mục III tự cập nhật số trang.
4. Điền lại trang thật vào Table of Contents sau khi layout xong.
