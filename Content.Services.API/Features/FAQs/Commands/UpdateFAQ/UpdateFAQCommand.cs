using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.FAQs.Commands.UpdateFAQ
{
    public class UpdateFAQCommand : IRequest<Result<FAQItemDto>>
    {
        public Guid Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool? IsActive { get; set; }
    }
}
