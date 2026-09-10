using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.CompanyAddresses.Queries.GetCompanyAddressById
{
    public class GetCompanyAddressByIdQuery : IRequest<Result<CompanyAddressDto>>
    {
        public Guid Id { get; set; }
    }
}
