using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.AuditLogs.Queries.GetAuditLogs.GetAuditLogs
{
    public class GetAuditLogsQueryValidator : AbstractValidator<GetAuditLogsQuery>
    {
        public GetAuditLogsQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage(LocalizationKeys.AuditLog.PageNumberPositive);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage(LocalizationKeys.AuditLog.PageSizeRange);
        }
    }
}
