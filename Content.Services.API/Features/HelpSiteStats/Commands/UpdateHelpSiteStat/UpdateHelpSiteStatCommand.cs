using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.HelpSiteStats.Commands.UpdateHelpSiteStat
{
    public class UpdateHelpSiteStatCommand : IRequest<Result<HelpSiteStatDto>>
    {
        public Guid Id { get; set; }

        // The key is intentionally not editable. It is the contract the frontend
        // keys off, so changing it would silently blank a slot in the hero. To move
        // a claim to a different key, create the new row and delete the old one.
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string? LabelAr { get; set; }
        public bool IsVisible { get; set; }
        public int SortOrder { get; set; }
    }
}
