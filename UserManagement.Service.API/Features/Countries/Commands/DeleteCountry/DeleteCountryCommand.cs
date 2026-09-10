using MediatR;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Countries.Commands.DeleteCountry
{
    public class DeleteCountryCommand : IRequest<Result<string>>
    {
        public Guid Id { get; set; }
    }
}
