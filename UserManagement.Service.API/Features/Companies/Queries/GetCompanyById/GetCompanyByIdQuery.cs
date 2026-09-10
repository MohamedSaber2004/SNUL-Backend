using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;
namespace UserManagement.Service.API.Features.Companies.Queries.GetCompanyById
{
    public class GetCompanyByIdQuery : IRequest<Result<CompanyDto>> { public Guid Id { get; set; } }
}
