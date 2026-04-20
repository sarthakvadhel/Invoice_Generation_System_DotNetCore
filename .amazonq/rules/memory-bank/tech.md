# Technology Stack — Invoice Generation System

## Runtime & Language
| Technology | Version | Role |
|---|---|---|
| .NET | 9.0 | Runtime platform for all three projects |
| C# | 13 (implicit with .NET 9) | Primary language |
| Blazor Server | 9.0 | Interactive server-side UI rendering |
| ASP.NET Core | 9.0 | Web API backend and Blazor host |

## NuGet Dependencies

### InvoiceSystem.API
| Package | Version | Purpose |
|---|---|---|
| `Microsoft.AspNetCore.OpenApi` | 9.0.14 | OpenAPI/Swagger endpoint (`/openapi`) |
| `Microsoft.EntityFrameworkCore.Sqlite` | 9.0.* | SQLite provider (wired up but not yet used in Program.cs) |
| `Microsoft.EntityFrameworkCore.Design` | 9.0.* | EF Core tooling (migrations) |
| `QuestPDF` | 2026.2.4 | PDF invoice generation |

### InvoiceSystem.Web
No additional NuGet packages beyond the .NET 9 SDK web defaults. References `InvoiceSystem.Shared` via project reference.

### InvoiceSystem.Shared
No NuGet packages. Pure class library targeting `net9.0`.

## Frontend
| Technology | Source | Purpose |
|---|---|---|
| Bootstrap | 5.x (lib/bootstrap in wwwroot) | UI layout and component styling |
| Vanilla JS | `wwwroot/js/invoice-autocomplete.js` | Customer name typeahead autocomplete |

## Project Settings (all three projects)
- `<Nullable>enable</Nullable>` — nullable reference types enforced
- `<ImplicitUsings>enable</ImplicitUsings>` — common namespaces auto-imported

## Development Commands
```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run API (port defined in Properties/launchSettings.json)
dotnet run --project src/InvoiceSystem.API

# Run Web frontend
dotnet run --project src/InvoiceSystem.Web

# Trust dev HTTPS certificate (run once)
dotnet dev-certs https --trust

# EF Core migrations (API project)
dotnet ef migrations add <MigrationName> --project src/InvoiceSystem.API
dotnet ef database update --project src/InvoiceSystem.API
```

## Notes
- Both API and Web must run simultaneously; Web calls API for data
- Data is currently in-memory only (no EF Core DbContext wired in Web); EF Core + SQLite packages are present in the API project for future persistence
- Solution file uses the newer `.slnx` format (Visual Studio 2022 17.x+)
