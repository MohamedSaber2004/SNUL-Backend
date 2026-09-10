using MediatR;
using SNUL.Shared.Common.DTOs.Commerce;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Carts.Queries.GetCartBySession
{
    public class GetCartBySessionQuery : IRequest<Result<CartDto>>
    {
        public string SessionId { get; set; } = string.Empty;
    }
}
