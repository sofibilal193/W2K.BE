using W2K.Common.Application.Extensions;
using W2K.Identity.Repositories;
using W2K.Common.Identity;
using FluentValidation;

#pragma warning disable IDE0130
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130

public class DeleteOfficeUserCommandValidator : AbstractValidator<DeleteOfficeUserCommand>
{
    public DeleteOfficeUserCommandValidator(IIdentityUnitOfWork data, ICurrentUser currentUser)
    {
        // UserId always required
        _ = RuleFor(x => x.UserId)
            .GreaterThan(0);

        if (currentUser.OfficeType != OfficeType.SuperAdmin)
        {
            // OfficeId required for non-superadmin
            _ = RuleFor(x => x.OfficeId)
                .NotNull()
                .GreaterThan(0);

            _ = RuleFor(x => x)
                .MustBeAssociatedWithOffice(
                    getUserId: x => x.UserId,
                    getOfficeId: x => x.OfficeId!.Value,
                    associationExistsAsync: async (userId, officeId, cancel) =>
                    await data.OfficeUsers.AnyAsync(
                        ou => ou.UserId == userId && ou.OfficeId == officeId,
                        cancel))
                .When(x => x.OfficeId.HasValue); // Only validate association when OfficeId is provided
        }
    }
}
