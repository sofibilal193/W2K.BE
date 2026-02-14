using FluentValidation;
using W2K.Common.Application.Extensions;
using W2K.Common.Application.Commands;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpsertOfficeCommandValidator : AbstractValidator<UpsertOfficeCommand>
{
    public UpsertOfficeCommandValidator()
    {
        _ = RuleFor(x => x.Tenant)
            .MaximumLength(25);

        _ = RuleFor(x => x.Category)
            .NotEmpty()
            .MaximumLength(50);

        _ = RuleFor(x => x.Code)
            .MaximumLength(25);

        _ = RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        _ = RuleFor(x => x.LegalName)
            .NotEmpty()
            .MaximumLength(100);

        _ = RuleFor(x => x.Phone)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .PhoneNumber();

        _ = RuleFor(x => x.Fax)
            .PhoneNumber();

        _ = RuleFor(x => x.Website)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Url()
            .MaximumLength(255);

        _ = RuleFor(x => x.LicenseNo)
            .NotEmpty()
            .MaximumLength(50);

        _ = RuleFor(x => x.LicenseState)
            .NotEmpty()
            .MaximumLength(50);

        _ = RuleFor(x => x.TaxId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .FederalTaxId();

        _ = RuleFor(x => x.BusinessType)
            .NotEmpty()
            .MaximumLength(50);

        _ = RuleFor(x => x.PromoCode)
            .MaximumLength(25);

        _ = RuleFor(x => x.HowHeard)
            .MaximumLength(50);

        _ = RuleFor(x => x.UtmSource)
            .MaximumLength(50);

        _ = RuleFor(x => x.UtmMedium)
            .MaximumLength(50);

        _ = RuleFor(x => x.UtmCampaign)
            .MaximumLength(50);

        _ = RuleFor(x => x.YearsCurrentOwnership)
            .NotEmpty()
            .InclusiveBetween((short)0, (short)100);

        _ = RuleFor(x => x.AnnualRevenue)
            .NotEmpty();

        _ = RuleFor(x => x.Address)
            .NotNull()
            .SetValidator(new AddressCommandValidator());

        _ = RuleFor(x => x.AdminUser)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .SetValidator(new AdminUserCommandValidator())
            .When(x => x.IsFirstTimeEnroll);
    }
}
