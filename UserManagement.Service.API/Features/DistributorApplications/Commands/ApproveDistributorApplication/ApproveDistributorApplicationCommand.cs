using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.DistributorApplications.Commands.ApproveDistributorApplication
{
    public class ApproveDistributorApplicationCommand : IRequest<Result<DistributorApplicationDto>>
    {
        public Guid Id { get; set; }
        public Guid? AccountManagerId { get; set; }
    }
}
