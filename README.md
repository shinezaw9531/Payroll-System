# Rezerv Studio Payroll System

An engineering assessment building an itemized processing engine for fitness studio instructor payroll. This solution separates domain computations from transport and database dependencies while driving a fast, dark-themed responsive React user experience.

---

## 🛠️ Tech Stack & Layers
- **Backend:** .NET 8 Web API, Entity Framework Core, SQL Server / LocalDB, Swagger OpenAPI.
- **Frontend:** React 18, Vite, TypeScript, Tailwind CSS.
- **Architecture Style:** Clean Architecture / Domain-Driven Design (DDD) Lite.

---

## 🚀 Setup & Execution Instructions

### 1. Database & Backend Engine Configuration
1. Open `src/PayrollBE.WebApi/appsettings.json` and point the `DefaultConnection` string to your target SQL Server instance.
2. Open your command line terminal in the `src/` root and execute the initialization layer:
   ```bash
   dotnet restore
   dotnet run --project PayrollBE.WebApi
