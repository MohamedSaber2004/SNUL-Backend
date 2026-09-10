using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<Result<UserDetailsDto>>
    {
        public Guid Id { get; set; }
    }
}
