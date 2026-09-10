using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Zones.Queries.GetZones
{
    public class GetZonesQuery : IRequest<Result<IReadOnlyList<ZoneDto>>>
    {
        public Guid? CityId { get; set; }
    }
}
