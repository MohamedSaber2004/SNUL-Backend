using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.LandingPages.Queries.GetLandingPageBySlug
{
    public class GetLandingPageBySlugQuery : IRequest<Result<LandingPageDto>>
    {
        public string Slug { get; set; } = string.Empty;
    }
}
