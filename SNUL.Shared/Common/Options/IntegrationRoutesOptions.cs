namespace SNUL.Shared.Common.Options
{
    /// <summary>
    /// Options pattern configuration for integration endpoint routes.
    /// Matches Welco's integration route structure.
    /// </summary>
    public class IntegrationRoutesOptions
    {
        public const string SectionName = "IntegrationRoutes";

        // Orders routes
        public string OrdersBase { get; set; } = "api/integration/orders";
        public string OrdersCreate { get; set; } = "";
        public string OrdersGetById { get; set; } = "{0}";
        public string OrdersUpdateStatus { get; set; } = "{0}/status";

        // Inventory routes
        public string InventoryBase { get; set; } = "api/integration/inventory";
        public string InventoryCheck { get; set; } = "check";
        public string InventoryReserve { get; set; } = "reserve";

        // Products routes
        public string ProductsBase { get; set; } = "api/integration/products";
        public string ProductsGetAll { get; set; } = "";
        public string ProductsGetById { get; set; } = "{0}";

        // Providers routes
        public string ProvidersBase { get; set; } = "api/integration/providers";
        public string ProvidersGetAll { get; set; } = "";

        // Quotes routes
        public string QuotesBase { get; set; } = "api/integration/quotes";
        public string QuotesCreate { get; set; } = "";
        public string QuotesUpdateStatus { get; set; } = "{0}/status";

        // Categories routes
        public string CategoriesBase { get; set; } = "api/integration/categories";
        public string CategoriesGetAll { get; set; } = "";
        public string CategoriesGetById { get; set; } = "{0}";

        // Distributors routes
        public string DistributorsBase { get; set; } = "api/integration/distributors";
        public string DistributorsApply { get; set; } = "apply";
        public string DistributorsGetAll { get; set; } = "";
        public string DistributorsGetById { get; set; } = "{0}";
        public string DistributorsApprove { get; set; } = "{0}/approve";
        public string DistributorsReject { get; set; } = "{0}/reject";

        // Support routes
        public string SupportBase { get; set; } = "api/integration/support";
        public string SupportTickets { get; set; } = "tickets";
        public string SupportTicketById { get; set; } = "tickets/{0}";
        public string SupportTicketReply { get; set; } = "tickets/{0}/reply";
        public string SupportTicketClose { get; set; } = "tickets/{0}/close";

        // Help Center & FAQs routes
        public string HelpBase { get; set; } = "api/integration/help";
        public string HelpArticles { get; set; } = "articles";
        public string HelpArticleById { get; set; } = "articles/{0}";
        public string HelpFaqs { get; set; } = "faqs";

        // Certifications routes
        public string CertificationsBase { get; set; } = "api/integration/certifications";
        public string CertificationsGetAll { get; set; } = "";
        public string CertificationsGetById { get; set; } = "{0}";
    }
}
