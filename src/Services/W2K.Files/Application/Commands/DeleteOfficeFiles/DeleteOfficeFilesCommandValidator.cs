using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Files.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class DeleteOfficeFilesCommandValidator : AbstractValidator<DeleteOfficeFilesCommand>
{
    public DeleteOfficeFilesCommandValidator()
    {
        _ = RuleFor(x => x.OfficeId)
            .GreaterThan(0)
            .WithMessage("OfficeId must be greater than 0.");

        _ = RuleFor(x => x.FileId)
            .GreaterThan(0)
            .WithMessage("FileId must be greater than 0.");
    }
}
