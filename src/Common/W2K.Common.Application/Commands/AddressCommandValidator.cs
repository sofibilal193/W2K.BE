using FluentValidation;
using W2K.Common.Application.Extensions;

namespace W2K.Common.Application.Commands;

public class AddressCommandValidator : AbstractValidator<AddressCommand>
{
    public AddressCommandValidator()
    {
        _ = RuleFor(x => x.Type)
            .NotEmpty()
            .MaximumLength(25);

        _ = RuleFor(x => x.Address1)
            .NotEmpty()
            .MaximumLength(100);

        _ = RuleFor(x => x.Address2)
            .MaximumLength(100);

        _ = RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(50);

        _ = RuleFor(x => x.County)
            .MaximumLength(50);

        _ = RuleFor(x => x.State)
            .NotEmpty()
            .MaximumLength(50);

        _ = RuleFor(x => x.ZipCode)
            .NotEmpty()
            .MaximumLength(20)
            .ZipCode();

        _ = RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(50);

        _ = RuleFor(x => x.TimeZone)
            .MaximumLength(50);

        _ = RuleFor(x => x.GooglePlaceId)
            .MaximumLength(100);
    }
}
