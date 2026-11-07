# ExpenseTracker API and Database Documentation

This document describes the currently implemented HTTP endpoints and database model for ExpenseTracker.

## Overview

- **Application type:** ASP.NET Core MVC with Razor views
- **Target framework:** .NET 8
- **Data access:** Entity Framework Core 8 with SQL Server
- **Authentication:** Cookie authentication
- **Default route:** `/{controller=Home}/{action=Index}/{id?}`
- **Anti-forgery:** Required on state-changing form posts

The application currently exposes MVC actions that return HTML views and redirects. It does not currently expose a dedicated JSON REST API or Swagger/OpenAPI endpoint.

## Authentication and authorization

### Cookie authentication

The application uses the ASP.NET Core cookie authentication scheme:

- Login page: `GET /Account/Login`
- Login redirect for unauthenticated users: `/Account/Login`
- Access-denied page: `/Account/AccessDenied`
- Cookie lifetime: 8 hours
- Sliding expiration: enabled
- Cookie: HTTP-only and configured to require HTTPS

After a successful login, the cookie contains these claims:

| Claim | Value |
|---|---|
| `NameIdentifier` | User ID |
| `Name` | User name |
| `Email` | User email |

`HomeController`, `CategoryController`, and `ExpensesController` require an authenticated user. Expense queries use the authenticated user's ID rather than accepting a user ID from the browser.

## Route summary

| Method | Route | Authentication | Description |
|---|---|---|---|
| GET | `/Account/Register` | Anonymous | Display registration form |
| POST | `/Account/Register` | Anonymous | Register a user |
| GET | `/Account/Login` | Anonymous | Display login form |
| POST | `/Account/Login` | Anonymous | Authenticate a user |
| POST | `/Account/Logout` | Authenticated | Sign out the current user |
| GET | `/Account/AccessDenied` | Anonymous | Display access-denied page |
| GET | `/Home/Index` | Authenticated | Display dashboard totals and category summary |
| GET | `/Category` | Authenticated | List active categories |
| GET | `/Category/Create` | Authenticated | Display category creation form |
| POST | `/Category/Create` | Authenticated | Create a category |
| GET | `/Category/Edit/{id}` | Authenticated | Display category edit form |
| POST | `/Category/Edit/{id}` | Authenticated | Update a category |
| GET | `/Category/Delete/{id}` | Authenticated | Display category deletion confirmation |
| POST | `/Category/Delete/{id}` | Authenticated | Soft-delete a category |
| GET | `/Expenses` | Authenticated | List the current user's expenses |
| GET | `/Expenses/Create` | Authenticated | Display expense creation form |
| POST | `/Expenses/Create` | Authenticated | Create an expense |
| GET | `/Expenses/Edit/{id}` | Authenticated | Display expense edit form |
| POST | `/Expenses/Edit/{expenseID}` | Authenticated | Update an expense |
| GET | `/Expenses/Delete/{id}` | Authenticated | Display expense deletion confirmation |
| POST | `/Expenses/Delete/{id}` | Authenticated | Soft-delete an expense |

All POST actions that modify data require a valid antiforgery token and use form binding rather than JSON request bodies.

## Account endpoints

### Register

`POST /Account/Register`

Form fields:

| Field | Required behavior |
|---|---|
| `Name` | Trimmed before storage |
| `Email` | Trimmed; must not already belong to an active user |
| `Password` | Hashed using `PasswordHasher<User>`; the original password is never stored |

A successful registration redirects to `/Account/Login`. Duplicate active email addresses return the registration view with a validation error.

### Login

`POST /Account/Login`

Form fields:

| Field | Description |
|---|---|
| `Email` | Trimmed and matched against an active user |
| `Password` | Verified against the stored password hash |
| `returnUrl` | Optional local return URL |

Invalid credentials return the login view with a generic `Invalid email or password.` message. External return URLs are not accepted.

### Logout

`POST /Account/Logout` signs out the cookie authentication session and redirects to `/Account/Login`.

## Category endpoints

### Category request

```text
Name         string, required, maximum 100 characters in the database
Description  string, optional, maximum 500 characters in the database
```

Category names are trimmed and compared case-insensitively against other active categories. Category names are also unique at the database level.

### Category behavior

- List, get, update, and delete operations ignore categories where `DelFlg = true`.
- Deletion is a soft delete; the row remains in the database and `DelFlg` is set to `true`.
- The category named `Miscellaneous` cannot be deleted.
- A category with active expenses cannot be deleted.
- Missing category IDs return `404 Not Found` for edit and delete flows.

## Expense endpoints

### Expense request

```text
CategoryID  optional integer; omitted or invalid zero value falls back to Miscellaneous
Amount      decimal; must be greater than zero
Date        DateOnly; required and cannot be in the future
Notes       optional string; trimmed before storage, maximum 500 characters in the database
```

The authenticated user's ID is assigned by the controller during creation. A selected category must be active. If no category is selected, the active category named `Miscellaneous` is used; creation fails if that category does not exist.

### Expense list filters

`GET /Expenses` accepts the following query-string values:

| Parameter | Type | Behavior |
|---|---|---|
| `FromDate` | `DateOnly?` | Include expenses on or after this date |
| `ToDate` | `DateOnly?` | Include expenses on or before this date |
| `CategoryID` | `int?` | Restrict results to one category |
| `SortBy` | enum | Date by default; amount when set to the amount option |
| `SortDirection` | enum | Descending by default; supports ascending |

Only non-deleted expenses belonging to the authenticated user are returned. The list returns category name, amount, date, and notes in the rendered view model.

### Expense behavior

- Get, update, and delete operations ignore soft-deleted expenses.
- Deletion is a soft delete; the row remains in the database and `DelFlg` is set to `true`.
- A missing or already deleted expense returns `404 Not Found` from edit and delete flows.
- Dashboard totals are calculated for the authenticated user only.
- The dashboard calculates today, Monday-based current-week, and current-month totals.
- Category summaries group all expenses for the user by category; the current dashboard query does not explicitly filter out `DelFlg = true` expenses.

## Database configuration

The application registers `ExpenseTrackerDBContext` with SQL Server using the `DefaultConnection` connection string:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=.;Database=ExpenseTrackerDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

For local development, update the connection string in user secrets, environment-specific configuration, or another configuration provider instead of committing credentials to source control.

## Database schema

### `Users`

| Column | Type / behavior | Notes |
|---|---|---|
| `UserID` | `int`, primary key | User identifier |
| `Name` | String, maximum 100 | Display name |
| `Email` | String, maximum 255 | Login email; application checks active-user duplicates |
| `Password` | String, maximum 255, default empty string | Password hash, never the clear-text password |
| `DelFlg` | `bit` | Soft-delete flag |

### `Categories`

| Column | Type / behavior | Notes |
|---|---|---|
| `CategoryID` | `int`, primary key | Category identifier |
| `Name` | String, maximum 100 | Unique database index `UQ_Categories_Name` |
| `Description` | Nullable string, maximum 500 | Optional description |
| `DelFlg` | `bit` | Soft-delete flag |

### `Expenses`

| Column | Type / behavior | Notes |
|---|---|---|
| `ExpenseID` | `int`, primary key | Expense identifier |
| `UserID` | `int`, foreign key | References `Users.UserID` |
| `CategoryID` | `int`, foreign key | References `Categories.CategoryID` |
| `Amount` | `decimal(18,2)` | Expense amount |
| `Date` | `DateOnly` | Expense date |
| `Notes` | Nullable string, maximum 500 | Optional notes |
| `DelFlg` | `bit` | Soft-delete flag |

## Relationships and indexes

### Relationships

- One `User` has many `Expenses`.
- One `Category` has many `Expenses`.
- `Expenses.UserID` has foreign key constraint `FK_Expenses_Users`.
- `Expenses.CategoryID` has foreign key constraint `FK_Expenses_Categories`.
- Both relationships use `ClientSetNull` delete behavior in the EF Core model. Application-level category deletion is blocked when active expenses reference the category.

### Indexes

| Index | Table / column | Purpose |
|---|---|---|
| `UQ_Categories_Name` | `Categories.Name` | Enforce unique category names |
| `IX_Expenses_CategoryID` | `Expenses.CategoryID` | Category lookups and filtering |
| `IX_Expenses_Date` | `Expenses.Date` | Date filtering and dashboard queries |
| `IX_Expenses_UserID` | `Expenses.UserID` | User-scoped expense queries |

## Soft-delete rules

The application uses `DelFlg` instead of physically removing users, categories, or expenses:

- Active records have `DelFlg = false`.
- Deleted records have `DelFlg = true`.
- Category and expense service list/get/update/delete queries exclude deleted records.
- There is no global EF Core query filter, so every query that should exclude deleted rows must include the condition explicitly.

## Current implementation notes

- Swagger services and middleware are not registered in `Program.cs`, even though the Swashbuckle package is referenced. No Swagger document is currently served.
- No EF Core migrations are present in the projects. Database creation or schema updates must therefore be handled using the existing database/scaffolding workflow or added migrations.
- The connection string uses Windows integrated authentication (`Trusted_Connection=True`) and `TrustServerCertificate=True`, which is suitable for local development but should be reviewed for production.
