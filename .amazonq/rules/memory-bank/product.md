# Product Overview — Invoice Generation System

## Purpose
A single-page application (SPA) for creating, managing, and printing invoices. Targets small businesses or freelancers who need a lightweight, self-hosted invoicing tool without a cloud dependency.

## Key Features
- **Customer management** — create, edit, delete customers (name, email, phone, address); customers with linked invoices cannot be deleted
- **Invoice management** — create invoices with a number, issued/due dates, status, notes, and dynamic line items; supports edit and delete
- **Line item tracking** — dynamic rows with description, quantity, unit price, and tax rate; subtotal auto-calculated as `Quantity × UnitPrice × (1 + TaxRate / 100)`
- **Invoice status workflow** — Draft → Sent → Paid / Overdue / Cancelled
- **Print view** — in-browser print sheet showing full invoice details, customer info, and itemised totals
- **Item catalog** — reusable catalog of named items (with details) that can be referenced when building invoice line items; supports autocomplete
- **Customer autocomplete** — JS-powered typeahead on the customer name field (startsWith ranked above contains, max 8 suggestions, keyboard navigable)

## Target Users
Freelancers, small business owners, or developers who want a self-hosted .NET invoicing tool they can run locally or deploy to a server.

## Use Cases
1. Add customers, then create invoices assigned to those customers
2. Build invoices from catalog items or free-text line items
3. Track invoice lifecycle from draft through to paid/cancelled
4. Print or export a formatted invoice view directly from the browser
