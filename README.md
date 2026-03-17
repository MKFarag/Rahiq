# Rahiq — E-Commerce REST API

A production-ready, full-featured e-commerce backend built with **ASP.NET Core (.NET 10)** following **Clean Architecture** principles. It covers the complete shopping lifecycle — from product browsing and cart management to order processing, payment verification, and automated email notifications.

---

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
  - [Layer Overview](#layer-overview)
  - [Dependency Graph](#dependency-graph)
  - [Key Design Patterns](#key-design-patterns)
- [Domain Model](#domain-model)
- [Application Layer — CQRS Use Cases](#application-layer--cqrs-use-cases)
- [API Reference](#api-reference)
- [Authentication & Authorization](#authentication--authorization)
- [Background Jobs](#background-jobs)
- [Caching Strategy](#caching-strategy)
- [File Storage](#file-storage)
- [Email & Notifications](#email--notifications)
- [Health Checks](#health-checks)
- [Rate Limiting](#rate-limiting)
- [Logging](#logging)
- [Configuration & Settings](#configuration--settings)
- [Tech Stack & Dependencies](#tech-stack--dependencies)
- [Project Structure](#project-structure)
- [Database & Migrations](#database--migrations)
- [Getting Started](#getting-started)

---

## Features

| Area | Capabilities |
|---|---|
| **Catalog** | Products (with discounts & images), Bundles (time-limited, multi-product), Categories, Types |
| **Cart** | Add/Update/Clear cart (supports both standalone products and bundles) |
| **Orders** | Full lifecycle: Pending → Processing → Shipped → Delivered (with cancellation) |
| **Payments** | Upload payment proof image; admin verification flow |
| **Shipping** | Customer-managed shipping addresses; admin assignment of tracking details |
| **Users & Roles** | ASP.NET Identity with fine-grained permission-based authorization |
| **Auth** | JWT + Refresh Token rotation; email confirmation; password reset |
| **Notifications** | HTML email templates for registration, confirmation, password reset, and daily admin reports |
| **Background Jobs** | Hangfire-powered fire-and-forget + scheduled jobs with a secured dashboard |
| **Caching** | .NET HybridCache (in-process L1 + distributed-ready L2) with tag-based invalidation |
| **Observability** | Serilog structured logging, health endpoints (DB, SMTP, Hangfire) |
| **Security** | Rate limiting, upload signature validation, permission claims, HTTPS-only |
| **Documentation** | NSwag / Swagger UI + ReDoc |

---

## Architecture

This project strictly implements **Clean Architecture** (also known as Onion or Ports & Adapters Architecture). Each layer has a single responsibility and depends only on layers beneath it.

### Layer Overview

```
┌──────────────────────────────────────────────────────────┐
│                     PRESENTATION                         │
│   ASP.NET Core Web API — Controllers, Middleware, DI     │
├──────────────────────────────────────────────────────────┤
│                    INFRASTRUCTURE                        │
│   EF Core, JWT, Email, Cache, Hangfire, Repositories     │
├──────────────────────────────────────────────────────────┤
│                     APPLICATION                          │
│   CQRS Handlers, Interfaces (Ports), Validators, Mapster │
├──────────────────────────────────────────────────────────┤
│                       DOMAIN                             │
│   Entities, Repository Interfaces, Result Pattern, Errors│
└──────────────────────────────────────────────────────────┘
```

#### Domain
The innermost, pure C# layer. Has **zero NuGet dependencies**.
- Defines all **domain entities** with business behaviour (computed properties, state-transition methods).
- Defines **repository interfaces** (ports) that Infrastructure must implement.
- Contains the **`Result<T>` / `Error`** value types used as a functional alternative to exceptions for business failures.
- Holds domain **error catalogs**, **permission constants**, **default role/user seed constants**, **cache key/tag constants**, and all **settings POCOs**.

#### Application
Depends only on Domain. Contains all **use cases** (73 total).
- Implements a **hand-rolled CQRS dispatcher** — no MediatR dependency.
- Each use case is a self-contained `IRequest` + `IRequestHandler` pair under the `Feathers/` directory.
- Defines **application service interfaces** (`IUnitOfWork`, `IJwtProvider`, `IEmailSender`, `INotificationService`, `ICacheService`, `IFileStorageService`, `IJobManager`, `ISignInService`, `ITemplateRenderer`, `IUrlEncoder`).
- Hosts all **FluentValidation** validators and **Mapster** mapping configurations.

#### Infrastructure
Depends on Application. Implements every interface defined there.
- **EF Core** `ApplicationDbContext` (inherits `IdentityDbContext`) with full entity configurations and 13 migration history.
- **Repository** and **Unit of Work** implementations.
- **Permission Authorization** infrastructure (dynamic policy provider + authorization handler).
- All technical services: JWT, MailKit SMTP, HybridCache, Hangfire, file storage, background jobs.

#### Presentation
The entry point. Depends on Application + Infrastructure.
- Thin **controllers** — each action simply dispatches to `ISender` and maps the `Result` to an HTTP response.
- All DI wiring, middleware pipeline, options validation, CORS, rate limiting, NSwag, and Hangfire dashboard configuration.

### Dependency Graph

```
Presentation ──► Application ──► Domain
     │                ▲
     └──► Infrastructure ──► Application
```

### Key Design Patterns

| Pattern | Details |
|---|---|
| **Clean Architecture** | Strict inward-only dependency rule; Domain has no framework references |
| **CQRS (Custom)** | Hand-rolled `IRequest` / `IRequestHandler` / `ISender`; `Sender` resolves handlers from DI at runtime via reflection |
| **Result Pattern** | `Result<T>` and `Error` replace exceptions for business failures; errors carry HTTP status codes so controllers can trivially map them via `ToProblem()` |
| **Repository + Unit of Work** | `IGenericRepository<T>` with full CRUD, projection, and bulk ops; `IUnitOfWork` aggregates all repositories under one `CompleteAsync()` |
| **Domain Model with Behaviour** | `Order`, `Bundle`, and `Product` entities have computed properties and state-transition methods (e.g. `Order.Pay()`, `Order.Ship()`, `Order.Cancel()`) |
| **Permission-Based Authorization** | `PermissionAuthorizationPolicyProvider` creates `AuthorizationPolicy` objects at runtime from JWT permission claims; `[HasPermission("permission")]` attribute on controller actions |
| **Hybrid Caching** | `HybridCache` with tag-based invalidation; standardised expiry presets (Short / Medium / Long / VeryLong); cache tags defined as constants on entities |
| **Options Pattern (Fail-Fast)** | All settings classes validated with `ValidateDataAnnotations().ValidateOnStart()` — misconfiguration crashes at startup rather than at runtime |
| **Secure File Upload** | Binary signature inspection rejects disguised executables; extension whitelist; 2 MB size limit |
| **Split Query Optimisation** | `ApplyIncludesSafely` automatically switches to `.AsSplitQuery()` for deep (2+ levels) or numerous (3+) includes to avoid Cartesian explosion |

---

## Domain Model

```
User ──────────────────────── has many ──► RefreshToken
  │
  ├── Customer ─── has many ──► Order
  │                               │
  │                               ├──► OrderItem ──► Product
  │                               │            └──► Bundle
  │                               ├──► Payment
  │                               └──► Shipping
  │
  └── has one ──► Cart (multiple CartItems)
                      ├──► Product
                      └──► Bundle

Product ──── belongs to ──► Category
        ──── belongs to ──► Type
Bundle  ──── has many (M:M via BundleItem) ──► Product
```

### Entity Summary

| Entity | Key Business Logic |
|---|---|
| `Order` | State machine: `Pending → Processing → Shipped → Delivered`; `CanBeCancelled` guard; `GrandTotal`, `Remaining` computed from items + shipping + payment |
| `OrderItem` | Discriminates between product and bundle items; calculates `Total` |
| `Product` | `SellingPrice = Price × (1 – DiscountPercentage / 100)` via domain extension method |
| `Bundle` | `IsActive`, `RemainingDays`, `OldPrice`, `SellingPrice`; time-bounded via `EndAt` |
| `Cart` | Supports product or bundle entries; `UnitPrice` and `TotalPrice` computed |
| `Payment` | Upload proof image; admin sets `IsProofed` |
| `Shipping` | Customer-managed address/phone; admin assigns carrier code and cost |
| `RefreshToken` | `IsExpired`, `IsActive` computed properties; revocable |

---

## Application Layer — CQRS Use Cases

All use cases live under `Application/Feathers/` and follow the `IRequest<Result<T>>` → `IRequestHandler` convention.

### Auth (8 use cases)
`GetTokenCommand` · `GetRefreshTokenCommand` · `RevokeRefreshTokenCommand` · `RegisterCommand` · `ConfirmEmailCommand` · `ResendConfirmationEmailCommand` · `SendResetPasswordCodeCommand` · `ResetPasswordCommand`

### Bundles (8 use cases)
`AddBundleCommand` · `UpdateBundleCommand` · `DeactivateBundleCommand` · `ReactivateBundleCommand` · `AddBundleImageCommand` · `DeleteBundleImageCommand` · `GetAllBundlesQuery` · `GetBundleQuery`

### Carts (4 use cases)
`AddToCartCommand` · `UpdateCartCommand` · `ClearMyCartCommand` · `GetMyCartQuery`

### Categories (5 use cases)
`AddCategoryCommand` · `UpdateCategoryCommand` · `DeleteCategoryCommand` · `GetAllCategoriesQuery` · `GetCategoryQuery`

### Orders (11 use cases)
`AddOrderCommand` · `CancelOrderCommand` · `StartProcessingOrderCommand` · `ShipOrderCommand` · `DeliverOrderCommand` · `GetMyOrderQuery` · `GetAllMyOrdersQuery` · `GetOrderQuery` · `GetAllOrdersByStatusQuery` · `GetAllOrdersByYearQuery` · `GetAllOrdersByMonthQuery`

### Payments (3 use cases)
`AddOrderPaymentCommand` · `VerifyPaymentCommand` · `GetAllNotVerifiedPaymentsQuery`

### Products (8 use cases)
`AddProductCommand` · `UpdateProductCommand` · `ChangeProductStatusCommand` · `AddProductDiscountCommand` · `AddProductImageCommand` · `DeleteProductImageCommand` · `GetAllProductsQuery` · `GetProductQuery`

### Roles (5 use cases)
`AddRoleCommand` · `UpdateRoleCommand` · `ToggleRoleStatusCommand` · `GetAllRolesQuery` · `GetRoleQuery`

### Shippings (6 use cases)
`AddCustomerShippingCommand` · `AssignShippingDetailsCommand` · `UpdateShippingCommand` · `DeleteShippingCommand` · `GetAllShippingsQuery` · `GetShippingQuery`

### Types (5 use cases)
`AddTypeCommand` · `UpdateTypeCommand` · `DeleteTypeCommand` · `GetAllTypesQuery` · `GetTypeQuery`

### Users (10 use cases)
`AddUserCommand` · `UpdateUserCommand` · `UpdateUserProfileCommand` · `ToggleStatusUserCommand` · `ChangeUserPasswordCommand` · `ChangeUserEmailRequestCommand` · `ConfirmChangeUserEmailCommand` · `GetAllUsersQuery` · `GetUserQuery` · `GetUserProfileQuery`

---

## API Reference

All endpoints are documented via Swagger UI (available at `/swagger` in development) and ReDoc (available at `/api-docs`).

### Auth — `/Auth`

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/Auth` | Anonymous | Login — returns JWT + refresh token |
| `POST` | `/Auth/refresh` | Anonymous | Rotate refresh token |
| `POST` | `/Auth/revoke-refresh` | Anonymous | Revoke a refresh token |
| `POST` | `/Auth/register` | Anonymous | Register new customer account |
| `POST` | `/Auth/confirm` | Anonymous | Confirm email address |
| `POST` | `/Auth/resend-confirm` | Anonymous | Re-send confirmation email |
| `POST` | `/Auth/forget-password` | Anonymous | Send password-reset link |
| `POST` | `/Auth/reset-password` | Anonymous | Reset password with token |

### Products — `api/products`

| Method | Path | Permission | Description |
|---|---|---|---|
| `GET` | `/api/products` | Anonymous | Paginated product list with search & sort |
| `GET` | `/api/products/{id}` | Anonymous | Get product details |
| `POST` | `/api/products` | `products:add` | Create product |
| `PUT` | `/api/products/{id}` | `products:update` | Update product |
| `PUT` | `/api/products/{id}/change-status` | `products:update` | Toggle availability |
| `PUT` | `/api/products/{id}/discount` | `products:update` | Set discount percentage |
| `PUT` | `/api/products/{id}/image` | `products:update` | Upload product image |
| `DELETE` | `/api/products/{id}/image` | `products:update` | Remove product image |

### Bundles — `api/bundles`

| Method | Path | Permission | Description |
|---|---|---|---|
| `GET` | `/api/bundles` | Anonymous | Paginated bundle list |
| `GET` | `/api/bundles/{id}` | Anonymous | Get bundle details |
| `POST` | `/api/bundles` | `bundles:add` | Create bundle |
| `PUT` | `/api/bundles/{id}` | `bundles:update` | Update bundle |
| `PUT` | `/api/bundles/{id}/image` | `bundles:update` | Upload bundle image |
| `DELETE` | `/api/bundles/{id}/image` | `bundles:update` | Remove bundle image |
| `PUT` | `/api/bundles/{id}/deactivate` | `bundles:update` | Deactivate bundle |
| `PUT` | `/api/bundles/{id}/reactivate` | `bundles:update` | Reactivate bundle |

### Orders — `api/orders`

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/orders` | Customer | Place a new order |
| `GET` | `/api/orders/me` | Customer | Get all my orders |
| `GET` | `/api/orders/me/{id}` | Customer | Get one of my orders |
| `PUT` | `/api/orders/{id}/cancel` | Customer | Cancel an order |
| `GET` | `/api/orders/{id}` | `orders:read` | Admin get order by ID |
| `GET` | `/api/orders/status` | `orders:read` | Filter orders by status |
| `GET` | `/api/orders/year` | `orders:read` | Orders grouped by year |
| `GET` | `/api/orders/month` | `orders:read` | Orders for a specific month |
| `PUT` | `/api/orders/{id}/start-processing` | `orders:update` | Move to Processing |
| `PUT` | `/api/orders/{orderId}/ship/{shippingId}` | `orders:update` | Mark as Shipped |
| `PUT` | `/api/orders/{id}/deliver` | `orders:update` | Mark as Delivered |

### Payments — `api/payments`

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/payments` | Customer | Upload payment proof |
| `GET` | `/api/payments` | `payments:read` | Get all unverified payments |
| `PUT` | `/api/payments/{id}/verify` | `payments:update` | Verify a payment |

### Carts — `api/carts`

| Method | Path | Auth | Description |
|---|---|---|---|
| `GET` | `/api/carts` | Customer | Get my cart |
| `POST` | `/api/carts` | Customer | Add item to cart |
| `PUT` | `/api/carts/{id}` | Customer | Update cart item quantity |
| `DELETE` | `/api/carts` | Customer | Clear my cart |

### Shippings — `api/shippings`

| Method | Path | Auth | Description |
|---|---|---|---|
| `GET` | `/api/shippings` | `shippings:read` | Get all shippings |
| `GET` | `/api/shippings/{id}` | `shippings:read` | Get shipping by ID |
| `POST` | `/api/shippings` | Customer | Add shipping address |
| `PUT` | `/api/shippings/{id}` | Customer | Update own shipping |
| `DELETE` | `/api/shippings/{id}` | Customer | Delete own shipping |
| `PUT` | `/api/shippings/{id}/assign` | `shippings:update` | Admin assigns tracking details |

### Roles — `api/roles`

| Method | Path | Permission |
|---|---|---|
| `GET` | `/api/roles` | `roles:read` |
| `GET` | `/api/roles/{id}` | `roles:read` |
| `POST` | `/api/roles` | `roles:add` |
| `PUT` | `/api/roles/{id}` | `roles:update` |
| `PUT` | `/api/roles/{id}/toggle-status` | `roles:update` |

### Users — `api/users`

| Method | Path | Permission |
|---|---|---|
| `GET` | `/api/users` | `users:read` |
| `GET` | `/api/users/{id}` | `users:read` |
| `POST` | `/api/users` | `users:add` |
| `PUT` | `/api/users/{id}` | `users:update` |
| `PUT` | `/api/users/{id}/toggle-status` | `users:update` |
| `PUT` | `/api/users/{id}/change-email` | `users:update` |
| `PUT` | `/api/users/{id}/change-password` | `users:update` |

### Profile — `api/profile`

| Method | Path | Auth | Description |
|---|---|---|---|
| `GET` | `/api/profile` | Authenticated | Get own profile |
| `PUT` | `/api/profile` | Authenticated | Update own profile |
| `PUT` | `/api/profile/change-password` | Authenticated | Change own password |
| `PUT` | `/api/profile/change-email` | Authenticated | Request email change |
| `PUT` | `/api/profile/confirm-change-email` | Authenticated | Confirm email change |

### Categories & Types

Full CRUD under `api/categories` and `api/types` with matching `categories:*` / `types:*` permissions.

---

## Authentication & Authorization

### JWT Authentication
- **Algorithm**: HMAC-SHA256
- **Claims**: `sub` (user ID), `email`, `givenName`, `familyName`, `jti`, `roles` (JSON array), `permissions` (JSON array)
- **Expiry**: Configurable via `Jwt:ExpiryMinutes` (default 60 minutes)

### Refresh Tokens
- Stored in the `RefreshTokens` table as an owned collection on `ApplicationUser`
- 14-day expiry
- Cryptographically random (64 bytes via `RandomNumberGenerator`)
- Rotated on each use; old tokens are revocable

### Permission-Based Authorization
The system uses **34 granular permissions** covering every domain area:

```
bundles:add      bundles:read      bundles:update
categories:add   categories:read   categories:update   categories:delete
orders:read      orders:update
payments:read    payments:update
products:add     products:read     products:update
roles:add        roles:read        roles:update
shippings:read   shippings:update
types:add        types:read        types:update        types:delete
users:add        users:read        users:update
```

**How it works:**
1. On login, the JWT is minted with the user's permissions as a `permissions` JSON array claim.
2. `[HasPermission("products:add")]` on a controller action sets a dynamic policy name.
3. `PermissionAuthorizationPolicyProvider` creates an `AuthorizationPolicy` with a `PermissionRequirement` on-the-fly.
4. `PermissionAuthorizationHandler` validates the requirement against the JWT claims.

The default **Admin** role is seeded with all 34 permissions. The **Customer** role has no admin permissions.

---

## Background Jobs

Hangfire is used for all asynchronous operations, backed by SQL Server storage.

| Job | Type | Trigger |
|---|---|---|
| Send registration confirmation email | Fire-and-forget | On user registration |
| Send email confirmation link | Fire-and-forget | On registration & resend request |
| Send password-reset link email | Fire-and-forget | On forget-password request |
| Send email change confirmation | Fire-and-forget | On email change request |
| Daily cancelled orders report | Recurring (daily) | Cron — sends admin summary |
| Daily pending orders report | Recurring (daily) | Cron — sends admin summary |
| Daily unverified payments report | Recurring (daily) | Cron — sends admin summary |
| Daily low-stock bundles report | Recurring (daily) | Cron — sends admin summary |

**Dashboard**: `/hangfire` — protected with HTTP Basic Authentication (credentials from `appsettings.json`).

---

## Caching Strategy

Uses .NET 9+ **`HybridCache`** — an in-process L1 cache with a pluggable distributed L2 backend (ready for Redis).

### Cache Keys & Tags

Every cacheable entity exposes static `Cache.Keys` and `Cache.Tags` constants for consistent keying and grouped invalidation.

### Expiry Presets

| Preset | Duration |
|---|---|
| Short | 5 minutes |
| Medium | 30 minutes |
| Long | 2 hours |
| VeryLong | 24 hours |

### Invalidation

Tag-based invalidation ensures that when a single entity is mutated, all related cached queries are automatically evicted (e.g. updating a product invalidates both the product detail cache and the product list cache).

---

## File Storage

Images are stored under `wwwroot/images/` and served as static assets.

### Upload Security

Every uploaded file passes four validators before being written to disk:

| Validator | Rule |
|---|---|
| `FileSizeValidator` | Maximum 2 MB |
| `FileNameValidator` | No path traversal characters |
| `ImageExtensionValidator` | Allowed: `.jpg`, `.jpeg`, `.png` only |
| `BlockedSignaturesValidator` | Binary magic-byte check — rejects `.exe`, JavaScript, and Office macro files masquerading as images |

---

## Email & Notifications

Email is sent via **MailKit** over SMTP (StartTLS).

HTML templates are loaded from disk (`Templates/*.html`), and `{{placeholder}}` tokens are replaced at runtime by `TemplateRenderer`.

### Template Placeholders

| Template | Key Placeholders |
|---|---|
| Registration confirmation | `{{name}}`, `{{confirmation_link}}`, `{{support_email}}` |
| Password reset | `{{name}}`, `{{reset_link}}` |
| Email change | `{{name}}`, `{{confirmation_link}}` |
| Daily admin reports | `{{report_date}}`, `{{items}}` (tabular data) |

---

## Health Checks

Available at `GET /health` — returns JSON with individual component statuses.

| Component | Check |
|---|---|
| SQL Server | Connection to `DefaultConnection` |
| Hangfire | Hangfire server heartbeat |
| SMTP | TCP connectivity to the configured mail host |

---

## Rate Limiting

| Policy | Applied To | Limit |
|---|---|---|
| `IpLimit` (Fixed Window) | `AuthController` | 30 requests per 60 seconds per IP |
| `Concurrency` | All other controllers | 200 concurrent requests, queue depth 50 |

Both policies and their parameters are configurable via `RateLimitingOptions` in `appsettings.json`.

---

## Logging

**Serilog** is configured for structured logging:
- JSON formatter (machine-readable log files)
- Rolling daily log files
- `SerilogRequestLogging` middleware logs every HTTP request with method, path, status code, and elapsed time
- Unhandled exceptions are caught by `GlobalExceptionHandler`, logged, and returned as RFC 7807 `ProblemDetails` with status 500

---

## Configuration & Settings

All settings are in `appsettings.json`. Sensitive values (JWT signing key, DB passwords, SMTP password) should be stored in **User Secrets** in development and **environment variables / Azure Key Vault** in production.

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "<SQL Server connection string>",
    "HangfireConnection": "<Hangfire SQL Server connection string>"
  },
  "Jwt": {
    "Key": "<min 32-char signing key>",
    "Issuer": "<issuer>",
    "Audience": "<audience>",
    "ExpiryMinutes": 60
  },
  "MailSettings": {
    "Host": "smtp.example.com",
    "Port": 587,
    "Username": "noreply@example.com",
    "Password": "<smtp password>",
    "DisplayName": "Rahiq"
  },
  "EmailTemplateOptions": {
    "AdminEmail": "admin@example.com",
    "SupportEmail": "support@example.com",
    "AppName": "Rahiq",
    "LogoUrl": "<logo URL>"
  },
  "AppUrlSettings": {
    "FrontendBaseUrl": "https://yourfrontend.com",
    "ApiBaseUrl": "https://yourapi.com"
  },
  "Hangfire": {
    "Username": "admin",
    "Password": "<dashboard password>"
  },
  "AllowedOrigins": [ "https://yourfrontend.com" ],
  "RateLimitingOptions": {
    "IpLimit": { "PermitLimit": 30, "WindowSeconds": 60 },
    "Concurrency": { "PermitLimit": 200, "QueueLimit": 50 }
  }
}
```

---

## Tech Stack & Dependencies

### Frameworks & Runtime
- **.NET 10** (net10.0)
- **ASP.NET Core 10** Web API

### Infrastructure
| Package | Purpose |
|---|---|
| `Microsoft.EntityFrameworkCore` 10.0.3 | ORM |
| `Microsoft.EntityFrameworkCore.SqlServer` 10.0.3 | SQL Server provider |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 10.0.3 | Identity + EF Core |
| `Microsoft.AspNetCore.Authentication.JwtBearer` 10.0.3 | JWT middleware |
| `Microsoft.Extensions.Caching.Hybrid` 10.3.0 | HybridCache (L1 + L2) |
| `MailKit` 4.14.1 | SMTP email sending |
| `Hangfire.Core` + `Hangfire.SqlServer` 1.8.23 | Background jobs |
| `System.Linq.Dynamic.Core` 1.7.1 | Runtime dynamic LINQ (sort/search) |

### Application
| Package | Purpose |
|---|---|
| `FluentValidation` 12.1.1 | Input validation |
| `Mapster` 7.4.0 | Object mapping |

### API & Observability
| Package | Purpose |
|---|---|
| `NSwag.AspNetCore` 14.6.3 | Swagger UI + ReDoc |
| `Serilog.AspNetCore` 10.0.0 | Structured logging |
| `SharpGrip.FluentValidation.AutoValidation.Mvc` 2.0.0 | Automatic model validation |
| `Hangfire.Dashboard.Basic.Authentication` 8.0.0 | Dashboard security |
| `AspNetCore.HealthChecks.SqlServer` 9.0.0 | SQL Server health check |
| `AspNetCore.HealthChecks.Hangfire` 9.0.0 | Hangfire health check |
| `AspNetCore.HealthChecks.UI.Client` 9.0.0 | Health check JSON writer |

---

## Project Structure

```
Rahiq/
│
├── Domain/                              ← Core domain (no external dependencies)
│   ├── Entities/                        ← Order, Product, Bundle, Cart, Payment, Shipping, ...
│   ├── Repositories/                    ← IGenericRepository, IUserRepository, IRoleRepository, IUnitOfWork
│   ├── Results/                         ← Result<T>, Error (Result Pattern)
│   ├── Errors/                          ← BundleErrors, UserErrors, ProductErrors, OrderErrors, ...
│   ├── Constants/                       ← Permissions, DefaultRoles, DefaultUsers, CacheTags, OrderStatus
│   └── Settings/                        ← JwtOptions, MailSettings, FileSettings, AppUrlSettings, ...
│
├── Application/                         ← Use-case layer
│   ├── Abstraction/Messaging/           ← IRequest, IRequestHandler, ISender (custom CQRS)
│   ├── Interfaces/                      ← IUnitOfWork, IJwtProvider, IEmailSender, ICacheService, ...
│   ├── Feathers/                        ← All 73 use cases, organised by domain area
│   │   ├── Auth/
│   │   ├── Bundles/
│   │   ├── Carts/
│   │   ├── Categories/
│   │   ├── Orders/
│   │   ├── Payments/
│   │   ├── Products/
│   │   ├── Roles/
│   │   ├── Shippings/
│   │   ├── Types/
│   │   └── Users/
│   └── DependencyInjection.cs           ← Mapster + FluentValidation DI registration
│
├── Infrastructure/                      ← Technical implementations
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Configurations/              ← IEntityTypeConfiguration per entity
│   │   ├── Migrations/                  ← 13 EF Core migrations
│   │   └── Extensions/                  ← QueryableExtensions (search, sort, split query)
│   ├── Repositories/                    ← GenericRepository, UserRepository, RoleRepository, UnitOfWork
│   ├── Services/                        ← JwtProvider, EmailSender, NotificationService,
│   │                                      CacheService, FileStorageService, JobManager,
│   │                                      TemplateRenderer, UrlEncoder, SignInService
│   ├── Authorization/                   ← HasPermissionAttribute, PermissionAuthorizationHandler,
│   │                                      PermissionAuthorizationPolicyProvider
│   ├── Health/                          ← MailProviderHealthCheck
│   └── DependencyInjection.cs           ← Infrastructure DI registration
│
├── Presentation/                        ← Web API entry point
│   ├── Controllers/                     ← 12 controllers (Auth, Products, Bundles, Orders, ...)
│   ├── DTOs/                            ← File validators (size, extension, signature)
│   ├── Middleware/                      ← GlobalExceptionHandler
│   ├── Extensions.cs                    ← Result.ToProblem(), ClaimsPrincipal.GetId()
│   ├── DependencyInjection.cs           ← Full DI + pipeline configuration
│   └── Program.cs                       ← App startup + middleware pipeline
│
```

---

## Database & Migrations

- **ORM**: Entity Framework Core 10 with SQL Server
- **DbContext**: `ApplicationDbContext` (inherits `IdentityDbContext<ApplicationUser, ApplicationRole, string>`)
- **Global FK behaviour**: All cascade deletes overridden to `Restrict` to prevent accidental data loss

### Seeded Data (applied via migrations)

| Data | Details |
|---|---|
| **Admin Role** | `IsDefault = false`, `IsDisabled = false` |
| **Customer Role** | `IsDefault = true`, `IsDisabled = false` |
| **Admin User** | Seeded credentials; assigned to Admin role |
| **Admin Permissions** | All 34 permission claims assigned to the Admin role |

### Running Migrations

```bash
# From the solution root
dotnet ef database update --project Infrastructure --startup-project Presentation
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (local or remote)
- SMTP server credentials (Gmail, SendGrid, etc.)

### 1. Clone the repository

```bash
git clone https://github.com/MKFarag/Rahiq.git
cd Rahiq
```

### 2. Configure User Secrets (Development)

```bash
cd Presentation
dotnet user-secrets set "Jwt:Key" "<your-32-char-min-key>"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"
dotnet user-secrets set "ConnectionStrings:HangfireConnection" "<your-hangfire-connection-string>"
dotnet user-secrets set "MailSettings:Password" "<your-smtp-password>"
dotnet user-secrets set "Hangfire:Password" "<your-dashboard-password>"
```

### 3. Apply Migrations

```bash
dotnet ef database update --project Infrastructure --startup-project Presentation
```

### 4. Run the API

```bash
dotnet run --project Presentation
```

The API will be available at `https://localhost:<port>`.

### 5. Explore the API

| URL | Description |
|---|---|
| `https://localhost:<port>/swagger` | Swagger UI |
| `https://localhost:<port>/api-docs` | ReDoc |
| `https://localhost:<port>/health` | Health check status |
| `https://localhost:<port>/hangfire` | Hangfire dashboard |

---

## License

This project is licensed under the MIT License.
