using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Zones.Queries.GetZoneById
{
    public class GetZoneByIdQuery : IRequest<Result<ZoneDto>>
    {
        public Guid Id { get; set; }
    }
}
