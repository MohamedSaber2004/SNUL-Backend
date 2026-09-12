using SNUL.Shared.Common.Classes;

namespace SNUL.Shared.Domain.Models
{
    /// <summary>
    /// Local isolation map: which SNUL company/system owns a Welco provider.
    /// Dashboard lists are filtered through this table; unmapped providers are hidden
    /// and direct access to them returns 403. One market per system.
    /// </summary>
    public class WelcoProviderMap : BaseEntity<Guid>
    {
        public Guid WelcoProviderId { get; set; }

        public string System { get; set; } = "snul";

        public Guid? CompanyId { get; set; }
        public virtual Company? Company { get; set; }

        public static WelcoProviderMap Create(Guid welcoProviderId, string system, Guid? companyId, string createdBy)
        {
            var map = new WelcoProviderMap
            {
                WelcoProviderId = welcoProviderId,
                System = system.Trim().ToLowerInvariant(),
                CompanyId = companyId
            };
            map.MarkAsCreated(createdBy);
            return map;
        }
    }
}
