using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Extensions;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using DocEntity = SNUL.Shared.Domain.Models.Document;
namespace Content.Services.API.Features.Documents.Queries.GetDocuments
{
    public class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, PaginatedResult<DocumentDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetDocumentsQueryHandler(IUnitOfWork uow) => _uow = uow;
        public async Task<PaginatedResult<DocumentDto>> Handle(GetDocumentsQuery r, CancellationToken ct)
        {
            var repo = _uow.GetRepository<DocEntity, Guid>();
            var q = repo.GetAll(x => !x.IsDeleted).AsNoTracking();
            return await q.OrderByDescending(x => x.CreatedAt).ToPaginatedListAsync(x => new DocumentDto { Id = x.Id, Title = x.Title, DocType = x.DocType, FileUrl = x.FileUrl, FileSizeKB = x.FileSizeKB, ProductId = x.ProductId, PublishedDate = x.PublishedDate, CreatedAt = x.CreatedAt }, r.PageNumber, r.PageSize, LocalizationKeys.Document.ListFetched, ct);
        }
    }
}
