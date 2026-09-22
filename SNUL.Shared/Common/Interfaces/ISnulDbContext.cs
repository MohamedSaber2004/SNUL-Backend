using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Domain.Models;

namespace SNUL.Shared.Common.Interfaces
{
    public interface ISnulDbContext : IAsyncDisposable
    {
        // Identity & Auth
        DbSet<ApplicationUser> ApplicationUsers { get; }
        DbSet<UserRefreshToken> UserRefreshTokens { get; }

        // Geography
        DbSet<Country> Countries { get; }
        DbSet<City> Cities { get; }
        DbSet<Zone> Zones { get; }

        // Addresses
        DbSet<UserAddress> UserAddresses { get; }
        DbSet<CompanyAddress> CompanyAddresses { get; }

        // Company & Certification
        DbSet<Company> Companies { get; }
        DbSet<Certification> Certifications { get; }

        // Catalogue
        DbSet<Category> Categories { get; }
        DbSet<Product> Products { get; }
        DbSet<ProductSpecification> ProductSpecifications { get; }
        DbSet<ProductMedia> ProductMedias { get; }
        DbSet<UserProductInteraction> UserProductInteractions { get; }

        // Currency & Exchange
        DbSet<Currency> Currencies { get; }
        DbSet<ExchangeRate> ExchangeRates { get; }
        DbSet<ExchangeRateSyncLog> ExchangeRateSyncLogs { get; }

        // Commerce (Cart / Order)
        DbSet<Cart> Carts { get; }
        DbSet<CartItem> CartItems { get; }
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }

        // Sales (RFQ / Quote / Inquiry / Distributor)
        DbSet<RFQ> RFQs { get; }
        DbSet<RFQItem> RFQItems { get; }
        DbSet<Quote> Quotes { get; }
        DbSet<QuoteItem> QuoteItems { get; }
        DbSet<ProductInquiry> ProductInquiries { get; }
        DbSet<DistributorApplication> DistributorApplications { get; }

        // Content
        DbSet<Document> Documents { get; }
        DbSet<LandingPage> LandingPages { get; }
        DbSet<HelpCategory> HelpCategories { get; }
        DbSet<HelpArticle> HelpArticles { get; }
        DbSet<FAQItem> FAQItems { get; }

        // Support
        DbSet<SupportTicket> SupportTickets { get; }
        DbSet<SupportContact> SupportContacts { get; }
        DbSet<OemInquiry> OemInquiries { get; }


        // Audit
        DbSet<AuditLog> AuditLogs { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
