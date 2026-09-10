using MediatR;
using SNUL.Shared.Enums;

namespace Attachment.Services.API.Features.Attachments.Commands.UploadFile
{
    public class UploadFileCommand : IRequest<string>
    {
        public IFormFile File { get; set; } = null!;
        public int Place { get; set; }
        public MediaType FileType { get; set; }
    }
}
