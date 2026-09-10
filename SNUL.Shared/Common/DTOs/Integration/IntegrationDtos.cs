namespace SNUL.Shared.Common.DTOs.Integration
{
    // ── Categories ─────────────────────────────────────────────────────────────

    public class ExternalCategoryDto
    {
        public Guid Id { get; set; }
        public string NameEn { get; set; } = null!;
        public string NameAr { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageName { get; set; }
        public Guid? ParentCategoryId { get; set; }
    }

    // ── Inventory ──────────────────────────────────────────────────────────────

    public class InventoryCheckRequest
    {
        public List<InventoryCheckItem> Items { get; set; } = new();
    }

    public class InventoryCheckItem
    {
        public Guid WelcoProductId { get; set; }
        public int RequestedQuantity { get; set; }
    }

    public class InventoryCheckResponse
    {
        public bool AllAvailable { get; set; }
        public List<InventoryItemResult> Items { get; set; } = new();
    }

    public class InventoryItemResult
    {
        public Guid WelcoProductId { get; set; }
        public int AvailableStock { get; set; }
        public bool IsAvailable { get; set; }
    }

    // ── Orders ─────────────────────────────────────────────────────────────────

    public class CreateExternalOrderRequest
    {
        public string? SourceMarket { get; set; } = "Egypt";
        public Guid? ExternalCustomerId { get; set; }
        public Guid? CurrencyId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ExternalOrderItemRequest> Items { get; set; } = new();
    }

    public class ExternalOrderItemRequest
    {
        public Guid WelcoProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class ExternalOrderResponse
    {
        public Guid WelcoOrderId { get; set; }
        public string WelcoOrderNumber { get; set; } = null!;
        public string Status { get; set; } = null!;
    }

    public class UpdateExternalStatusRequest
    {
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
    }

    // ── Products ───────────────────────────────────────────────────────────────

    public class ExternalProductDto
    {
        public Guid Id { get; set; }
        public string NameEn { get; set; } = null!;
        public string NameAr { get; set; } = null!;
        public string Sku { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageName { get; set; }
        public Guid CategoryId { get; set; }
    }

    // ── Providers ──────────────────────────────────────────────────────────────

    public class ExternalProviderDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string Status { get; set; } = null!;
        public bool IsProvider { get; set; }
    }

    // ── Distributor Applications ───────────────────────────────────────────────

    public class ApplyDistributorRequest
    {
        public string CompanyName { get; set; } = null!;
        public string ContactPerson { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public Guid CountryId { get; set; }
        public string? SalesVolumeBand { get; set; }
        public string? CategoryInterest { get; set; }
        public string? Website { get; set; }
        public string SourceMarket { get; set; } = "Egypt";
    }

    public class DistributorApplicationDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string ContactPerson { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }
        public string? SalesVolumeBand { get; set; }
        public string? CategoryInterest { get; set; }
        public string? Website { get; set; }
        public string Status { get; set; } = null!; // Pending, Approved, Rejected
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateDistributorStatusRequest
    {
        public string Status { get; set; } = null!; // Approved, Rejected
        public string? Reason { get; set; }
    }

    // ── Quotes ─────────────────────────────────────────────────────────────────

    public class CreateExternalQuoteRequest
    {
        public string? SourceMarket { get; set; } = "Egypt";
        public List<ExternalQuoteItemRequest> Items { get; set; } = new();
    }

    public class ExternalQuoteItemRequest
    {
        public Guid WelcoProductId { get; set; }
        public int Quantity { get; set; }
        public decimal? RequestedUnitPrice { get; set; }
        public string? Notes { get; set; }
    }

    public class ExternalQuoteResponse
    {
        public Guid WelcoRfqId { get; set; }
        public string WelcoRfqNumber { get; set; } = null!;
        public string Status { get; set; } = null!;
    }

    // ── Support Tickets ────────────────────────────────────────────────────────

    public class ExternalSupportTicketDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Subject { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Status { get; set; } = null!; // Open, Answered, Closed
        public string? Reply { get; set; }
        public DateTime? RepliedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ReplySupportTicketRequest
    {
        public string Reply { get; set; } = null!;
    }

    // ── Help Center & FAQs ─────────────────────────────────────────────────────

    public class ExternalHelpArticleDto
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public string Slug { get; set; } = null!;
    }

    public class ExternalFAQDto
    {
        public Guid Id { get; set; }
        public string Question { get; set; } = null!;
        public string Answer { get; set; } = null!;
        public int SortOrder { get; set; }
    }

    // ── Certifications ─────────────────────────────────────────────────────────

    public class ExternalCertificationDto
    {
        public Guid Id { get; set; }
        public string CertificateNumber { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string IssuedTo { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Description { get; set; }
        public string? CertificationImageName { get; set; }
    }
}
