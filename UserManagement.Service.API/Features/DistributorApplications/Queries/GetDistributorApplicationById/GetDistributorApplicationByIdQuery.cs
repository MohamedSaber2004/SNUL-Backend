using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.DistributorApplications.Queries.GetDistributorApplicationById
{
    public class GetDistributorApplicationByIdQuery : IRequest<Result<DistributorApplicationDto>>
    {
        public Guid Id { get; set; }

        public GetDistributorApplicationByIdQuery() { }

        public GetDistributorApplicationByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
