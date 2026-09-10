using Commerce.Services.API.Features.Integration.Categories.Queries.GetExternalCategories;
using Commerce.Services.API.Features.Integration.Categories.Queries.GetExternalCategoryById;
using Commerce.Services.API.Features.Integration.Certifications;
using Commerce.Services.API.Features.Integration.Distributors;
using Commerce.Services.API.Features.Integration.Help;
using Commerce.Services.API.Features.Integration.Inventory.Commands.ReserveExternalInventory;
using Commerce.Services.API.Features.Integration.Inventory.Queries.CheckExternalInventory;
using Commerce.Services.API.Features.Integration.Orders.Commands.CreateExternalOrder;
using Commerce.Services.API.Features.Integration.Orders.Commands.UpdateExternalOrderStatus;
using Commerce.Services.API.Features.Integration.Orders.Queries.GetExternalOrderById;
using Commerce.Services.API.Features.Integration.Products.Queries.GetExternalProductById;
using Commerce.Services.API.Features.Integration.Products.Queries.GetExternalProducts;
using Commerce.Services.API.Features.Integration.Providers.Queries.GetExternalProviders;
using Commerce.Services.API.Features.Integration.Quotes.Commands.CreateExternalQuote;
using Commerce.Services.API.Features.Integration.Quotes.Commands.UpdateExternalQuoteStatus;
using Commerce.Services.API.Features.Integration.Support;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Controllers;

namespace Commerce.Services.API.Controllers
{
    /// <summary>
    /// Snul-internal controller that proxies calls through to Welco's integration API.
    /// Only accessible by Admin role — NOT exposed through Snul's Ocelot gateway to the public.
    /// Uses MediatR pattern to invoke IWelcoIntegrationService.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/v1/integration")]
    [Tags("Welco Integration (Internal Admin Dashboard)")]
    public class IntegrationController : AppControllerBase
    {
        public IntegrationController(IMediator mediator) : base(mediator) { }

        // ── Providers ─────────────────────────────────────────────────────────

        /// <summary>Fetch all Welco providers.</summary>
        [HttpGet("providers")]
        public async Task<IActionResult> GetProviders(CancellationToken ct)
            => ToActionResult(await _mediator.Send(new GetExternalProvidersQuery(), ct));

        // ── Products ──────────────────────────────────────────────────────────

        /// <summary>Fetch products from Welco (for admin sync operations).</summary>
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
            => ToActionResult(await _mediator.Send(new GetExternalProductsQuery { Page = page, PageSize = pageSize }, ct));

        /// <summary>Fetch a single Welco product by ID.</summary>
        [HttpGet("products/{welcoProductId:guid}")]
        public async Task<IActionResult> GetProduct([FromRoute] Guid welcoProductId, CancellationToken ct)
            => ToActionResult(await _mediator.Send(new GetExternalProductByIdQuery { WelcoProductId = welcoProductId }, ct));

        // ── Inventory ──────────────────────────────────────────────────────────

        /// <summary>Check real-time inventory availability from Welco warehouse.</summary>
        [HttpPost("inventory/check")]
        public async Task<IActionResult> CheckInventory([FromBody] CheckExternalInventoryQuery query, CancellationToken ct)
            => ToActionResult(await _mediator.Send(query, ct));

        /// <summary>Reserve inventory from Welco warehouse.</summary>
        [HttpPost("inventory/reserve")]
        public async Task<IActionResult> ReserveInventory([FromBody] ReserveExternalInventoryCommand command, CancellationToken ct)
            => ToActionResult(await _mediator.Send(command, ct));

        // ── Orders ─────────────────────────────────────────────────────────────

        /// <summary>Push an order from Snul to Welco.</summary>
        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateExternalOrderCommand command, CancellationToken ct)
            => ToActionResult(await _mediator.Send(command, ct));

        /// <summary>Get a Welco order status by Welco order ID.</summary>
        [HttpGet("orders/{welcoOrderId:guid}")]
        public async Task<IActionResult> GetOrder([FromRoute] Guid welcoOrderId, CancellationToken ct)
            => ToActionResult(await _mediator.Send(new GetExternalOrderByIdQuery { WelcoOrderId = welcoOrderId }, ct));

        /// <summary>Update the status of an order on Welco side.</summary>
        [HttpPut("orders/{welcoOrderId:guid}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
            [FromRoute] Guid welcoOrderId,
            [FromBody] UpdateExternalStatusRequest request,
            CancellationToken ct)
            => ToActionResult(await _mediator.Send(new UpdateExternalOrderStatusCommand
            {
                WelcoOrderId = welcoOrderId,
                Status = request.Status,
                Notes = request.Notes
            }, ct));

        // ── Quotes ─────────────────────────────────────────────────────────────

        /// <summary>Forward an Egypt RFQ to Welco's quote system.</summary>
        [HttpPost("quotes")]
        public async Task<IActionResult> CreateQuote([FromBody] CreateExternalQuoteCommand command, CancellationToken ct)
            => ToActionResult(await _mediator.Send(command, ct));

        /// <summary>Update a Welco quote status.</summary>
        [HttpPut("quotes/{welcoQuoteId:guid}/status")]
        public async Task<IActionResult> UpdateQuoteStatus(
            [FromRoute] Guid welcoQuoteId,
            [FromBody] UpdateExternalStatusRequest request,
            CancellationToken ct)
            => ToActionResult(await _mediator.Send(new UpdateExternalQuoteStatusCommand
            {
                WelcoQuoteId = welcoQuoteId,
                Status = request.Status,
                Notes = request.Notes
            }, ct));

        // ── Categories ─────────────────────────────────────────────────────────

        /// <summary>Fetch all categories from Welco (for taxonomy sync).</summary>
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories(CancellationToken ct)
            => ToActionResult(await _mediator.Send(new GetExternalCategoriesQuery(), ct));

        /// <summary>Fetch a single Welco category by ID.</summary>
        [HttpGet("categories/{welcoCategoryId:guid}")]
        public async Task<IActionResult> GetCategory([FromRoute] Guid welcoCategoryId, CancellationToken ct)
            => ToActionResult(await _mediator.Send(new GetExternalCategoryByIdQuery { WelcoCategoryId = welcoCategoryId }, ct));

        // ── Distributors ───────────────────────────────────────────────────────

        /// <summary>Submit a distributor application to Welco.</summary>
        [HttpPost("distributors/apply")]
        public async Task<IActionResult> ApplyDistributor([FromBody] ApplyDistributorRequest request, CancellationToken ct)
            => ToActionResult(await _mediator.Send(new ApplyExternalDistributorCommand { Request = request }, ct));

        /// <summary>Get all distributor applications from Welco.</summary>
        [HttpGet("distributors")]
        public async Task<IActionResult> GetDistributors(CancellationToken ct)
            => ToActionResult(await _mediator.Send(new GetExternalDistributorsQuery(), ct));

        /// <summary>Get a distributor application by ID from Welco.</summary>
        [HttpGet("distributors/{id:guid}")]
        public async Task<IActionResult> GetDistributorById([FromRoute] Guid id, CancellationToken ct)
            => ToActionResult(await _mediator.Send(new GetExternalDistributorByIdQuery { Id = id }, ct));

        /// <summary>Approve a distributor application on Welco.</summary>
        [HttpPut("distributors/{id:guid}/approve")]
        public async Task<IActionResult> ApproveDistributor([FromRoute] Guid id, CancellationToken ct)
            => ToActionResult(await _mediator.Send(new ApproveExternalDistributorCommand { Id = id }, ct));

        /// <summary>Reject a distributor application on Welco.</summary>
        [HttpPut("distributors/{id:guid}/reject")]
        public async Task<IActionResult> RejectDistributor([FromRoute] Guid id, [FromBody] UpdateDistributorStatusRequest? request, CancellationToken ct)
            => ToActionResult(await _mediator.Send(new RejectExternalDistributorCommand { Id = id, Reason = request?.Reason }, ct));

        // ── Support Tickets ────────────────────────────────────────────────────

        /// <summary>Get support tickets from Welco.</summary>
        [HttpGet("support/tickets")]
        public async Task<IActionResult> GetSupportTickets([FromQuery] string? status, CancellationToken ct)
            => ToActionResult(await _mediator.Send(new GetExternalSupportTicketsQuery { Status = status }, ct));

        /// <summary>Get a support ticket by ID from Welco.</summary>
        [HttpGet("support/tickets/{id:guid}")]
        public async Task<IActionResult> GetSupportTicketById([FromRoute] Guid id, CancellationToken ct)
            => ToActionResult(await _mediator.Send(new GetExternalSupportTicketByIdQuery { Id = id }, ct));

        /// <summary>Reply to a support ticket on Welco.</summary>
        [HttpPost("support/tickets/{id:guid}/reply")]
        public async Task<IActionResult> ReplySupportTicket([FromRoute] Guid id, [FromBody] ReplySupportTicketRequest request, CancellationToken ct)
            => ToActionResult(await _mediator.Send(new ReplyExternalSupportTicketCommand { Id = id, Reply = request.Reply }, ct));

        /// <summary>Close a support ticket on Welco.</summary>
        [HttpPost("support/tickets/{id:guid}/close")]
        public async Task<IActionResult> CloseSupportTicket([FromRoute] Guid id, CancellationToken ct)
            => ToActionResult(await _mediator.Send(new CloseExternalSupportTicketCommand { Id = id }, ct));

        // ── Help Center & FAQs ─────────────────────────────────────────────────

        /// <summary>Get help articles from Welco.</summary>
        [HttpGet("help/articles")]
        public async Task<IActionResult> GetHelpArticles(CancellationToken ct)
            => ToActionResult(await _mediator.Send(new GetExternalHelpArticlesQuery(), ct));

        /// <summary>Get FAQs from Welco.</summary>
        [HttpGet("help/faqs")]
        public async Task<IActionResult> GetFaqs(CancellationToken ct)
            => ToActionResult(await _mediator.Send(new GetExternalFaqsQuery(), ct));

        // ── Certifications ─────────────────────────────────────────────────────

        /// <summary>Get certifications from Welco.</summary>
        [HttpGet("certifications")]
        public async Task<IActionResult> GetCertifications(CancellationToken ct)
            => ToActionResult(await _mediator.Send(new GetExternalCertificationsQuery(), ct));

        /// <summary>Get certification by ID from Welco.</summary>
        [HttpGet("certifications/{id:guid}")]
        public async Task<IActionResult> GetCertificationById([FromRoute] Guid id, CancellationToken ct)
            => ToActionResult(await _mediator.Send(new GetExternalCertificationByIdQuery { Id = id }, ct));
    }
}
