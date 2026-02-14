using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class SearchOfficesQueryHandlerValidator : AbstractValidator<SearchOfficesQuery>
{
    public SearchOfficesQueryHandlerValidator()
    {
        _ = RuleFor(x => x.SearchTerm)
            .NotEmpty()
            .MaximumLength(50);
    }
}
