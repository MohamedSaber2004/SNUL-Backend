using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Cities.Queries.GetCityById
{
    public class GetCityByIdQuery : IRequest<Result<CityDto>>
    {
        public Guid Id { get; set; }
    }
}
