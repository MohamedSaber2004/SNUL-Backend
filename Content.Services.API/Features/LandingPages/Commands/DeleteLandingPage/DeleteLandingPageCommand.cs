using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.LandingPages.Commands.DeleteLandingPage
{
    public class DeleteLandingPageCommand : IRequest<Result<string>>
    {
        public Guid Id { get; set; }
    }
}
