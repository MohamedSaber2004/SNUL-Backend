using SNUL.Shared.Common.Classes;
namespace SNUL.Shared.Domain.Models
{
    public class Document : BaseEntity<Guid>
    {
        public string Title { get; set; } = null!;
        public string DocType { get; set; } = null!; 
        public string FileUrl { get; set; } = null!;
        public int FileSizeKB { get; set; }
        public Guid? ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public DateTime PublishedDate { get; set; }
    }
    public class LandingPage : BaseEntity<Guid>
    {
        public string Type { get; set; } = null!; 
        public string Slug { get; set; } = null!;
        public string HeroTitle { get; set; } = null!;
        public string? HeroBody { get; set; }
        public string? ContentBlock { get; set; }
    }
    public class HelpCategory : BaseEntity<Guid>
    {
        public string Name { get; set; } = null!;
        public string? NameAr { get; set; } // Arabic
        public string? Icon { get; set; }
        public virtual ICollection<HelpArticle> Articles { get; set; } = new List<HelpArticle>();
    }
    public class HelpArticle : BaseEntity<Guid>
    {
        public Guid CategoryId { get; set; }
        public virtual HelpCategory? Category { get; set; }
        public string Title { get; set; } = null!;
        public string? TitleAr { get; set; } // Arabic
        public string Body { get; set; } = null!;
        public string? BodyAr { get; set; } // Arabic
        public string Slug { get; set; } = null!;
    }
    public class FAQItem : BaseEntity<Guid>
    {
        public string Question { get; set; } = null!;
        public string? QuestionAr { get; set; } // Arabic
        public string Answer { get; set; } = null!;
        public string? AnswerAr { get; set; } // Arabic
        public int SortOrder { get; set; }
    }
    public class Notification : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public string Type { get; set; } = null!;
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; }
    }
    public class SupportTicket : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public string Subject { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Status { get; set; } = "Open"; 
        public string? Reply { get; set; }
        public DateTime? RepliedAt { get; set; }
        public Guid? RepliedBy { get; set; }
    }
    public class SupportContact : BaseEntity<Guid>
    {
        public string SupportEmail { get; set; } = "support@snul.health";
        // No invented defaults: these are admin-managed via
        // PUT /api/v1/support/contact. Empty means "not configured yet".
        public string PhoneNumber { get; set; } = string.Empty;
        public string WhatsAppNumber { get; set; } = string.Empty;
        public string? WorkingHours { get; set; }
    }
    public class OemInquiry : BaseEntity<Guid>
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string ServiceType { get; set; } = null!;
        public string Message { get; set; } = null!;
    }

    /// <summary>
    /// Telemetry shown on the public Help Center hero, editable by an admin via
    /// /api/v1/help/site-stats so business claims can be corrected or withdrawn
    /// without a code deploy.
    /// <para>
    /// <see cref="StatKey"/> is the contract: the frontend keys off it, the value
    /// is data. The four intended keys are
    /// <c>lotTraceable</c> (the "100%" metric), <c>isoStandard</c> (the "ISO 13485"
    /// metric), <c>resolutionRate</c> (the "99.4%" metric) and <c>slaBadge</c> (the
    /// "&lt; 2h SLA" badge). Renaming a key breaks that contract, so keys are
    /// data-contract, not labels.
    /// </para>
    /// <para>
    /// <see cref="IsVisible"/> exists so a claim can be withdrawn without deleting
    /// its row: flip the flag and the public queries stop returning it. Nothing
    /// here is seeded and no property carries a default, because these are
    /// operator-supplied facts. A database with no rows means nothing to show.
    /// </para>
    /// </summary>
    public class HelpSiteStat : BaseEntity<Guid>
    {
        public string StatKey { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string Label { get; set; } = null!;
        public string? LabelAr { get; set; } // Arabic
        public int SortOrder { get; set; }
        public bool IsVisible { get; set; }
    }
    public class SiteLogo : BaseEntity<Guid>
    {
        /// <summary>
        /// Full URL of the logo image (CDN/attachment URL).
        /// Admin-managed via PUT /api/v1/content/logo. Empty means "use the default static logo".
        /// </summary>
        public string LogoUrl { get; set; } = string.Empty;

        /// <summary>Accessible alt text for the logo image.</summary>
        public string AltText { get; set; } = "SNUL";
    }
}
