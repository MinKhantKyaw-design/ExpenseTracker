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

