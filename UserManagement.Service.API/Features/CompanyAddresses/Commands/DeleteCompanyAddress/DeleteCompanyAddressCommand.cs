using MediatR;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.CompanyAddresses.Commands.DeleteCompanyAddress
{
    public class DeleteCompanyAddressCommand : IRequest<Result<string>>
    {
        public Guid Id { get; set; }
    }
}
