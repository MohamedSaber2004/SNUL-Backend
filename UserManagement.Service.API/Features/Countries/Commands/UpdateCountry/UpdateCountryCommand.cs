using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Countries.Commands.UpdateCountry
{
    public class UpdateCountryCommand : IRequest<Result<CountryDto>>
    {
        public Guid Id { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public string? Code { get; set; }
        public string? PhoneCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
