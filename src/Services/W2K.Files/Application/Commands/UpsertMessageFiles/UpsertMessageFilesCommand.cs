using W2K.Common.Application.DTOs.Files;
using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Files.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public readonly record struct UpsertMessageFilesCommand(int OfficeId, int ThreadId, bool IsInternal, IReadOnlyCollection<MessageFilesCommand> Files) : IRequest<MessageFileDto>;

public readonly record struct MessageFilesCommand(string FileName, string ContentType, byte[] Content);

public class UpsertMessageFilesCommandValidator : AbstractValidator<UpsertMessageFilesCommand>
{
    public UpsertMessageFilesCommandValidator()
    {
        RuleFor(x => x.OfficeId)
            .GreaterThan(0);

        RuleFor(x => x.ThreadId)
            .GreaterThan(0);

        RuleFor(x => x.Files)
            .NotEmpty();

        RuleForEach(x => x.Files)
            .ChildRules(x =>
                {
                    x.RuleFor(f => f.ContentType)
                        .NotEmpty()
                        .Must(contentType => FilesConstants.AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
                        .WithMessage($"Only {string.Join(", ", FilesConstants.AllowedContentTypes)} are allowed content types.");

                    x.RuleFor(f => f.Content)
                        .Must(content => content.Length <= FilesConstants.MaxFileSizeBytes)
                        .WithMessage($"File size cannot exceed {FilesConstants.MaxFileSizeBytes / (1024 * 1024)} MB.");
                });
    }
}
