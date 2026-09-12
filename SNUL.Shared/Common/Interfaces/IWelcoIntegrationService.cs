using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Results;

namespace SNUL.Shared.Common.Interfaces
{
    /// <summary>
    /// Outbound gateway to Welco. Every call accepts an optional dashboard
    /// discriminator (<c>system</c>: "snul" | "welo"); null falls back to the
    /// configured default system, preserving existing callers unchanged.
    /// </summary>
    public interface IWelcoIntegrationService
    {

        Task<Result<List<ExternalProviderDto>>> GetProvidersAsync(CancellationToken ct = default, string? system = null);

        Task<Result<List<ExternalProductDto>>> GetProductsAsync(int page = 1, int pageSize = 50, CancellationToken ct = default, string? system = null);
        Task<Result<ExternalProductDto>> GetProductByIdAsync(Guid welcoProductId, CancellationToken ct = default, string? system = null);

        Task<Result<InventoryCheckResponse>> CheckInventoryAsync(InventoryCheckRequest request, CancellationToken ct = default, string? system = null);
        Task<Result<InventoryCheckResponse>> ReserveInventoryAsync(InventoryCheckRequest request, CancellationToken ct = default, string? system = null);

        Task<Result<ExternalOrderResponse>> CreateOrderAsync(CreateExternalOrderRequest request, CancellationToken ct = default, string? system = null);
        Task<Result<ExternalOrderResponse>> GetOrderByIdAsync(Guid welcoOrderId, CancellationToken ct = default, string? system = null);
        Task<Result<bool>> UpdateOrderStatusAsync(Guid welcoOrderId, UpdateExternalStatusRequest request, CancellationToken ct = default, string? system = null);

        Task<Result<ExternalQuoteResponse>> CreateQuoteAsync(CreateExternalQuoteRequest request, CancellationToken ct = default, string? system = null);
        Task<Result<bool>> UpdateQuoteStatusAsync(Guid welcoQuoteId, UpdateExternalStatusRequest request, CancellationToken ct = default, string? system = null);

        Task<Result<List<ExternalCategoryDto>>> GetCategoriesAsync(CancellationToken ct = default, string? system = null);
        Task<Result<ExternalCategoryDto>> GetCategoryByIdAsync(Guid welcoCategoryId, CancellationToken ct = default, string? system = null);

        Task<Result<DistributorApplicationDto>> SubmitDistributorApplicationAsync(ApplyDistributorRequest request, CancellationToken ct = default, string? system = null);
        Task<Result<List<DistributorApplicationDto>>> GetDistributorApplicationsAsync(CancellationToken ct = default, string? system = null);
        Task<Result<DistributorApplicationDto>> GetDistributorApplicationByIdAsync(Guid id, CancellationToken ct = default, string? system = null);
        Task<Result<bool>> ApproveDistributorApplicationAsync(Guid id, CancellationToken ct = default, string? system = null);
        Task<Result<bool>> RejectDistributorApplicationAsync(Guid id, string? reason = null, CancellationToken ct = default, string? system = null);

        Task<Result<List<ExternalSupportTicketDto>>> GetSupportTicketsAsync(string? status = null, CancellationToken ct = default, string? system = null);
        Task<Result<ExternalSupportTicketDto>> GetSupportTicketByIdAsync(Guid id, CancellationToken ct = default, string? system = null);
        Task<Result<bool>> ReplySupportTicketAsync(Guid id, string reply, CancellationToken ct = default, string? system = null);
        Task<Result<bool>> CloseSupportTicketAsync(Guid id, CancellationToken ct = default, string? system = null);

        Task<Result<List<ExternalHelpArticleDto>>> GetHelpArticlesAsync(CancellationToken ct = default, string? system = null);
        Task<Result<List<ExternalFAQDto>>> GetFaqsAsync(CancellationToken ct = default, string? system = null);

        Task<Result<List<ExternalCertificationDto>>> GetCertificationsAsync(CancellationToken ct = default, string? system = null);
        Task<Result<ExternalCertificationDto>> GetCertificationByIdAsync(Guid id, CancellationToken ct = default, string? system = null);
    }
}
