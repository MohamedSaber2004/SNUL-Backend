using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.CompanyAddresses.Commands.UpdateCompanyAddress
{
    public class UpdateCompanyAddressCommand : IRequest<Result<CompanyAddressDto>>
    {
        public Guid Id { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? CityId { get; set; }
        public Guid? ZoneId { get; set; }
        public string? Street { get; set; }
        public string? Building { get; set; }
        public string? Floor { get; set; }
        public string? Apartment { get; set; }
        public bool? IsDefault { get; set; }
    }
}
