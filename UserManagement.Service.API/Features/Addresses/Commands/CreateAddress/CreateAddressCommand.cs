using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Addresses.Commands.CreateAddress
{
    public class CreateAddressCommand : IRequest<Result<UserAddressDto>>
    {
        public Guid UserId { get; set; }
        public Guid CountryId { get; set; }
        public Guid CityId { get; set; }
        public Guid ZoneId { get; set; }
        public string Street { get; set; } = string.Empty;
        public string? Building { get; set; }
        public string? Floor { get; set; }
        public string? Apartment { get; set; }
        public bool IsDefault { get; set; }
    }
}
