using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Extensions;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
namespace Content.Services.API.Features.OemInquiries.Queries.GetOemInquiries
{
    public class GetOemInquiriesQueryHandler : IRequestHandler<GetOemInquiriesQuery, PaginatedResult<OemInquiryDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetOemInquiriesQueryHandler(IUnitOfWork uow) => _uow = uow;
        public async Task<PaginatedResult<OemInquiryDto>> Handle(GetOemInquiriesQuery r, CancellationToken ct)
        {
            var repo = _uow.GetRepository<OemInquiry, Guid>();
            var q = repo.GetAll(x => !x.IsDeleted);
            if (!string.IsNullOrWhiteSpace(r.SearchTerm))
            {
                var term = r.SearchTerm.Trim().ToLower();
                q = q.Where(x => x.FullName.ToLower().Contains(term) || x.Email.ToLower().Contains(term) || x.CompanyName.ToLower().Contains(term) || x.ServiceType.ToLower().Contains(term) || x.Message.ToLower().Contains(term));
            }
            return await q.OrderByDescending(x => x.CreatedAt).ToPaginatedListAsync(x => new OemInquiryDto { Id = x.Id, FullName = x.FullName, Email = x.Email, CompanyName = x.CompanyName, ServiceType = x.ServiceType, Message = x.Message, CreatedAt = x.CreatedAt }, r.PageNumber, r.PageSize, LocalizationKeys.OemInquiry.ListFetched, ct);
        }
    }
}
