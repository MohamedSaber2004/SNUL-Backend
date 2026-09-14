# Comprehensive Dual-Platform API & Multi-Environment Specification Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use subagent-driven-development (recommended) or executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Establish an exhaustive, fully verified documentation catalog, endpoint contracts, M2M authentication flows, and multi-environment infrastructure guide across both SNUL (`E:\SNUL Site\backend\SNUL`) and Welco (`E:\welco site\Welco`) platforms.

**Architecture:** Dual-platform topology consisting of 8 microservices per system (16 total), Ocelot API Gateways, strictly isolated regional client databases (SNUL Egypt vs. Welco Pakistan), and dynamic zero-duplication catalog integration secured via machine-to-machine Client-Credentials JWT tokens with thread-safe caching and 5-minute proactive refresh.

**Tech Stack:** .NET 10 (ASP.NET Core), EF Core (SQL Server), MediatR (CQRS), FluentValidation, Ocelot Gateway, Scalar OpenAPI, Polly Resilience.

---

## File Structure Map

| File Path | Responsibility |
| :--- | :--- |
| `E:\SNUL Site\backend\SNUL\README.md` | Master dual-platform architecture, endpoint dictionary, request/response contracts, and environment guide. |
| `E:\welco site\Welco\README.md` | Welco manufacturing master hub documentation and upstream integration endpoints reference. |
| `E:\SNUL Site\backend\SNUL\Commerce.Services.API\Controllers\IntegrationController.cs` | SNUL integration controller surfacing 25 external Welco endpoints. |
| `E:\welco site\Welco\UserManamgent.Service.API\Controllers\IntegrationDistributorsController.cs` | Welco distributor application receiver and approval controller. |
| `E:\SNUL Site\backend\SNUL\SNUL.Shared\Services\WelcoIntegrationService.cs` | Resilient HTTP integration client with JWT caching and multi-market routing. |

---

### Task 1: Verify Endpoints, DTO Schemas, and Role Authorizations

**Files:**
- Modify: `E:\SNUL Site\backend\SNUL\README.md`
- Verify: `E:\SNUL Site\backend\SNUL\SNUL.slnx`
- Verify: `E:\welco site\Welco\Welco.sln`

- [x] **Step 1: Inspect and verify all microservice routes and role decorators**
  - Verify Auth, UserManagement, Product, Commerce, Sales, Content, Certification, and Attachment routes across both SNUL and Welco.
  - Confirm role mappings: `Admin`, `SnulStaff`/`WelcoStaff`, `Provider`/`OrganizationUser`, `Client`/`Customer`.

- [x] **Step 2: Ensure strictly isolated order and client database boundaries**
  - Verify `IntegrationOrdersController.cs` is removed from Welco.
  - Verify `Commerce.Services.API/Controllers/IntegrationController.cs` in SNUL does not proxy local orders or client accounts.

- [x] **Step 3: Document request and response JSON forms with full DTO types**
  - Document `POST /api/v1/auth/register`, `POST /api/v1/auth/login`, `POST /api/integration/token`.
  - Document `POST /api/v1/distributor-applications`, `POST /api/v1/orders`, `POST /api/v1/rfqs`, `POST /api/v1/quotes`.
  - Document `POST /api/v1/integration/inventory/check` and `reserve`.

---

### Task 2: Validate Multi-Environment Configurations and Ocelot Route Coverage

**Files:**
- Verify: `E:\SNUL Site\backend\SNUL\SNUL.Gateway.API\ocelot.*.json`
- Verify: `E:\welco site\Welco\Welco.API\ocelot.*.json`
- Verify: `E:\SNUL Site\backend\SNUL\Commerce.Services.API\appsettings.*.json`

- [x] **Step 1: Check Ocelot routing configurations for 3 environments**
  - Validate `Development`, `Test`, and `Production` ocelot json files for all microservices.
  - Ensure `DangerousAcceptAnyServerCertificateValidator: true` is present for development certificates.
  - Ensure root routes `/api/v1/carts` and `/api/v1/orders` are correctly matched.

- [x] **Step 2: Check Machine-to-Machine Integration Settings**
  - Verify `WelcoIntegration` configuration schema in `appsettings.json`, `appsettings.Development.json`, `appsettings.Test.json`, and `appsettings.Production.json`.
  - Verify `BaseUrl`, `ClientId`, `ClientSecret`, `DefaultSystem`, and multi-market dictionary (`snul`, `welo`).

---

### Task 3: Solution Compilation and Test Suite Verification

**Files:**
- Test: `E:\SNUL Site\backend\SNUL\SNUL.Shared.Tests\SNUL.Shared.Tests.csproj`
- Test: `E:\welco site\Welco\Welco.Tests\Welco.Tests.csproj`

- [x] **Step 1: Compile SNUL Solution**
  Run: `dotnet build "E:\SNUL Site\backend\SNUL\SNUL.slnx"`
  Expected: Build succeeded with 0 Warnings, 0 Errors.

- [x] **Step 2: Compile Welco Solution**
  Run: `dotnet build "E:\welco site\Welco\Welco.sln"`
  Expected: Build succeeded with 0 Warnings, 0 Errors.

- [x] **Step 3: Execute SNUL Automated Test Suite**
  Run: `dotnet test "E:\SNUL Site\backend\SNUL\SNUL.Shared.Tests\SNUL.Shared.Tests.csproj"`
  Expected: 30 Passed, 0 Failed.

- [x] **Step 4: Execute Welco Automated Test Suite**
  Run: `dotnet test "E:\welco site\Welco\Welco.Tests\Welco.Tests.csproj"`
  Expected: 29 Passed, 0 Failed.
