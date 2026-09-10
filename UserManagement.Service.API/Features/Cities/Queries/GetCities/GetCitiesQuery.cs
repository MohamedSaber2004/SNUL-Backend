using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Cities.Queries.GetCities
{
    public class GetCitiesQuery : IRequest<Result<IReadOnlyList<CityDto>>>
    {
        public Guid? CountryId { get; set; }
    }
}
