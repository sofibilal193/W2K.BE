using FluentValidation;
using W2K.Common.Application.Extensions;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Files.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpsertOfficeFilesCommandValidator : AbstractValidator<UpsertOfficeFilesCommand>
{
    public UpsertOfficeFilesCommandValidator()
    {
        _ = RuleFor(x => x.Documents)
            .NotEmpty();

        _ = RuleFor(x => x.Files)
            .MustHaveMatchingDocuments(
                x => x.Documents ?? [],
                x => x.Label)
            .When(x => x.Documents is not null && x.Documents.Count > 0);

        _ = RuleForEach(x => x.Files).ChildRules(x =>
            {
                _ = x.RuleFor(x => x.Content)
                    .NotNull()
                    .FileMaxSize();
            }).When(x =>
            {
                // Ensure the "Each file label must have a corresponding uploaded document" validation passes
                return x.Files.All(file =>
                    x.Documents?.Any(doc => doc.FileName == file.Label) == true);
            });
    }
}
