using MediatR;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Carts.Commands.ClearCart
{
    public class ClearCartCommand : IRequest<Result<string>>
    {
        public Guid CartId { get; set; }
    }
}
