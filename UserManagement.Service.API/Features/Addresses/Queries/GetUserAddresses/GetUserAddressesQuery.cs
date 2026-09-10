using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Addresses.Queries.GetUserAddresses
{
    public class GetUserAddressesQuery : IRequest<Result<IReadOnlyList<UserAddressDto>>>
    {
        public Guid UserId { get; set; }
    }
}
