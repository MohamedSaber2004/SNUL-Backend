using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Domain.Models;

namespace SNUL.Shared.Common.Interfaces
{
    public interface ISnulDbContext : IAsyncDisposable
    {
        DbSet<ApplicationUser> ApplicationUsers { get; }
        DbSet<UserRefreshToken> UserRefreshTokens { get; }
        DbSet<Country> Countries { get; }
        DbSet<City> Cities { get; }
        DbSet<Zone> Zones { get; }
        DbSet<UserAddress> UserAddresses { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
