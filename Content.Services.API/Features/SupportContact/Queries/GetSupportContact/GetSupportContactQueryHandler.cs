using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using SupportContactEntity = SNUL.Shared.Domain.Models.SupportContact;

namespace Content.Services.API.Features.SupportContact.Queries.GetSupportContact
{
    public class GetSupportContactQueryHandler : IRequestHandler<GetSupportContactQuery, Result<SupportContactDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetSupportContactQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Result<SupportContactDto>> Handle(GetSupportContactQuery request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<SupportContactEntity, Guid>();
            var contact = await repo.GetAll(c => !c.IsDeleted)
                .AsNoTracking()
                .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
                .Select(ContentDtoMapper.SupportContactProjection)
                .FirstOrDefaultAsync(cancellationToken);

            // No row yet: return an empty (unconfigured) contact rather than
            // inventing phone numbers or opening hours. The row is created by
            // PUT /api/v1/support/contact, which upserts.
            if (contact == null)
            {
                return Result<SupportContactDto>.Success(new SupportContactDto
                {
                    Id = Guid.Empty,
                    SupportEmail = string.Empty,
                    PhoneNumber = string.Empty,
                    WhatsAppNumber = string.Empty,
                    WorkingHours = null
                }, LocalizationKeys.SupportContact.Fetched);
            }

            return Result<SupportContactDto>.Success(contact, LocalizationKeys.SupportContact.Fetched);
        }
    }
}
