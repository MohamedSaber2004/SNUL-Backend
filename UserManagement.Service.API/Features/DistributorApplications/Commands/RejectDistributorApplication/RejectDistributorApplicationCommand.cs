using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.DistributorApplications.Commands.RejectDistributorApplication
{
    public class RejectDistributorApplicationCommand : IRequest<Result<DistributorApplicationDto>>
    {
        public Guid Id { get; set; }
        public string? Reason { get; set; }
    }
}
