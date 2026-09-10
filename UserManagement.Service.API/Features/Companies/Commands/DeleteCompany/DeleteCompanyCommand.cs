using MediatR;
using SNUL.Shared.Results;
namespace UserManagement.Service.API.Features.Companies.Commands.DeleteCompany
{
    public class DeleteCompanyCommand : IRequest<Result<string>> { public Guid Id { get; set; } }
}
