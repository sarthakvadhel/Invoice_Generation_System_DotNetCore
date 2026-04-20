# Project Structure — Invoice Generation System

## Solution Layout
```
Invoice_Generation_System_DotNetCore-main/
├── InvoiceSystem.slnx               # Solution file (links all three projects)
├── src/
│   ├── InvoiceSystem.API/           # ASP.NET Core Web API (backend stub)
│   │   ├── Program.cs               # Minimal API entry point; OpenAPI registered
│   │   ├── appsettings.json
│   │   └── Properties/launchSettings.json
│   ├── InvoiceSystem.Shared/        # Class library — shared models
│   │   └── Models/
│   │       ├── Customer.cs
│   │       ├── Invoice.cs           # Includes InvoiceStatus enum
│   │       └── InvoiceLineItem.cs
│   └── InvoiceSystem.Web/           # Blazor Server frontend
│       ├── Program.cs               # DI registration; Razor + Interactive Server
│       ├── Components/
│       │   ├── App.razor
│       │   ├── Routes.razor
│       │   ├── _Imports.razor
│       │   ├── Layout/
│       │   │   ├── MainLayout.razor
│       │   │   └── NavMenu.razor
│       │   └── Pages/
│       │       ├── Home.razor
│       │       ├── Customers.razor
│       │       ├── Invoices.razor
│       │       ├── Items.razor
│       │       └── Error.razor
│       ├── Services/
│       │   ├── CustomerService.cs
│       │   ├── InvoiceService.cs
│       │   └── ItemCatalogService.cs
│       └── wwwroot/
│           ├── app.css
│           ├── js/
│           │   └── invoice-autocomplete.js
│           └── lib/bootstrap/
```

## Core Components and Relationships

### Models (InvoiceSystem.Shared)
| Model | Key Fields | Notes |
|---|---|---|
| `Customer` | Id, Name, Email, Phone, Address, CreatedAt | Navigation: `ICollection<Invoice>` |
| `Invoice` | Id, InvoiceNumber, IssuedDate, DueDate, Status, Notes, CustomerId | `TotalAmount` is `[NotMapped]` computed |
| `InvoiceLineItem` | Id, Description, Quantity, UnitPrice, TaxRate, InvoiceId | `Subtotal` is `[NotMapped]` computed |
| `InvoiceStatus` | Draft, Sent, Paid, Overdue, Cancelled | Enum defined in Invoice.cs |
| `ItemCatalogEntry` | Id, Name, Details | Defined inside ItemCatalogService.cs |

### Services (InvoiceSystem.Web/Services) — all registered as `Singleton`
- `CustomerService` — in-memory CRUD; blocks delete if customer has invoices
- `InvoiceService` — in-memory CRUD; depends on `CustomerService` to resolve customer references; manages bi-directional customer↔invoice links
- `ItemCatalogService` — in-memory CRUD; seeded with 4 sample items on startup; enforces case-insensitive name uniqueness

### Pages (Blazor Server)
- `/` — Home (dashboard/landing)
- `/customers` — Customer CRUD page
- `/invoices` — Invoice CRUD page with dynamic line items and print view
- `/items` — Item catalog CRUD page

## Architectural Patterns
- **In-memory singleton services** — no database; all data lives in `List<T>` fields guarded by a `lock (_gate)` object for thread safety
- **Blazor Server interactive rendering** — `AddInteractiveServerComponents()` / `AddInteractiveServerRenderMode()`; all pages use server-side interactivity
- **Shared model library** — `InvoiceSystem.Shared` is referenced by both API and Web projects to avoid model duplication
- **JS interop for autocomplete** — `invoiceAutocomplete.init(inputId, listId, names[])` called via `JS.InvokeVoidAsync` in `OnAfterRenderAsync`; cleanup registered on the DOM element via `_autocompleteCleanup`
- **EditForm + DataAnnotationsValidator** — all forms use Blazor's `EditForm` with `DataAnnotationsValidator` and `ValidationMessage` components
- **Dual-mode form pattern** — every CRUD page uses `_isEditMode` + `_editingId` fields to toggle between Add and Edit states with a shared form and `ResetForm()` method
