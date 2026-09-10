using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.FAQs.Queries.GetFAQs
{
    public class GetFAQsQuery : IRequest<Result<List<FAQItemDto>>>
    {
    }
}
