using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetUserOfficesQueryValidator : AbstractValidator<GetUserOfficesQuery>
{
    public GetUserOfficesQueryValidator()
    {
        _ = RuleFor(x => x.SortBy)
            .IsInEnum();
    }
}
