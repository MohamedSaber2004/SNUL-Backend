using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.DistributorApplications.Queries.GetDistributorApplications
{
    public class GetDistributorApplicationsQuery : IRequest<PaginatedResult<DistributorApplicationDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public DistributorApplicationStatus? Status { get; set; }
    }
}
