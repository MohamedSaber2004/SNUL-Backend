using MediatR;
using SNUL.Shared.Common.DTOs.Auth.Requests;
using SNUL.Shared.Common.DTOs.Auth.Responses;
using SNUL.Shared.Enums;
using SNUL.Shared.Results;

namespace Auth.Services.API.Features.Auth.Commands.UpdateProfile
{
    public class UpdateProfileCommand : IRequest<Result<UserProfileDto>>
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureName { get; set; }
        public AppLanguage? Language { get; set; }
        public IList<UpdateProfileAddressDto>? Addresses { get; set; }
    }
}
