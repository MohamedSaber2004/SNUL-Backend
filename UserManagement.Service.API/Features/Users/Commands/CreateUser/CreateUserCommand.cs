using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Enums;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<Result<UserDto>>
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = string.Empty;
        public UserType UserType { get; set; } = UserType.OrganizationUser;
        public Guid? CompanyId { get; set; }
        public string? ProfilePictureName { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
