# Development Guidelines — Invoice Generation System

## Code Quality Standards

### Nullable Reference Types
All projects enable `<Nullable>enable</Nullable>`. Follow these conventions:
- Use `string? _error;` for optional state fields
- Use `null!` for navigation properties that EF/DI guarantees non-null: `public Invoice Invoice { get; set; } = null!;`
- Use `?.` and `??` operators consistently; avoid null-forgiving `!` except on navigation properties
- Guard against null args at service method entry: `if (customer is null) throw new ArgumentNullException(nameof(customer));`

### Naming Conventions
- Private fields: `_camelCase` prefix (e.g., `_gate`, `_nextId`, `_isEditMode`, `_editingId`)
- Services: `sealed class`, PascalCase, suffix `Service`
- Models: PascalCase, no suffix
- Razor page state: `_model`, `_error`, `_success`, `_isEditMode`, `_editingId` — consistent across all pages
- Constants: `private const int MinimumLineItemPanels = 2;`
- JS namespace: `window.invoiceAutocomplete = { ... }` — object literal namespace pattern

### C# Idioms Used Throughout
```csharp
// Pattern matching for null checks
if (existing is null) return false;
if (previousCustomer is not null && ...) { ... }

// Collection property null-coalescing init
customer.Invoices ??= new List<Invoice>();

// Index-from-end operator
if (IsLineItemFilled(_model.LineItems[^1])) { ... }

// Target-typed new
private readonly object _gate = new();
private readonly List<Customer> _customers = new();

// Inline object initializer with trailing comma
new Customer { Name = _model.Name, Email = _model.Email, }

// String.IsNullOrWhiteSpace for all text validation
if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("...");

// Case-insensitive string comparison
string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase)
```

## Service Layer Patterns

### Thread-Safe In-Memory Service
All services follow this exact structure:
```csharp
public sealed class XxxService
{
    private readonly object _gate = new();
    private readonly List<XxxModel> _items = new();
    private int _nextId = 1;

    public IReadOnlyList<XxxModel> GetAll()
    {
        lock (_gate) { return _items.OrderBy(...).ToList(); }
    }

    public XxxModel? GetById(int id)
    {
        lock (_gate) { return _items.FirstOrDefault(x => x.Id == id); }
    }

    public XxxModel Add(XxxModel item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        lock (_gate)
        {
            item.Id = _nextId++;
            _items.Add(item);
            return item;
        }
    }

    public bool Update(XxxModel item) { /* lock, find, mutate fields, return bool */ }
    public bool Delete(int id) { /* lock, find, remove, return bool */ }
}
```

- Always return `bool` from Update/Delete (true = success, false = not found)
- Always return the entity from Add
- `GetAll()` always returns a sorted snapshot (`ToList()` inside the lock)
- Validate business rules inside the lock (e.g., duplicate name check in `ItemCatalogService`)
- Trim string inputs before storing: `item.Name = item.Name?.Trim();`

### Service Dependencies
- Services are registered as `Singleton` in `Program.cs`
- Constructor injection is used when one service depends on another (`InvoiceService` takes `CustomerService`)
- Cross-entity integrity is maintained manually (e.g., removing invoice from `customer.Invoices` on delete)

## Blazor Page Patterns

### Standard Page Structure
Every CRUD page follows this template:
```razor
@page "/route"
@inject XxxService XxxService

<PageTitle>Title</PageTitle>
<h1>Title</h1>

@if (!string.IsNullOrWhiteSpace(_error)) {
    <div class="alert alert-danger" role="alert">@_error</div>
}

<div class="d-flex flex-column gap-4">
    <!-- Form card -->
    <div class="card shadow-sm form-card">
        <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
            <span>@(_isEditMode ? "Edit x" : "Add x")</span>
            @if (_isEditMode) { <button @onclick="CancelEdit">Cancel</button> }
        </div>
        <div class="card-body">
            <EditForm Model="_model" OnValidSubmit="SaveX" FormName="save-x">
                <DataAnnotationsValidator />
                <ValidationSummary />
                <!-- fields -->
                <button type="submit">@(_isEditMode ? "Update x" : "Save x")</button>
            </EditForm>
        </div>
    </div>

    <!-- List card -->
    <div class="card shadow-sm">
        <div class="card-header">X records</div>
        <div class="card-body">
            @if (_items.Count == 0) { <div class="text-muted">No x yet.</div> }
            else { /* table-responsive > table.table.table-striped.align-middle.mb-0 */ }
        </div>
    </div>
</div>

@code {
    private XxxModel _model = new();
    private List<XxxModel> _items = new();
    private string? _error;
    private bool _isEditMode;
    private int _editingId;

    protected override void OnInitialized() => Refresh();

    private void Refresh() => _items = XxxService.GetAll().ToList();

    private void SaveX() { _error = null; try { /* add or update */ ResetForm(); Refresh(); } catch (Exception ex) { _error = ex.Message; } }

    private void BeginEdit(int id) { /* populate _model from found item, set _isEditMode = true */ }
    private void CancelEdit() => ResetForm();
    private void DeleteX(int id) { _error = null; if (!XxxService.Delete(id)) { _error = "..."; return; } Refresh(); }
    private void ResetForm() { _isEditMode = false; _editingId = 0; _model = new XxxModel(); }
}
```

### Form Field Conventions
- Use `InputText` / `InputTextArea` / `InputSelect` / `InputDate` / `InputNumber` Blazor components
- Always pair with `<ValidationMessage For="() => _model.Field" />`
- Bootstrap grid: `<div class="row g-3">` with `<div class="col-12 col-md-6">`
- Action buttons: `btn-outline-primary` for Edit, `btn-outline-danger` for Delete, `btn-outline-secondary` for secondary actions

### Error Handling in Pages
- `_error` is always reset to `null` at the start of any mutating action
- Service exceptions are caught and assigned to `_error` for display
- Boolean return from service methods drives inline error messages (not exceptions)

## JavaScript Interop Pattern

### Autocomplete Init (invoice-autocomplete.js)
Called from `OnAfterRenderAsync` on every render:
```csharp
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    await JS.InvokeVoidAsync("invoiceAutocomplete.init", "inputElementId", "listElementId", namesArray);
}
```

The JS function:
- Cleans up previous listeners via `input._autocompleteCleanup()` before re-initialising
- Ranks suggestions: `startsWith` matches before `contains` matches, max 8 results
- Supports keyboard navigation (ArrowUp/Down, Enter, Escape)
- Dispatches a native `input` event with `{ bubbles: true }` when a suggestion is selected so Blazor's binding picks it up
- Registers a document-level `click` listener to close the list on outside click

## Model Conventions

### Data Annotations
```csharp
[Required, MaxLength(200)]          // string fields — always both
[Required, MaxLength(320), EmailAddress]  // email fields
[MaxLength(20)]                     // optional short strings
[Range(0.01, double.MaxValue)]      // positive decimal quantities
[Range(0, 100)]                     // percentage fields (TaxRate)
[NotMapped]                         // computed properties (Subtotal, TotalAmount)
```

### Computed Properties
Never store computed values in the database; use `[NotMapped]` expression-body properties:
```csharp
[NotMapped]
public decimal Subtotal => Quantity * UnitPrice * (1 + TaxRate / 100m);

[NotMapped]
public decimal TotalAmount => LineItems.Sum(li => li.Subtotal);
```

### Navigation Properties
- Required navigations initialised to `null!`: `public Customer Customer { get; set; } = null!;`
- Collection navigations initialised to empty list: `public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();`
- XML `<summary>` doc comments on all non-obvious properties

## Bootstrap UI Conventions
- Page wrapper: `<div class="d-flex flex-column gap-4">`
- Cards: `class="card shadow-sm"` with `card-header` / `card-body`
- Form card header: `class="card-header bg-primary text-white d-flex justify-content-between align-items-center"`
- Tables: `class="table table-striped align-middle mb-0"` inside `<div class="table-responsive">`
- Status badges: `<span class="badge text-bg-light border">@inv.Status</span>`
- Button sizes in tables: `btn btn-sm btn-outline-*`
- Spacing between action buttons: `me-2` on all but the last button
