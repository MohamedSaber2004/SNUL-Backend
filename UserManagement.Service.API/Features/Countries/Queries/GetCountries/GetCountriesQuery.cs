using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Countries.Queries.GetCountries
{
    public class GetCountriesQuery : IRequest<Result<IReadOnlyList<CountryDto>>>
    {
    }
}
