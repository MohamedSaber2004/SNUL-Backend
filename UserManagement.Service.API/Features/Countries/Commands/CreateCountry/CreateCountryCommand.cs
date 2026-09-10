using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Countries.Commands.CreateCountry
{
    public class CreateCountryCommand : IRequest<Result<CountryDto>>
    {
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? PhoneCode { get; set; }
    }
}
