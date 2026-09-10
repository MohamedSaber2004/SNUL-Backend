using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.Addresses.Queries.GetAddressById
{
    public class GetAddressByIdQueryValidator : AbstractValidator<GetAddressByIdQuery>
    {
        public GetAddressByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.UserAddress.AddressIdRequired);
        }
    }
}
