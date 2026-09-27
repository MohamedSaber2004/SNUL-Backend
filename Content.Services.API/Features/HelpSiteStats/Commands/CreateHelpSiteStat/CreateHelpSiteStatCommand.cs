using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.HelpSiteStats.Commands.CreateHelpSiteStat
{
    public class CreateHelpSiteStatCommand : IRequest<Result<HelpSiteStatDto>>
    {
        public string StatKey { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string? LabelAr { get; set; }

        // Creating a row is an operator asserting the claim, so it publishes unless
        // the admin explicitly opts out. Withdrawing later is a PUT with false.
        public bool IsVisible { get; set; } = true;

        public int SortOrder { get; set; }
    }
}
