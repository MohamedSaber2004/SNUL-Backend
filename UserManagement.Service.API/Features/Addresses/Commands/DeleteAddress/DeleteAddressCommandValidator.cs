using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.Addresses.Commands.DeleteAddress
{
    public class DeleteAddressCommandValidator : AbstractValidator<DeleteAddressCommand>
    {
        public DeleteAddressCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.UserAddress.AddressIdRequired);
        }
    }
}
