using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.FAQs.Commands.CreateFAQ
{
    public class CreateFAQCommand : IRequest<Result<FAQItemDto>>
    {
        public string Question { get; set; } = string.Empty;
        public string? QuestionAr { get; set; }
        public string Answer { get; set; } = string.Empty;
        public string? AnswerAr { get; set; }
        public int SortOrder { get; set; }
    }
}
