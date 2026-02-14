using FluentValidation;

namespace W2K.Common.Application.Queries.GlobalSearch;

/// <summary>
/// Validator for <see cref="GlobalSearchQuery"/>.
/// </summary>
public class GlobalSearchQueryValidator : AbstractValidator<GlobalSearchQuery>
{
    public GlobalSearchQueryValidator()
    {
        _ = RuleFor(x => x.SearchTerm)
            .NotEmpty()
            .MaximumLength(100);
    }
}
