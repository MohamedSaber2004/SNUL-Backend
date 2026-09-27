using MediatR;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.HelpSiteStats.Commands.DeleteHelpSiteStat
{
    public class DeleteHelpSiteStatCommand : IRequest<Result<string>>
    {
        public Guid Id { get; set; }
    }
}
