using MediatR;
using SNUL.Shared.Common.DTOs.Commerce;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Carts.Commands.CreateCart
{
    public class CreateCartCommand : IRequest<Result<CartDto>>
    {
        public Guid? UserId { get; set; }
        public string? SessionId { get; set; }
        public Guid? CurrencyId { get; set; }
    }
}
