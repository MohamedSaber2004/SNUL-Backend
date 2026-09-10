using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;
namespace Content.Services.API.Features.OemInquiries.Queries.GetOemInquiryById
{
    public class GetOemInquiryByIdQuery : IRequest<Result<OemInquiryDto>> { public Guid Id { get; set; } }
}
