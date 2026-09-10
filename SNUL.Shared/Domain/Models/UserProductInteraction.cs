using SNUL.Shared.Common.Classes;
using SNUL.Shared.Enums;

namespace SNUL.Shared.Domain.Models
{
    public class UserProductInteraction : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public Guid ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public string Type { get; set; } = null!; // Wishlist, RecentlyViewed, Compare
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
