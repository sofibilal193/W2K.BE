using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpdateOfficeStatusCommandValidator : AbstractValidator<UpdateOfficeStatusCommand>
{
    public UpdateOfficeStatusCommandValidator()
    {
        _ = RuleFor(x => x.OfficeId)
            .GreaterThan(0)
            .WithMessage("Office ID must be greater than 0.");
    }
}
