using System.Security.Claims;
using SNUL.Shared.Domain.Models;

namespace SNUL.Shared.Common.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(ApplicationUser user, IList<string> roles, Guid? clinicId = null, bool hasActiveSubscription = false);
        string GenerateRefreshToken(ApplicationUser user);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
