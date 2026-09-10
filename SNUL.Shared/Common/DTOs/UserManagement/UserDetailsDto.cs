using SNUL.Shared.Enums;

namespace SNUL.Shared.Common.DTOs.UserManagement
{
    public class UserDetailsDto : UserDto
    {
        public IReadOnlyList<UserAddressDto> Addresses { get; set; } = new List<UserAddressDto>();
    }
}
