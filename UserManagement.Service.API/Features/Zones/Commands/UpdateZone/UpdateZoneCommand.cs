using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Zones.Commands.UpdateZone
{
    public class UpdateZoneCommand : IRequest<Result<ZoneDto>>
    {
        public Guid Id { get; set; }
        public Guid? CityId { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public bool? IsActive { get; set; }
    }
}
