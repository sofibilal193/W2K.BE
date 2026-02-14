using FluentValidation;
using W2K.Common.Application.Extensions;

namespace W2K.Common.Application.Commands.Messaging;

public class UpsertMessageCommandValidator : AbstractValidator<UpsertMessageCommand>
{
    public UpsertMessageCommandValidator()
    {

        _ = RuleFor(x => x.Content)
            .NotEmpty()
            .MustBeBase64Encoded();

        _ = RuleFor(x => x.OfficeId)
            .GreaterThan(0);

        // If ThreadId is provided, we are adding to an existing thread or editing a message
        When(x => x.ThreadId.HasValue, () =>
            {
                _ = RuleFor(x => x.ThreadId)
                    .GreaterThan(0);
            });

        // If MessageId is provided, we are editing an existing message
        When(x => x.MessageId.HasValue, () =>
            {
                _ = RuleFor(x => x.MessageId)
                    .NotNull();
            });

        // If EntityType is provided, EntityId is required
        When(x => x.EntityType.HasValue, () =>
            {
                _ = RuleFor(x => x.EntityType)
                    .IsInEnum();

                _ = RuleFor(x => x.EntityId)
                    .NotEmpty()
                    .GreaterThan(0)
                    .WithMessage("'{PropertyName}' is required when EntityType is provided.");
            });
    }
}
