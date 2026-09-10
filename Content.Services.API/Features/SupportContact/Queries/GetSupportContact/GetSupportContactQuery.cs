using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.SupportContact.Queries.GetSupportContact
{
    public class GetSupportContactQuery : IRequest<Result<SupportContactDto>>
    {
    }
}
