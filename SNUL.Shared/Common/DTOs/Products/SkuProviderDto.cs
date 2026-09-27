using SNUL.Shared.Common.DTOs.UserManagement;

namespace SNUL.Shared.Common.DTOs.Products
{
    /// <summary>
    /// One provider's offer for a given SKU (product detail "Offered by").
    /// Pairs the owning company with that company's own listing.
    /// </summary>
    public class SkuProviderDto
    {
        public CompanyDto? Company { get; set; }
        public ProductDto Listing { get; set; } = null!;
    }
}
