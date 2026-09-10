using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.SupportContact.Commands.UpdateSupportContact
{
    public class UpdateSupportContactCommand : IRequest<Result<SupportContactDto>>
    {
        public string SupportEmail { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string WhatsAppNumber { get; set; } = string.Empty;
        public string? WorkingHours { get; set; }
    }
}
