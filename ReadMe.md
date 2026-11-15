\# Expense Tracker



A lightweight daily expense tracking web application built with \*\*ASP.NET Core MVC (.NET 8)\*\* and \*\*Microsoft SQL Server 2022\*\*.



The application allows users to:



\- View daily expenses

\- Create, edit, and soft-delete expenses

\- Create, edit, and soft-delete categories

\- Filter expenses by date and category

\- Sort expenses by amount and date

\- View dashboard summaries

\- View spending totals by category

\- Use SignalR for dashboard updates

\- Display category spending using ApexCharts



\---



\## 1. Technologies



| Technology | Version |

|---|---|

| .NET | 8 |

| ASP.NET Core MVC | .NET 8 |

| Entity Framework Core | 8 |

| Microsoft SQL Server | 2022 |

| C# | .NET 8 |

| Razor Views | ASP.NET Core MVC |

| Bootstrap | 5 |

| SignalR | ASP.NET Core |

| ApexCharts | JavaScript |

| Database | MSSQL |



\---



\## 2. Project Structure



```text

ExpenseTracker

│

├── Controllers

│   ├── HomeController.cs

│   ├── ExpensesController.cs

│   └── CategoryController.cs

│

├── Database

│   ├── Context

│   │   └── ExpenseTrackerDBContext.cs

│   │

│   └── Models

│       ├── User.cs

│       ├── Category.cs

│       └── Expense.cs

│

├── DTOs

│   ├── CategoryDto.cs

│   ├── CategoryRequest.cs

│   ├── ExpenseDto.cs

│   ├── ExpenseRequest.cs

│   └── CategorySummaryDto.cs

│

├── Hubs

│   └── DashboardHub.cs

│

├── Interfaces

│   ├── ICategoryService.cs

│   ├── IExpenseService.cs

│   └── IDashboardService.cs

│

├── Services

│   ├── CategoryService.cs

│   ├── ExpenseService.cs

│   └── DashboardService.cs

│

├── ViewModels

│   └── DashboardViewModel.cs

│

├── Views

│   ├── Home

│   │   └── Index.cshtml

│   │

│   ├── Expenses

│   │   ├── Index.cshtml

│   │   ├── Create.cshtml

│   │   ├── Edit.cshtml

│   │   └── Delete.cshtml

│   │

│   ├── Category

│   │   ├── Index.cshtml

│   │   ├── Create.cshtml

│   │   ├── Edit.cshtml

│   │   └── Delete.cshtml

│   │

│   └── Shared

│       └── \_Layout.cshtml

│

├── wwwroot

│   ├── css

│   └── js

│

├── appsettings.json

├── Program.cs




dotnet ef dbcontext scaffold "Server=.;Database=ExpenseTrackerDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --project ExpenseTracker.Database --context-dir Context --context ExpenseTrackerDBContext -o Models --force



\###Technology stack can therefore be presented as:



ASP.NET Core MVC

.NET 8

C#

Entity Framework Core 8

Microsoft SQL Server 2022

Razor Views

Bootstrap

SignalR

ApexCharts



## CQRS implementation

This branch uses **manual CQRS** (without MediatR). MVC controllers call dedicated handlers in `ExpenseTracker/Features`.

- `Features/Expenses/Commands`: create, update, and soft-delete expense operations.
- `Features/Expenses/Queries`: expense list and single-expense reads.
- `Features/Categories/Commands`: create, update, and soft-delete category operations.
- `Features/Categories/Queries`: category list and single-category reads.
- `Features/Dashboard/Queries`: dashboard totals and category summary.

Commands change application state; queries only read data. Handlers use the existing `ExpenseTrackerDBContext`, so no database relationship or schema changes are required for CQRS. Existing Razor views, cookie authentication, DTOs, and validators are retained.

Expense queries and update/delete commands always include the authenticated `userId` in their database predicates. The user ID comes from `BaseController.GetCurrentUserId()` and is not accepted from the browser. Category data remains shared across users, matching the existing database design.

### Adding a feature

1. Add a command handler under the relevant feature's `Commands` folder for a state-changing operation.
2. Add a query handler under `Queries` for a read-only operation.
3. Register the handler in `Program.cs` using scoped lifetime.
4. Inject the handler into the MVC controller and call `HandleAsync`.

No MediatR package is required.
