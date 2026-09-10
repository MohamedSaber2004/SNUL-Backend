using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Cities.Commands.UpdateCity
{
    public class UpdateCityCommand : IRequest<Result<CityDto>>
    {
        public Guid Id { get; set; }
        public Guid? CountryId { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public bool? IsActive { get; set; }
    }
}
