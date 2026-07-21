# PrintHub diagrams — hybrid toolchain

Each diagram uses the language/tool that draws it *correctly* and fastest. Export
every one as **PNG** and insert into the Word document.

## Which tool for which diagram

| Diagram (doc section) | Tool | Source / how |
|---|---|---|
| **System Context** (like WonderTales fig.1) | **draw.io** | Manual. System = **rectangle** (NOT a DFD circle); actors = stick figures; external systems = `<<External system>>` boxes; arrows labelled "… via ►" with multiplicities. See spec below. |
| **Use Case** (per actor) | **PlantUML** | `usecase-customer.puml` (+ make owner/staff/admin similarly). Or draw.io UML shapes. |
| **ERD** (§4) | **dbdiagram.io** | `erd.dbml` — paste, auto-lays out crow's-foot with PK/FK. |
| **Order state machine** (§6) | **Mermaid** | `order-state.mmd` |
| **Shop onboarding state machine** (§6) | **Mermaid** | `shop-state.mmd` |
| **Quote comparison sequence** (§13) | **Mermaid** | `quote-compare-sequence.mmd` |
| **Order production swimlane/activity** (§6) | **PlantUML** | `order-production-swimlane.puml` |
| **Architecture** (§7) | **draw.io** | Manual boxes: clients → Api (REST/OData) → Application → Infrastructure (EF/SQL); Api ─gRPC→ QuoteEngine; Api ↔ RabbitMQ ↔ ProductionAgent. |
| **Screen Flow** | **draw.io** | Screens = rectangles; arrows = navigation; modals = ovals. |

## Rendered images

All diagrams are pre-rendered to **`png/`** (ready to insert into Word). To
regenerate after editing a source, run:

```
powershell -ExecutionPolicy Bypass -File docs/diagrams/render.ps1
```

It renders `.mmd` with mermaid-cli and `.puml` with PlantUML (Smetana layout — no
Graphviz needed). Prerequisites: Java, and `npm i -g @mermaid-js/mermaid-cli`.

## Render targets (online, no install)
- **Mermaid** (`.mmd`): https://mermaid.live — paste, then *Actions ▸ PNG*.
- **PlantUML** (`.puml`): https://www.plantuml.com/plantuml — paste, *PNG*.
- **dbdiagram** (`.dbml`): https://dbdiagram.io — paste, *Export ▸ PNG/PDF*.
- **draw.io**: https://app.diagrams.net — you can even embed the `.mmd`/`.puml`
  above via *Arrange ▸ Insert ▸ Advanced ▸ Mermaid / PlantUML*, keeping everything
  in one `.drawio` file.

## Correctness notes (avoid the common mistakes)
- **System context ≠ DFD context.** DFD uses a single circle (process 0); the doc's
  context diagram uses a **rectangle** for the system. Use the rectangle style.
- **Use case include vs extend directions:**
  - `<<include>>` arrow points **base → included** (base always performs the included UC).
  - `<<extend>>` arrow points **extending → base** (the extension runs conditionally).
  - **Generalization** uses a hollow-triangle solid arrow (e.g. Customer ▷ Guest).
- **ERD**: use **crow's foot**; mark PK and FK; show the three attribute-bearing
  n-n junctions explicitly (ShopService, ShopStaff, Favourite).
- **State chart**: include the initial `[*]` and final states; label transitions with
  the event, not just the target.

## System Context spec (draw.io, matching the reference look)
- Centre: rectangle **"PrintHub"**.
- Left/top actors (stick figures) → **Interact with ►** → PrintHub, with multiplicities:
  Guest, Customer, Shop Owner, Shop Staff, Admin.
- Right external systems (`<<External system>>` boxes) with labelled arrows from PrintHub:
  - **VietQR** — "Generates top-up QR via ►"
  - **Routing Service** — "Gets distance/ETA via ►"
  - **Gmail SMTP** — "Sends email via ►"
  - **Quote Engine (gRPC)** — "Estimates price via ►"
  - **RabbitMQ / Production Agent** — "Dispatches production via ►"
