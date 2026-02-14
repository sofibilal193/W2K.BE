using W2K.Common.Application.Extensions;
using W2K.Identity.Repositories;
using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class ActivateUserCommandValidator : AbstractValidator<ActivateUserCommand>
{
    public ActivateUserCommandValidator(IIdentityUnitOfWork data)
    {
        _ = RuleFor(x => x.OfficeId)
            .GreaterThan(0)
            .When(x => x.OfficeId.HasValue);

        _ = RuleFor(x => x.UserId)
            .GreaterThan(0);

        _ = RuleFor(x => x)
            .MustBeAssociatedWithOffice(
                getUserId: x => x.UserId,
                getOfficeId: x => x.OfficeId!.Value,
                associationExistsAsync: async (userId, officeId, cancel) =>
                await data.OfficeUsers.AnyAsync(ou => ou.UserId == userId && ou.OfficeId == officeId, cancel))
            .When(x => x.OfficeId.HasValue);
    }
}
