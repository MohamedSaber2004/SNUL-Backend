using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Results;

namespace SNUL.Shared.Common.Interfaces
{
    public interface IWelcoIntegrationService
    {

        Task<Result<List<ExternalProviderDto>>> GetProvidersAsync(CancellationToken ct = default);

        Task<Result<List<ExternalProductDto>>> GetProductsAsync(int page = 1, int pageSize = 50, CancellationToken ct = default);
        Task<Result<ExternalProductDto>> GetProductByIdAsync(Guid welcoProductId, CancellationToken ct = default);

        Task<Result<InventoryCheckResponse>> CheckInventoryAsync(InventoryCheckRequest request, CancellationToken ct = default);
        Task<Result<InventoryCheckResponse>> ReserveInventoryAsync(InventoryCheckRequest request, CancellationToken ct = default);

        Task<Result<ExternalOrderResponse>> CreateOrderAsync(CreateExternalOrderRequest request, CancellationToken ct = default);
        Task<Result<ExternalOrderResponse>> GetOrderByIdAsync(Guid welcoOrderId, CancellationToken ct = default);
        Task<Result<bool>> UpdateOrderStatusAsync(Guid welcoOrderId, UpdateExternalStatusRequest request, CancellationToken ct = default);

        Task<Result<ExternalQuoteResponse>> CreateQuoteAsync(CreateExternalQuoteRequest request, CancellationToken ct = default);
        Task<Result<bool>> UpdateQuoteStatusAsync(Guid welcoQuoteId, UpdateExternalStatusRequest request, CancellationToken ct = default);

        Task<Result<List<ExternalCategoryDto>>> GetCategoriesAsync(CancellationToken ct = default);
        Task<Result<ExternalCategoryDto>> GetCategoryByIdAsync(Guid welcoCategoryId, CancellationToken ct = default);

        Task<Result<DistributorApplicationDto>> SubmitDistributorApplicationAsync(ApplyDistributorRequest request, CancellationToken ct = default);
        Task<Result<List<DistributorApplicationDto>>> GetDistributorApplicationsAsync(CancellationToken ct = default);
        Task<Result<DistributorApplicationDto>> GetDistributorApplicationByIdAsync(Guid id, CancellationToken ct = default);
        Task<Result<bool>> ApproveDistributorApplicationAsync(Guid id, CancellationToken ct = default);
        Task<Result<bool>> RejectDistributorApplicationAsync(Guid id, string? reason = null, CancellationToken ct = default);

        Task<Result<List<ExternalSupportTicketDto>>> GetSupportTicketsAsync(string? status = null, CancellationToken ct = default);
        Task<Result<ExternalSupportTicketDto>> GetSupportTicketByIdAsync(Guid id, CancellationToken ct = default);
        Task<Result<bool>> ReplySupportTicketAsync(Guid id, string reply, CancellationToken ct = default);
        Task<Result<bool>> CloseSupportTicketAsync(Guid id, CancellationToken ct = default);

        Task<Result<List<ExternalHelpArticleDto>>> GetHelpArticlesAsync(CancellationToken ct = default);
        Task<Result<List<ExternalFAQDto>>> GetFaqsAsync(CancellationToken ct = default);

        Task<Result<List<ExternalCertificationDto>>> GetCertificationsAsync(CancellationToken ct = default);
        Task<Result<ExternalCertificationDto>> GetCertificationByIdAsync(Guid id, CancellationToken ct = default);
    }
}
