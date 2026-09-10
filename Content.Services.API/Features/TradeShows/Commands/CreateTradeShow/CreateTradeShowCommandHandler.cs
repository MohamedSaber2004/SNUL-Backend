using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Persistance;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.TradeShows.Commands.CreateTradeShow
{
    public class CreateTradeShowCommandHandler : IRequestHandler<CreateTradeShowCommand, Result<TradeShowEventDto>>
    {
        private readonly SnulDbContext _db;
        public CreateTradeShowCommandHandler(SnulDbContext db) => _db = db;

        public async Task<Result<TradeShowEventDto>> Handle(CreateTradeShowCommand request, CancellationToken ct)
        {
            var entity = new TradeShowEvent
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Location = request.Location.Trim(),
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };
            entity.MarkAsCreated("System");
            _db.TradeShowEvents.Add(entity);
            await _db.SaveChangesAsync(ct);
            var dto = new TradeShowEventDto { Id = entity.Id, Name = entity.Name, Location = entity.Location, StartDate = entity.StartDate, EndDate = entity.EndDate, CreatedAt = entity.CreatedAt };
            return Result<TradeShowEventDto>.Success(dto, LocalizationKeys.TradeShow.Created, 201);
        }
    }
}
