using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Companies.Queries.GetMyCompany
{
    public class GetMyCompanyQuery : IRequest<Result<CompanyDto>>
    {
    }
}
