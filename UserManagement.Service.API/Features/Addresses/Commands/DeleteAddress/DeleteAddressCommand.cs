using MediatR;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Addresses.Commands.DeleteAddress
{
    public class DeleteAddressCommand : IRequest<Result<string>>
    {
        public Guid Id { get; set; }
    }
}
