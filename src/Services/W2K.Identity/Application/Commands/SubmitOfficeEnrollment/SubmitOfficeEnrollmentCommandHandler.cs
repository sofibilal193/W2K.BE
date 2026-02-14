using W2K.Common.Application.Validations;
using W2K.Common.Exceptions;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class SubmitOfficeEnrollmentCommandHandler(IIdentityUnitOfWork data, IMediator mediator) : IRequestHandler<SubmitOfficeEnrollmentCommand>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMediator _mediator = mediator;

    public async Task Handle(SubmitOfficeEnrollmentCommand command, CancellationToken cancellationToken)
    {
        var office = await _data.Offices
            .Include(x => x.Owners)
            .Include(x => x.BankAccounts)
            .Include(x => x.Addresses)
            .FirstOrDefaultAsync(x => x.Id == command.OfficeId, cancellationToken) ?? throw new NotFoundException(nameof(Office), command.OfficeId);

        if (office.Owners.Count == 0 || office.BankAccounts.Count == 0)
        {
            throw ValidationCodes.GenerateValidationException(
                ValidationCodes.OwnerAndBankAccountRequired,
                ValidationCodes.OwnerAndBankAccountRequired
            );
        }

        if (office.IsEnrollmentCompleted)
        {
            throw ValidationCodes.GenerateValidationException(
                ValidationCodes.EnrollmentAlreadyCompleted,
                ValidationCodes.EnrollmentAlreadyCompleted
           );
        }

        office.SetEnrollment(true);
        _data.Offices.Update(office);
        _ = await _data.SaveEntitiesAsync(cancellationToken);

        var primaryAddress = office.Addresses.FirstOrDefault(x => x.Type == IdentityConstants.OfficePrimaryAddressType);
        var notification = new OfficeEnrollmentSubmittedNotification(office.Id, office.Name, office.Phone, primaryAddress, true, false);
        await _mediator.Publish(notification, cancellationToken);
    }
}
