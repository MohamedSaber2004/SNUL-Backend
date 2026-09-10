using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.CompanyAddresses.Queries.GetCompanyAddresses
{
    public class GetCompanyAddressesQuery : IRequest<Result<IReadOnlyList<CompanyAddressDto>>>
    {
        public Guid CompanyId { get; set; }
    }
}
