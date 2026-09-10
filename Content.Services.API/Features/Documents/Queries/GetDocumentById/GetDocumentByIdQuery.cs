using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.Documents.Queries.GetDocumentById
{
    public class GetDocumentByIdQuery : IRequest<Result<DocumentDto>>
    {
        public Guid Id { get; set; }
    }
}
