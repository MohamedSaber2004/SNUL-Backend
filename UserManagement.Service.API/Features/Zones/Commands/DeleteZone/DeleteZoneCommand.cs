using MediatR;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Zones.Commands.DeleteZone
{
    public class DeleteZoneCommand : IRequest<Result<string>>
    {
        public Guid Id { get; set; }
    }
}
