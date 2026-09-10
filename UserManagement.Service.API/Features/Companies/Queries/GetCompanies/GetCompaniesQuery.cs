using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Companies.Queries.GetCompanies
{
    public class GetCompaniesQuery : IRequest<PaginatedResult<CompanyDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsProvider { get; set; }
        public SNUL.Shared.Enums.CompanyType? Type { get; set; }
    }
}
