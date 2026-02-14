using W2K.Identity.Entities;
using W2K.Identity.Repositories;
using W2K.Common.Exceptions;
using W2K.Identity.Application.Notifications;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpsertOfficeBankAccountCommandHandler(IIdentityUnitOfWork data, IMediator mediator) : IRequestHandler<UpsertOfficeBankAccountCommand>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMediator _mediator = mediator;

    public async Task Handle(UpsertOfficeBankAccountCommand command, CancellationToken cancellationToken)
    {
        var office = await _data.Offices.Include(x => x.BankAccounts).GetAsync(command.OfficeId, cancellationToken)
            ?? throw new NotFoundException(nameof(Office), command.OfficeId);

        office.UpsertBankAccount(new OfficeBankAccount(GetOfficeBankAccountInfo(command)));

        _ = await _data.SaveEntitiesAsync(cancellationToken);

        await _mediator.Publish(
            new IdentityEventLogNotification(
            "Office Bank Account Updated",
            "System",
            $"Office ID: {office.Id}, Bank Account Updated.",
            null,
            office.Id),
            cancellationToken);
    }

    private static OfficeBankAccount.OfficeBankAccountInfo GetOfficeBankAccountInfo(UpsertOfficeBankAccountCommand command)
    {
        return new OfficeBankAccount.OfficeBankAccountInfo
        {
            Type = command.AccountType,
            BankName = command.BankName,
            RoutingNumber = command.RoutingNumber,
            AccountName = command.AccountName,
            AccountNumber = command.AccountNumber,
            IsPrimary = command.IsPrimary
        };
    }
}
