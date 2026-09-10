using MediatR;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Cities.Commands.DeleteCity
{
    public class DeleteCityCommand : IRequest<Result<string>>
    {
        public Guid Id { get; set; }
    }
}
