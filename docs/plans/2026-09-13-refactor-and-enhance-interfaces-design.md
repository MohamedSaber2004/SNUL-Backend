# Refactor and Enhance Interfaces Design Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use subagent-driven-development (recommended) or executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Refactor, standardize, and enhance all core C# code interfaces across `SNUL.Shared` and dependent microservices to follow Interface Segregation (ISP), consistent asynchronous naming, robust pagination, typed response contracts, and clean domain abstractions.

**Architecture:** Split monolith and legacy interface designs into cohesive, decoupled contracts: decompose entity interfaces (`IEntity<TKey>`, `IAuditableEntity`, `ISoftDeletableEntity`), refine repository & unit-of-work abstractions with clean async and pagination signatures, modernize file storage and validation abstractions to avoid tuple anti-patterns, and align all context and service contracts.

**Tech Stack:** .NET 10, C# 13, Entity Framework Core, ASP.NET Core, FluentValidation, MediatR, xUnit.

---

## File Structure & Decomposition Map

| Layer / Target | File Path | Responsibility |
| :--- | :--- | :--- |
| **Context Contract** | `SNUL.Shared/Common/Interfaces/ISnulDbContext.cs` | Renamed from legacy `IWelcoDbContext.cs`, defines EF Core DbSet contracts and transaction lifecycle. |
| **Entity Contracts** | `SNUL.Shared/Common/Interfaces/IBaseEntity.cs` | Segmented entity contracts: `IEntity<TKey>`, `IAuditableEntity`, `ISoftDeletableEntity`, `IActiveEntity`. |
| **Repository Contract** | `SNUL.Shared/Common/Repositories/Interfaces/Base/IGenericRepository.cs` | Clean repository abstraction with proper async methods, pagination, specification, and predicate filters. |
| **Unit of Work Contract**| `SNUL.Shared/Common/Repositories/Interfaces/Base/IUnitOfWork.cs` | Enhanced transactional lifecycle contract with execution strategy support and repository factory. |
| **Storage Contract** | `SNUL.Shared/Common/Interfaces/IBaseFileService.cs` | Modernized file operations returning `Result<T>` and supporting stream + form file overloads. |
| **Validation Contracts**| `SNUL.Shared/Common/Interfaces/IFileValidator.cs` | Unified file validator base interface for image, video, audio, and documents. |
| **Service Contracts** | `SNUL.Shared/Common/Interfaces/ICurrentUserService.cs` | Claims, roles, authentication state, and tenant metadata contract. |
| **Email Contract** | `SNUL.Shared/Common/Interfaces/IEmailService.cs` | Enhanced email contract supporting cancellation tokens, attachments, and templating. |

---

### Task 1: Rename and Refactor DbContext Interface Contract

**Files:**
- Create: `SNUL.Shared/Common/Interfaces/ISnulDbContext.cs`
- Modify: `SNUL.Shared/Persistance/SnulDbContext.cs`
- Modify: `SNUL.Shared/DependencyInjection.cs`
- Delete: `SNUL.Shared/Common/Interfaces/IWelcoDbContext.cs`

- [ ] **Step 1: Create the new `ISnulDbContext.cs` interface file**

Create `E:\SNUL Site\backend\SNUL\SNUL.Shared\Common\Interfaces\ISnulDbContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SNUL.Shared.Domain.Models;

namespace SNUL.Shared.Common.Interfaces
{
    public interface ISnulDbContext : IAsyncDisposable, IDisposable
    {
        DbSet<ApplicationUser> ApplicationUsers { get; }
        DbSet<UserRefreshToken> UserRefreshTokens { get; }
        DbSet<Country> Countries { get; }
        DbSet<City> Cities { get; }
        DbSet<Zone> Zones { get; }
        DbSet<UserAddress> UserAddresses { get; }
        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
    }
}
```

- [ ] **Step 2: Delete legacy `IWelcoDbContext.cs`**

Remove file `E:\SNUL Site\backend\SNUL\SNUL.Shared\Common\Interfaces\IWelcoDbContext.cs`.

- [ ] **Step 3: Update `SnulDbContext.cs` and `DependencyInjection.cs` references**

Ensure `SnulDbContext` implements `ISnulDbContext` and `DependencyInjection.cs` registers `services.AddScoped<ISnulDbContext>(sp => sp.GetRequiredService<SnulDbContext>());`.

- [ ] **Step 4: Verify build succeeds**

Run: `dotnet build "E:\SNUL Site\backend\SNUL\SNUL.Shared\SNUL.Shared.csproj"`
Expected: Build succeeded with 0 errors.

---

### Task 2: Segment and Enhance Entity Interfaces (ISP)

**Files:**
- Modify: `SNUL.Shared/Common/Interfaces/IBaseEntity.cs`
- Modify: `SNUL.Shared/Common/Models/Base/BaseEntity.cs` (or base models implementing `IBaseEntity`)

- [ ] **Step 1: Write failing unit test for entity interface contracts**

Create test file in `SNUL.Shared.Tests/EntityInterfaceTests.cs`:

```csharp
using SNUL.Shared.Common.Interfaces;
using Xunit;

namespace SNUL.Shared.Tests
{
    public class SampleEntity : IEntity<Guid>, IAuditableEntity, ISoftDeletableEntity, IActiveEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }

    public class EntityInterfaceTests
    {
        [Fact]
        public void SampleEntity_ImplementsSegregatedInterfaces()
        {
            var entity = new SampleEntity { Id = Guid.NewGuid(), IsActive = true };
            Assert.IsAssignableFrom<IEntity<Guid>>(entity);
            Assert.IsAssignableFrom<IAuditableEntity>(entity);
            Assert.IsAssignableFrom<ISoftDeletableEntity>(entity);
            Assert.IsAssignableFrom<IActiveEntity>(entity);
        }
    }
}
```

- [ ] **Step 2: Refactor `IBaseEntity.cs` with segregated interfaces**

Update `E:\SNUL Site\backend\SNUL\SNUL.Shared\Common\Interfaces\IBaseEntity.cs`:

```csharp
namespace SNUL.Shared.Common.Interfaces
{
    public interface IEntity<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
    }

    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        string CreatedBy { get; set; }
        string? UpdatedBy { get; set; }
    }

    public interface ISoftDeletableEntity
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
        string? DeletedBy { get; set; }
    }

    public interface IActiveEntity
    {
        bool IsActive { get; set; }
    }

    public interface IBaseEntity<TKey> : IEntity<TKey>, IAuditableEntity, ISoftDeletableEntity, IActiveEntity
        where TKey : IEquatable<TKey>
    {
    }
}
```

- [ ] **Step 3: Run test to verify it passes**

Run: `dotnet test "E:\SNUL Site\backend\SNUL\SNUL.Shared.Tests\SNUL.Shared.Tests.csproj" --filter "FullyQualifiedName~EntityInterfaceTests"`
Expected: PASS.

---

### Task 3: Refactor Repository and Unit of Work Interfaces

**Files:**
- Modify: `SNUL.Shared/Common/Repositories/Interfaces/Base/IGenericRepository.cs`
- Modify: `SNUL.Shared/Common/Repositories/Interfaces/Base/IUnitOfWork.cs`
- Modify: `SNUL.Shared/Common/Repositories/Implementation/Base/GenericRepository.cs`
- Modify: `SNUL.Shared/Common/Repositories/Implementation/Base/UnitOfWork.cs`

- [ ] **Step 1: Write unit tests for enhanced repository and UnitOfWork methods**

Add unit tests in `SNUL.Shared.Tests/RepositoryInterfaceTests.cs` testing pagination, predicate counts, and transaction execution helpers.

- [ ] **Step 2: Update `IGenericRepository.cs`**

Refactor `E:\SNUL Site\backend\SNUL\SNUL.Shared\Common\Repositories\Interfaces\Base\IGenericRepository.cs`:
- Remove misleading `IQueryable<T> GetAllAsync(...)` method (as `IQueryable` is synchronous and deferred; replace with clear `GetQueryable(bool asNoTracking = false)` and `GetAllListAsync(...)`).
- Add `Task<PagedResult<T>> GetPagedListAsync(int pageIndex, int pageSize, Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, CancellationToken cancellationToken = default)`.
- Ensure all mutating operations have consistent cancellation token parameters.

- [ ] **Step 3: Update `IUnitOfWork.cs`**

Refactor `E:\SNUL Site\backend\SNUL\SNUL.Shared\Common\Repositories\Interfaces\Base\IUnitOfWork.cs`:
- Add `Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action, CancellationToken cancellationToken = default);`
- Add `Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);`

- [ ] **Step 4: Update implementations in `GenericRepository.cs` and `UnitOfWork.cs`**

Implement the new contract methods and ensure existing repository calls remain backwards-compatible.

- [ ] **Step 5: Run tests and verify build**

Run: `dotnet test "E:\SNUL Site\backend\SNUL\SNUL.Shared.Tests\SNUL.Shared.Tests.csproj"`
Expected: All tests pass.

---

### Task 4: Modernize File Storage and Validator Interfaces

**Files:**
- Modify: `SNUL.Shared/Common/Interfaces/IBaseFileService.cs`
- Modify: `SNUL.Shared/Common/Interfaces/IFileValidator.cs`
- Modify: `SNUL.Shared/Common/Interfaces/IImageValidator.cs`
- Modify: `SNUL.Shared/Common/Interfaces/IVideoValidator.cs`
- Modify: `SNUL.Shared/Common/Interfaces/IAudioValidator.cs`

- [ ] **Step 1: Refactor `IBaseFileService.cs`**

Replace tuple responses with `Result<T>` and add Stream overloads:

```csharp
using Microsoft.AspNetCore.Http;
using SNUL.Shared.Results;

namespace SNUL.Shared.Common.Interfaces
{
    public interface IBaseFileService
    {
        Task<Result<string>> UploadFileAsync(IFormFile file, string folderPath, CancellationToken cancellationToken = default);
        Task<Result<string>> UploadStreamAsync(Stream stream, string fileName, string folderPath, CancellationToken cancellationToken = default);
        bool FileExists(string? fullFilePath);
        Task<Result<bool>> DeleteFileAsync(string fileName, string folderPath, CancellationToken cancellationToken = default);
        string GetUniqueFileName(string fileName);
        Task<Result<byte[]>> DownloadFileAsync(string folderPath, string fileName, CancellationToken cancellationToken = default);
    }
}
```

- [ ] **Step 2: Refactor Validator Interfaces**

Unify file validator contracts with typed result contracts (`ValidationResult` or `Result<bool>`) and stream/form-file validation methods.

- [ ] **Step 3: Update implementations and verify build**

Run: `dotnet build "E:\SNUL Site\backend\SNUL\SNUL.Shared\SNUL.Shared.csproj"`
Expected: Build succeeded.

---

### Task 5: Enhance Service Interfaces (CurrentUserService, EmailService, ExchangeRateService)

**Files:**
- Modify: `SNUL.Shared/Common/Interfaces/ICurrentUserService.cs`
- Modify: `SNUL.Shared/Common/Interfaces/IEmailService.cs`
- Modify: `SNUL.Shared/Common/Interfaces/IExchangeRateService.cs`

- [ ] **Step 1: Enhance `ICurrentUserService.cs`**

Add role evaluation, claims extraction, and tenant/market attributes:

```csharp
using System.Security.Claims;

namespace SNUL.Shared.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
        string? IpAddress { get; }
        string? UserAgent { get; }
        string? Market { get; }
        ClaimsPrincipal? User { get; }
        bool IsInRole(string role);
        string? GetClaim(string claimType);
    }
}
```

- [ ] **Step 2: Enhance `IEmailService.cs`**

Add cancellation tokens and structured email request overloads.

- [ ] **Step 3: Update implementations and verify build across all projects**

Run: `dotnet build "E:\SNUL Site\backend\SNUL\SNUL.sln"`
Expected: 0 Warnings, 0 Errors.

---

### Task 6: Full Solution Build, Verification & Regression Testing

- [ ] **Step 1: Build entire solution**

Run: `dotnet build "E:\SNUL Site\backend\SNUL\SNUL.sln" --no-incremental`
Expected: Build succeeded with 0 warnings and 0 errors.

- [ ] **Step 2: Run all unit and integration tests**

Run: `dotnet test "E:\SNUL Site\backend\SNUL\SNUL.Shared.Tests\SNUL.Shared.Tests.csproj"`
Expected: All tests pass.
