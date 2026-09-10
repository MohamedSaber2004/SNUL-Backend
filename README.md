# SNUL Backend — Duplicate of Welco Microservices Platform

> Duplication target: `E:\SNUL Site\backend\SNUL`
> Source truth: `E:\Welco Site\Welco` (analyzed from **source files**, not from Welco README — 758x `.cs` files verified).
> Strategy locked: **Rename to `SNUL.*` / New separate DB / New port block / Rebrand to SNUL.**

---

## 1. Source Inventory (verified in code)

### 1.1 Solution layout

```
Welco/
  Welco.Shared/                  classlib net10.0 — shared kernel (MUST be ported first)
  Auth.Services.API/             :7203 / http 5066
  UserManamgent.Service.API/     :7204 / http 5067  (note upstream typo "Manamgent")
  Product.Services.API/          :7054 / http 5241
  Commerce.Services.API/         :7045 / http 5076
  Sales.Services.API/            :7046 / http 5056
  Content.Services.API/          :7047 / http 5109
  Certification.Services.API/    :7101 / http 5200
  Attachment.Services.API/       :7180 / http 5121
  Welco.API/                     :7166 / http 5293  (Ocelot Gateway)
  Welco.Tests/
```

### 1.2 Shared kernel (`Welco.Shared` → `SNUL.Shared`)

- `DependencyInjection.cs`: `AddWelcoSharedDependencies(config)` (HttpContextAccessor, CurrentUserService, EmailService, GenericRepository/UnitOfWork, Jwt/Email/ExchangeRate options, `WelcoDbContext` SQL Server + retry, MemoryCache, `IExchangeRateService`, `FrankfurterExchangeRateProvider` via Fawazahmed CDN + `ExchangeRateApiProvider` alt) + `AddWelcoIdentity(config)` (Identity `ApplicationUser:IdentityUser<Guid>` + `IdentityRole<Guid>`, `CustomIdentityOptions`).
- `Persistance/WelcoDbContext.cs`: `IdentityDbContext<ApplicationUser,...>` with 42x `DbSet` (Users, RefreshTokens, Countries, Cities, Zones, UserAddresses, CompanyAddresses, Certifications, Categories, Products, Currencies, ExchangeRates, ExchangeRateSyncLogs, Companies, ProductSpecifications, ProductMedias, ProductProcedureTags, UserProductInteractions, Carts, CartItems, Orders, OrderItems, Invoices, RFQs, RFQItems, Quotes, QuoteItems, ProductInquiries, DistributorApplications, Documents, LandingPages, HelpCategories, HelpArticles, FAQItems, SupportTickets, TradeShowEvents, BlogPosts, Notifications, AuditLogs, SupportContacts, OemInquiries). Soft-delete global filter (`IsDeleted`), `ApplyAuditInformation` + `CaptureAuditEntries` on every SaveChanges.
- `Persistance/Configurations/`: 23x `IEntityTypeConfiguration` + `Persistance/Seeding/`: `RoleSeeder, UserSeeder (+User.json +UserSeedModel), CurrencySeeder, LandingPageSeeder, WorldLocationSeeder, BogusDemoSeeder`.
- `Common/`: `Attributes/RoleAuthorizeAttribute` (Admin bypass, `userType` claim, localized 401/403), `Behaviors/ValidationBehavior`, `Classes/BaseEntity`, `DTOs/` (Auth, Products, Commerce, Sales, Content, UserMgmt, Cert, Attach), `Exceptions/` (BadRequest/Conflict/Forbidden/NotFound/Unauthorized/Validation), `Extensions/` (Guid, JwtAuth, Pagination), `Interfaces/` (CurrentUser, Email, ExchangeRate x2, JwtToken, File validators, IWelcoDbContext), `Middlewares/CustomExceptionHandlerMiddleware`, `Options/` (CustomIdentity, Email, ExchangeRate, Jwt, UploadPaths), `Repositories/` (GenericRepository/UnitOfWork), `Services/` (CurrentUserService, CustomFileProvider, EmailService/MailKit, UploadPaths).
- `Controllers/AppControllerBase.cs`: `ToActionResult(Result<T>/PaginatedResult<T>)`, `Success/CreatedResult/Failure/NotFound/Unauthorized/Forbidden/Conflict/ServerError`, `Accept-Language/Language/X-Language/Culture` → `AppLanguage` normalization, localization via `ILocalizationProvider`.
- `Results/Result.cs + PaginatedResult.cs`: `{isSuccess, statusCode, message, errors[], data}` envelope.
- `Localization/`: `JsonLocalizationProvider + JsonStringLocalizer*`, `LocalizationKeys`, `Messages`, `Resources/messages.en.json + messages.ar.json`.
- `Infrastructure/ExchangeRate/`: `ExchangeRateService, ExchangeRateSyncBackgroundService, FrankfurterExchangeRateProvider`.
- `OpenApi/`: `AcceptLanguageHeaderTransformer, OpenApiExtensions`.
- `Enums/`: `UserType (Admin, WelcoStaff→SnulStaff, OrganizationUser)`, `AppLanguage`, `CompanyType/CompanyStatus`, `MediaType`.
- NuGet: `Bogus, MailKit/MimeKit, JwtBearer 10.0.11, Identity.EF, OpenApi, EFCore.SqlServer/Tools/Design, FluentValidation, MediatR 12.4.1`.

### 1.3 Service API matrix (routes = `*ApiRoutes.cs` constants, all MediatR CQRS `Features/<Aggregate>/Commands|Queries/<Op>/<Op>Command|Query.cs + Handler + Validator`)

**Auth (`api/v1/auth`)** — `AuthController`: `POST register, login, verify-register-otp, forgot-password, verify-password-otp, reset-password, refresh-token, logout | GET+PUT profile (RoleAuthorize)`. Features: `Register, Login, VerifyEmailOtp, ForgotPassword, VerifyPasswordOtp, ResetPassword, RefreshToken, Logout, UpdateProfile, GetUserProfile` + `Infrastructure/IJwtTokenService, JwtTokenService`.

**UserManagement** — controllers: `Users (GET all/byId, POST, PUT, DELETE, PUT change-password — Admin)`, `Companies (GET my-company, GET all anon, GET byId, POST/PUT/DELETE Admin)`, `CompanyAddresses` (dual bases: nested under company + direct `company-addresses`), `Addresses (GetAllByUser, GetById, CRUD)`, `Countries/Cities/Zones (GET anon, POST/PUT/DELETE Admin; Cities also GetByCountry, Zones GetByCity)`, `DistributorApplications (POST anon create, GET all/byId staff, PUT approve/reject Admin)`, `AuditLogs (GET all/byId staff)`.

**Product (`api/v1/...`)** — `Products (GET all/byId/show/videos anon; POST/PUT/DELETE + PUT videos staff)`, `Categories (GET all/byId/show/products-by-category anon; CUD staff)`, `Currencies (GET all/byId/byCode anon; CUD Admin)`, `ExchangeRates (GET latest/latest-by-base/pair/convert/history/sync-logs anon; POST sync/sync-history Admin)`, `Wishlist (GET all/check, POST add, DELETE remove — auth)`. Extras: `Common/ProductDtoMapper`, `Jobs/ExchangeRateSyncJob`, `Filters/HangfireAuthorizationFilter`.

**Commerce** — `CartsController`: `GET (list staff, byId, byUser, bySession)`, `POST create/add-item/clear`, `PUT update-item`, `DELETE remove-item` (OrganizationUser/Admin). `OrdersController`: `GET all/byId/track`, `POST create (OrgUser/Admin/Staff)`, `PUT update-status (Admin/Staff)`.

**Sales (`api/v1/rfqs|quotes|product-inquiries`)** — `RFQsController (GET all/byId, POST create OrgUser+, PUT {id}/status Staff+)`, `QuotesController (GET all/byId, POST create Staff+, POST {id}/approve + {id}/decline OrgUser+)`, `ProductInquiriesController (GET all/byId Staff+, POST anon guest inquiry, DELETE {id} Admin)`.

**Content** — `Documents (GET all/byId anon; POST/DELETE staff)`, `FAQs (GET anon; CUD staff)`, `HelpArticles (GET all/byId/bySlug anon; CUD staff)`, `HelpCategories (same)`, `LandingPages (GET all/bySlug anon; CUD staff)`, `OemInquiries (GET all/byId staff; POST anon; DELETE Admin)`, `OemServices (GET all anon)`, `SupportContact (GET anon; PUT Admin)`, `SupportTickets (GET all staff; GET my; GET byId; POST create OrgUser+; POST reply staff; POST close)`, `TradeShows (GET anon; POST staff)` @ `api/v1/trade-shows`.

**Certification (`api/v1/certifications`)** — `GET (all/byId/{id}/show anon)`, `POST / PUT {id} / DELETE {id} (Admin/Staff)`.

**Attachment (`api/v1/attachments`)** — `POST upload (multipart/form-data), POST upload-multiple, PUT {name}, GET download? (download AllowAnonymous)`. `Infrastructure: BaseFileService, FilePathHelper, Image/Video/Audio/FileValidator, DependencyInjection`.

**Gateway (`Welco.API`)** — `Program.cs` merges `Ocelot/ocelot.*.{env}.json` (8 service files + `ocelot.global.json`) into `ocelot.merged.{env}.json` at startup; JWT Bearer validation (shared secret fallback `V5B?*77+gzD_pk+2!%ORg<i)<D$DH+Xf.nECc?];2l;`), `GatewayCorsPolicy`, `FixedWindow RateLimiter (PermitLimit 100 / 60s)`, `InsecureClient` (3s, accept-any-cert for downstream HTTPS), `OpenApiAggregatorService + OpenApiCacheWarmer`, Scalar docs (`/ → /scalar/v1`, `/openapi/all.json`, `/api/docs/{service}`), Serilog console+file, forwarded headers, security headers (nosniff/DENY/XSS/referrer/HSTS), `UseOcelot()`. Ocelot per-service files distinguish anon `GET` vs `Bearer`-protected `POST/PUT/DELETE`.

---

## 2. Target Design (SNUL)

```
backend/SNUL/
  SNUL.slnx
  SNUL.Shared/                      ← Phase 1 (this task)
  Auth.Services.API/                ← Phase 2
  UserManagement.Service.API/       ← Phase 3 (fix "Manamgent" typo)
  Product.Services.API/             ← Phase 4
  Commerce.Services.API/            ← Phase 5
  Sales.Services.API/               ← Phase 6
  Content.Services.API/             ← Phase 7
  Certification.Services.API/       ← Phase 8
  Attachment.Services.API/          ← Phase 9
  SNUL.Gateway.API/                 ← Phase 10 (upgrade existing stub)
  README.md                         ← this file
```

### Port block (offset +100 so both stacks run side-by-side)

| Service | Welco HTTPS | **SNUL HTTPS** | **SNUL HTTP** |
|---|---|---|---|
| Gateway | 7166 | **7266** | 5393 |
| Auth | 7203 | **7303** | 5166 |
| UserMgmt | 7204 | **7304** | 5167 |
| Product | 7054 | **7154** | 5341 |
| Commerce | 7045 | **7145** | 5176 |
| Sales | 7046 | **7146** | 5156 |
| Content | 7047 | **7147** | 5209 |
| Cert | 7101 | **7201** | 5300 |
| Attach | 7180 | **7280** | 5221 |

### Rename map (apply to every cloned file)

| Welco | SNUL |
|---|---|
| `Welco.` namespace | `SNUL.` |
| `WelcoDbContext / IWelcoDbContext` | `SnulDbContext / ISnulDbContext` |
| `AddWelcoSharedDependencies / AddWelcoIdentity` | `AddSnulSharedDependencies / AddSnulIdentity` |
| `UserType.WelcoStaff` | `UserType.SnulStaff` |
| `Welco Team` (EmailSettings Name) | `SNUL Team` |
| `Welco.API / Welco.Gateway.API` | `SNUL.Gateway.API` |
| `UserManamgent.Service.API` | `UserManagement.Service.API` |
| JWT `Issuer/Audience/ValidIssuers/ValidAudiences` + Ocelot `Host/Port/BaseUrl` | SNUL ports/hosts above + `snul-*.runasp.net` |
| Swagger title `Welco Microservices Platform API` | `SNUL Microservices Platform API` |

---

## 3. Implementation Phases

### Phase 0 — Prep (done: .NET 10.0.202 verified)
- [x] Analyze source files
- [ ] Create SQL Server DB `SNUL_DB` + user; keep connection string in User Secrets (never commit — Welco leaked a real connection string + Gmail app-password in `appsettings*.json`; do NOT copy those values)
- [ ] Reserve JWT secret (≥32 chars), SMTP sender, `PORT` env for hoster

### Phase 1 — `SNUL.Shared` (Done ✅)
1. Copy `Welco.Shared` → `SNUL.Shared` excluding `bin/obj/Migrations/*.cs`, `*.Backup.tmp`, `.vs`.
2. Apply rename map + update `SNUL.Shared.csproj` (keep NuGet versions).
3. Update `messages.en/ar.json` branding, `User.json` admin seed, `UploadPathsOptions`.
4. `dotnet build SNUL.Shared` must pass before Phase 2.

### Phases 2–9 — Microservices (repeat per service)
1. Copy service folder (exclude `bin/obj/.vs`), rename namespaces + `*ApiRoutes.cs` unchanged paths (`/api/v1/...` stay compatible).
2. Fix `.csproj` → `ProjectReference ../SNUL.Shared/SNUL.Shared.csproj`; keep `Scalar, JwtBearer, OpenApi` refs.
3. Rewrite `appsettings.json/.Development/.Production/.Test`: new `DatabaseConnection` (SNUL_DB), new JWT issuers/audiences (SNUL ports), per-service extras (Auth: Email+Identity; Attachment: UploadPaths; Product: ExchangeRateSettings).
4. Rewrite `Properties/launchSettings.json` ports per table above.
5. `dotnet build` per service. Order: Auth → UserMgmt → Product → Commerce → Sales → Content → Cert → Attach.

### Phase 10 — Gateway upgrade
1. Add to existing `SNUL.Gateway.API.csproj`: `Ocelot 25.0, Scalar.AspNetCore, Serilog.AspNetCore/Console/File, JwtBearer, OpenApi` + `ProjectReference SNUL.Shared`.
2. Port `Welco.API/Program.cs` (Ocelot merge, JWT, CORS, RateLimiter, OpenApiAggregator, Scalar, Serilog, headers, `UseOcelot`).
3. Copy `Welco.API/Ocelot/ocelot.*.json` → `SNUL.Gateway.API/Ocelot/`, replace ports/BaseUrl, copy `Options/, Services/` (aggregator, warmer).
4. Copy `appsettings.json` Serilog/CORS/RateLimiting/OpenApiAggregator sections.

### Phase 11 — Cross-cutting (Done ✅)
Seeders, soft-delete + audit hooks, ValidationBehavior pipeline, RoleAuthorize Admin-bypass, Result envelope, `Accept-Language` handling, file-storage dirs, Hangfire job (Product).

### Phase 12 — DB + Run + Verify (Migration Ready / DB Update Deferred)
```bash
# from Auth.Services.API (or any service referencing SNUL.Shared):
# 1. Migration generated:
dotnet ef migrations add InitialSnul --project ../SNUL.Shared
# 2. Idempotent SQL script generated for hosting deployment:
#    SNUL.Shared/Migrations/InitialSnul.sql (apply whenever host DB is configured)
# 3. Apply migration to configured host DB:
dotnet ef database update --project ../SNUL.Shared
# 4. Run gateway or microservices:
dotnet run --project SNUL.Gateway.API --launch-profile https
# 5. Verify endpoints:
#    https://localhost:7266/scalar/v1
#    https://localhost:7266/openapi/all.json
#    POST https://localhost:7266/api/v1/auth/register → login → GET profile
#    GET  https://localhost:7266/api/v1/products (anonymous)
```

---

## 4. Configuration Template (per microservice, DO NOT commit secrets)

```json
{
  "ConnectionStrings": { "DatabaseConnection": "Server=...; Database=SNUL_DB; ..." },
  "JwtSettings": {
    "Issuer": "https://localhost:7303/",
    "Audience": "https://localhost:7303/",
    "ValidIssuers": ["https://localhost:7303/", "https://localhost:7266/", "https://snul-gateway.runasp.net/"],
    "ValidAudiences": ["https://localhost:7303/", "https://localhost:7266/", "https://snul-gateway.runasp.net/"],
    "ExpiryInMinutes": 60,
    "RefreshTokenExpiryDays": 30,
    "Secret": "<32+ chars via user-secrets>"
  },
  "Identity": { "RequiredLength": 6, "RequireUniqueEmail": true, "RequireConfirmedEmail": false },
  "EmailSettings": { "Host": "smtp.gmail.com", "Port": 587, "Email": "...", "Username": "...", "Password": "<app-password>" },
  "ExchangeRateSettings": { "BaseUrl": "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies", "BaseCurrency": "USD" }
}
```

```bash
dotnet user-secrets set "ConnectionStrings:DatabaseConnection" "Server=...;Database=SNUL_DB;..."
dotnet user-secrets set "JwtSettings:Secret" "<secret>"
```

## 5. Progress Log

| Phase | Status | Notes |
|---|---|---|
| 0 Prep | Done | .NET 10.0.202, source analysis complete, host DB secrets reserved for user host config |
| 1 SNUL.Shared | Done ✅ | Cloned 143 files, renamed to `SNUL.*`/`SnulDbContext`/`SnulStaff`, 0 Welco refs, `dotnet build` 0 warn/0 err (26s) |
| 2 Auth | Done ✅ | Cloned 41 files, `SNUL.Shared` ref, ports 7303/5166, sanitized secrets, `dotnet build` 0/0 |
| 3 UserMgmt | Done ✅ | Cloned 143 files, typo fixed `Manamgent→Management`, ports 7304/5167, `AddSnulJwtAuthentication` fix, `dotnet build` 0/0 |
| 4 Product | Done ✅ | Cloned 86 files, Hangfire kept, ports 7154/5341, ExchangeRateSettings kept, `dotnet build` 0 err |
| 5 Commerce | Done ✅ | Cloned 54 files, ports 7145/5176, build 0/0 |
| 6 Sales | Done ✅ | Cloned 39 files (RFQs/Quotes/ProductInquiries), ports 7146/5156, build 0/0 |
| 7 Content | Done ✅ | Cloned 120 files (10 controllers), ports 7147/5209, build 0/0 |
| 8 Cert | Done ✅ | Cloned 28 files, ports 7201/5300, build 0/0 |
| 9 Attach | Done ✅ | Cloned 29 files, UploadPaths RootPath→SNUL.Gateway wwwroot, ports 7280/5221, build 0/0 |
| 10 Gateway | Done ✅ | Ocelot 25.0 + Scalar + Serilog + 28 route files ported (7266), wwwroot/Providers copied, Microsoft.OpenApi→2.12.0 fix, build 0/0 |
| 11 Cross-cutting | Done ✅ | Seeders (Role, User, Currency, Location, LandingPage, BogusDemo), soft-delete + audit in `SnulDbContext`, `ValidationBehavior` in MediatR, `RoleAuthorize` Admin-bypass, `Result` envelope, `Accept-Language` (messages.en/ar), Hangfire recurring job in Product |
| 12 Verify & Migration | Done (DB update deferred) ✅ | Migration `InitialSnul` generated (`20260910093040_InitialSnul.cs` + Snapshot) + idempotent SQL script `SNUL.Shared/Migrations/InitialSnul.sql`. All 10 projects compile with 0 err/0 warn. Live DB update deferred per host DB configuration. |

## 6. Risks / Notes

- Welco `appsettings.Development.json` contains a live DB connection string + Gmail app-password — **do not copy**; use User Secrets / env vars.
- `UserManamgent` typo: rename to `UserManagement` in SNUL (update solution + namespaces + Ocelot `usermanagement` filename stays lowercase for route compat).
- Ocelot `DangerousAcceptAnyServerCertificateValidator: true` is dev-only; review before production.
- Migrations reference old `WelcoDbContext`; fresh `InitialSnul` migration has been generated in `SNUL.Shared/Migrations` along with idempotent `InitialSnul.sql` script.
- `Directory.Build.props` added at solution root with `<SatelliteResourceLanguages>en</SatelliteResourceLanguages>` and `<UseAppHost>false</UseAppHost>` to prevent out-of-disk-space issues on constrained volumes during local builds.
