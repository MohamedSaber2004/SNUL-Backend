using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Enums;

namespace Sales.Services.API.Features.Shared
{
        internal static class BuyerScope
    {
        public sealed record Caller(bool IsOrganizationUser, Guid? CompanyId);

        public static async Task<Caller> GetAsync(IUnitOfWork uow, ICurrentUserService cur, CancellationToken ct)
        {
            if (cur.UserId == Guid.Empty) return new Caller(true, null);
            var user = await uow.GetRepository<ApplicationUser, Guid>().GetByIdAsync(cur.UserId, ct);
            if (user == null || user.IsDeleted) return new Caller(true, null);
            var isOrg = user.UserType == UserType.OrganizationUser;
            return new Caller(isOrg, isOrg ? user.CompanyId : null);
        }
    }
}
