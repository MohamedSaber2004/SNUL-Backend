using MediatR;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.FAQs.Commands.DeleteFAQ
{
    public class DeleteFAQCommand : IRequest<Result<string>>
    {
        public Guid Id { get; set; }
    }
}
