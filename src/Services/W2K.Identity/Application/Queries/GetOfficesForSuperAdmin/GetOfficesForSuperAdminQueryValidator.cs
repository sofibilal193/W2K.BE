using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetOfficesForSuperAdminQueryValidator : AbstractValidator<GetOfficesForSuperAdminQuery>
{
    public GetOfficesForSuperAdminQueryValidator()
    {
        _ = RuleFor(x => x.SortBy)
            .IsInEnum();
    }
}
