# Invoice Generation System — .NET Core

An open-source Invoice Generation and Management SPA. Built with an **ASP.NET Core Web API** backend, a responsive **Blazor** frontend styled with Bootstrap, and **EF Core (SQLite)** for data persistence. Features dynamic line-item tracking and automated PDF rendering via **QuestPDF**.

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Prerequisites](#prerequisites)
3. [Step-by-Step: Running the Project in Visual Studio 2022/2026 Insider](#step-by-step-running-the-project-in-visual-studio-20222026-insider)
   - [Step 1 — Install the Required Tools](#step-1--install-the-required-tools)
   - [Step 2 — Clone the Repository](#step-2--clone-the-repository)
   - [Step 3 — Open the Solution](#step-3--open-the-solution)
   - [Step 4 — Restore NuGet Packages](#step-4--restore-nuget-packages)
   - [Step 5 — Understand the Solution Structure](#step-5--understand-the-solution-structure)
   - [Step 6 — Configure Multiple Startup Projects](#step-6--configure-multiple-startup-projects)
   - [Step 7 — Run the Application](#step-7--run-the-application)
   - [Step 8 — Open the App in Chrome](#step-8--open-the-app-in-chrome)
4. [Project Structure Explained](#project-structure-explained)
5. [Technology Stack](#technology-stack)
6. [Troubleshooting](#troubleshooting)

---

## Project Overview

The Invoice Generation System allows you to:

- **Create and manage customers** — store name, email, phone, and address.
- **Create invoices** — assign them to customers with line items (description, quantity, unit price, tax rate).
- **Track invoice status** — Draft → Sent → Paid / Overdue / Cancelled.
- **Generate PDF invoices** — powered by QuestPDF.

The application is split into three projects inside one solution:

| Project | Type | Purpose |
|---|---|---|
| `InvoiceSystem.API` | ASP.NET Core Web API | Backend REST API (data & business logic) |
| `InvoiceSystem.Web` | Blazor Server | Frontend web UI (pages & navigation) |
| `InvoiceSystem.Shared` | Class Library | Shared models used by both API and Web |

---

## Prerequisites

Before you start, make sure you have the following installed on your computer:

| Tool | Why You Need It | Download Link |
|---|---|---|
| **Visual Studio 2022 / 2026 Insider** | IDE to open, build, and run the project | [visualstudio.microsoft.com](https://visualstudio.microsoft.com/) |
| **.NET 9 SDK** | Required runtime and compiler for the project | [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) |
| **Google Chrome** | Recommended browser for the best experience | [google.com/chrome](https://www.google.com/chrome/) |
| **Git** *(optional but recommended)* | To clone the repository from GitHub | [git-scm.com](https://git-scm.com/) |

> **What is Visual Studio Insider?**  
> Visual Studio Insider (also called "Preview") is a pre-release version of Visual Studio that includes the very latest features before they ship to the stable release. It works exactly like the regular version — you open solutions, build, and run projects the same way.

---

## Step-by-Step: Running the Project in Visual Studio 2022/2026 Insider

### Step 1 — Install the Required Tools

1. **Install Visual Studio 2022/2026 Insider**
   - Go to [https://visualstudio.microsoft.com/vs/preview/](https://visualstudio.microsoft.com/vs/preview/) and download the Insider/Preview edition.
   - Run the installer. When asked about **Workloads**, tick:
     - ✅ **ASP.NET and web development**
     - ✅ **.NET desktop development** *(optional but useful)*
   - Click **Install**. This also installs the .NET SDK automatically.

2. **Verify .NET 9 is installed**
   - Open a terminal (press `Win + R`, type `cmd`, press Enter).
   - Type `dotnet --version` and press Enter.
   - You should see a version number starting with `9.` (e.g., `9.0.x`). If not, download .NET 9 SDK from [https://dotnet.microsoft.com/download/dotnet/9.0](https://dotnet.microsoft.com/download/dotnet/9.0).

3. **Install Google Chrome** (if not already installed)
   - Go to [https://www.google.com/chrome/](https://www.google.com/chrome/) and download it.

---

### Step 2 — Clone the Repository

**Option A — Using Git (recommended)**

1. Open a terminal or **Git Bash**.
2. Navigate to the folder where you want to save the project. For example:
   ```
   cd C:\Projects
   ```
3. Clone the repository:
   ```
   git clone https://github.com/sarthakvadhel/Invoice_Generation_System_DotNetCore.git
   ```
4. A new folder called `Invoice_Generation_System_DotNetCore` will be created. Remember where it is.

**Option B — Downloading as a ZIP**

1. Go to the repository page on GitHub: [https://github.com/sarthakvadhel/Invoice_Generation_System_DotNetCore](https://github.com/sarthakvadhel/Invoice_Generation_System_DotNetCore)
2. Click the green **Code** button → **Download ZIP**.
3. Extract the ZIP to a folder on your computer (e.g., `C:\Projects\Invoice_Generation_System_DotNetCore`).

---

### Step 3 — Open the Solution

1. Launch **Visual Studio 2022/2026 Insider**.
2. On the start screen, click **Open a project or solution**.
3. Navigate to the folder where you cloned/extracted the project.
4. Open the file `InvoiceSystem.slnx` (this is the solution file that links all three projects together).
5. Visual Studio will load the solution. You will see three projects in the **Solution Explorer** panel on the right:
   - `InvoiceSystem.API`
   - `InvoiceSystem.Web`
   - `InvoiceSystem.Shared`

> **Tip:** If you don't see the Solution Explorer, go to **View → Solution Explorer** in the menu bar.

---

### Step 4 — Restore NuGet Packages

NuGet packages are external libraries the project depends on (like QuestPDF for PDF generation and Entity Framework Core for database access). They are not stored in the repository and must be downloaded first.

1. In the menu bar click **Build → Restore NuGet Packages**.
2. Wait for Visual Studio to finish downloading. You'll see progress in the **Output** window at the bottom.
3. When it says *"Restore complete"*, you're ready to proceed.

> **What is NuGet?** NuGet is .NET's package manager — similar to npm for JavaScript or pip for Python. It downloads code libraries from the internet so your project can use them.

---

### Step 5 — Understand the Solution Structure

Inside the `src` folder you will find three projects:

```
src/
├── InvoiceSystem.API/         ← Web API backend (handles data & logic)
│   └── Program.cs             ← Entry point for the API server
├── InvoiceSystem.Web/         ← Blazor frontend (what you see in the browser)
│   └── Components/
│       ├── Pages/             ← Web pages (Home, Invoices, Customers, etc.)
│       └── Layout/            ← Shared layout and navigation menu
└── InvoiceSystem.Shared/      ← Shared models (Customer, Invoice, InvoiceLineItem)
    └── Models/
```

- The **API project** runs on a port like `https://localhost:7xxx` and serves data.
- The **Web project** runs on a different port and shows the UI in your browser.
- Both need to run at the same time for the app to work correctly.

---

### Step 6 — Configure Multiple Startup Projects

Because the solution has two runnable projects (API and Web), you need to tell Visual Studio to start **both** at the same time.

1. In the **Solution Explorer**, right-click on the top-level solution item (the root node named `InvoiceSystem`).
2. Select **Set Startup Projects…**
3. In the dialog that opens, choose **Multiple startup projects**.
4. Set the **Action** for both projects:
   - `InvoiceSystem.API` → **Start**
   - `InvoiceSystem.Web` → **Start**
   - `InvoiceSystem.Shared` → **None** *(this is a library, not a runnable app)*
5. Click **OK**.

> **Why do we need two projects running?** The Web (Blazor) frontend is the user interface. The API is the backend that handles all the data. The Web project talks to the API to get and save data, so both must be running at the same time.

---

### Step 7 — Run the Application

1. Make sure Chrome is your default browser, **or** set it manually:
   - Next to the green **▶ Run** button at the top of Visual Studio, click the small dropdown arrow.
   - Select **Google Chrome** from the browser list.

2. Press **F5** (or click the green **▶ Start** button).

3. Visual Studio will:
   - Build all three projects.
   - Launch the **API** server (a console window will appear — do not close it).
   - Launch the **Web** server and automatically open Chrome.

4. Chrome will open and you will see the **Invoice Generation System** home page with navigation links to **Invoices** and **Customers**.

> **Note:** The first build may take a minute or two as Visual Studio compiles the code and restores packages.

---

### Step 8 — Open the App in Chrome

If Chrome does not open automatically:

1. Look at the **Output** window in Visual Studio (bottom panel).
2. Find the line that says something like:
   ```
   Now listening on: https://localhost:5001
   ```
3. Open **Google Chrome** and type that URL into the address bar (e.g., `https://localhost:5001`).
4. Press **Enter** — you will see the Invoice Generation System.

**If you see a security warning ("Your connection is not private"):**

This happens because the app uses a local HTTPS certificate that is self-signed. This is normal for local development.

- Click **Advanced** → **Proceed to localhost (unsafe)**.

To avoid this warning in the future, trust the development certificate:
1. Close Visual Studio and the running app.
2. Open a terminal and run:
   ```
   dotnet dev-certs https --trust
   ```
3. A dialog will appear asking you to trust the certificate — click **Yes**.
4. Restart the app.

---

## Project Structure Explained

### Models (InvoiceSystem.Shared)

| Model | Description |
|---|---|
| `Customer` | Stores customer info: name, email, phone, address |
| `Invoice` | Stores invoice: number, issued date, due date, status, notes, line items |
| `InvoiceLineItem` | A single row on an invoice: description, quantity, unit price, tax rate |
| `InvoiceStatus` | Enum: `Draft`, `Sent`, `Paid`, `Overdue`, `Cancelled` |

### Invoice Total Calculation

Each `InvoiceLineItem` calculates its subtotal automatically:
```
Subtotal = Quantity × UnitPrice × (1 + TaxRate / 100)
```

The `Invoice.TotalAmount` is the sum of all line item subtotals.

---

## Technology Stack

| Technology | Version | Purpose |
|---|---|---|
| .NET | 9.0 | Runtime platform |
| ASP.NET Core | 9.0 | Web API backend |
| Blazor Server | 9.0 | Interactive web UI |
| Entity Framework Core | 9.0 | Database ORM |
| SQLite | — | Local file-based database |
| QuestPDF | 2026.x | PDF invoice generation |
| Bootstrap | 5.x | UI styling |

---

## Troubleshooting

**"The project doesn't build / I see red errors"**
- Make sure .NET 9 SDK is installed (`dotnet --version` in terminal).
- Try **Build → Clean Solution**, then **Build → Rebuild Solution**.
- Try **Build → Restore NuGet Packages** again.

**"Port already in use" error**
- Another program is using the same port. Restart your computer and try again, or change the port in `Properties/launchSettings.json` inside the API or Web project.

**"Chrome shows a blank page or 'This site can't be reached'"**
- Make sure both the API and Web projects are set as startup projects (Step 6).
- Check that you pressed **F5** (not Ctrl+F5 which runs without debugging).
- Look at the Output window for error messages.

**"I accidentally closed the API console window"**
- The API is no longer running. Stop the session (Shift+F5), and press F5 again to restart both projects.

**"The development certificate is not trusted"**
- Run `dotnet dev-certs https --trust` in a terminal and accept the prompt (see Step 8).

