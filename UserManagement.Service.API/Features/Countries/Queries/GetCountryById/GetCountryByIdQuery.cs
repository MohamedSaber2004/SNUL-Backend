using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Countries.Queries.GetCountryById
{
    public class GetCountryByIdQuery : IRequest<Result<CountryDto>>
    {
        public Guid Id { get; set; }
    }
}
