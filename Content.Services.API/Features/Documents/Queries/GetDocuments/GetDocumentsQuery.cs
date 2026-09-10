using MediatR;
using SNUL.Shared.Results;
using SNUL.Shared.Common.DTOs.Content;
namespace Content.Services.API.Features.Documents.Queries.GetDocuments
{
    public class GetDocumentsQuery : IRequest<PaginatedResult<DocumentDto>> { public int PageNumber { get; set; } = 1; public int PageSize { get; set; } = 10; }
}
