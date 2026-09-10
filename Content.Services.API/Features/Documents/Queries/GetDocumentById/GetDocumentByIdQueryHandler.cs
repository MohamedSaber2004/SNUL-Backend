using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using DocEntity = SNUL.Shared.Domain.Models.Document;

namespace Content.Services.API.Features.Documents.Queries.GetDocumentById
{
    public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, Result<DocumentDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetDocumentByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<DocumentDto>> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<DocEntity, Guid>();
            var dto = await repo.GetAll(d => !d.IsDeleted && d.Id == request.Id)
                .Select(ContentDtoMapper.DocumentProjection)
                .FirstOrDefaultAsync(cancellationToken);

            if (dto == null)
                return Result<DocumentDto>.NotFound(LocalizationKeys.Document.NotFound);

            return Result<DocumentDto>.Success(dto, LocalizationKeys.Document.Fetched);
        }
    }
}
