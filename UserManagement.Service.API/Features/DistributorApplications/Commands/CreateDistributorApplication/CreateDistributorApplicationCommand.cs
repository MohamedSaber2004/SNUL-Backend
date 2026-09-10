using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.DistributorApplications.Commands.CreateDistributorApplication
{
    public class CreateDistributorApplicationCommand : IRequest<Result<DistributorApplicationDto>>
    {
        public string CompanyName { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
        public string? SalesVolumeBand { get; set; }
        public string? CategoryInterest { get; set; }
        public string? Website { get; set; }
        public string ContactPerson { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }
}
