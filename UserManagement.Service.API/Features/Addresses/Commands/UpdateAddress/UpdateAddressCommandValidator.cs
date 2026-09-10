using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.Addresses.Commands.UpdateAddress
{
    public class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
    {
        public UpdateAddressCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.UserAddress.AddressIdRequired);

            RuleFor(x => x.Street)
                .MaximumLength(250)
                .When(x => !string.IsNullOrEmpty(x.Street));
        }
    }
}
