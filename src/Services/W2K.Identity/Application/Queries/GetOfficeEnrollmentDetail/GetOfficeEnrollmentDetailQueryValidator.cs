using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetOfficeEnrollmentDetailQueryValidator : AbstractValidator<GetOfficeEnrollmentDetailQuery>
{
    public GetOfficeEnrollmentDetailQueryValidator()
    {
        _ = RuleFor(x => x.OfficeId)
            .GreaterThan(0);
    }
}
