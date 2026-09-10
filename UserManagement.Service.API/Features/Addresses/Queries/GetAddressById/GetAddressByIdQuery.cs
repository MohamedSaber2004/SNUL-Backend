using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Addresses.Queries.GetAddressById
{
    public class GetAddressByIdQuery : IRequest<Result<UserAddressDto>>
    {
        public Guid Id { get; set; }
    }
}
