using MediatR;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Users.Commands.ChangeUserPassword
{
    public class ChangeUserPasswordCommand : IRequest<Result<string>>
    {
        public Guid Id { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }
}
