using MediatR;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest<Result<string>>
    {
        public Guid Id { get; set; }
    }
}
