using MediatR;
using SNUL.Shared.Common.DTOs.Auth.Responses;
using SNUL.Shared.Results;

namespace Auth.Services.API.Features.Auth.Queries.GetUserProfile
{
    public class GetUserProfileQuery : IRequest<Result<UserProfileDto>>
    {
    }
}
