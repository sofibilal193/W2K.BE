using W2K.Common.Exceptions;
using W2K.Common.Identity;
using W2K.Common.ValueObjects;
using W2K.Identity.Application.Notifications;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpsertOfficeOwnerCommandHandler(IIdentityUnitOfWork data, ICurrentUser currentUser, IMediator mediator) : IRequestHandler<UpsertOfficeOwnerCommand>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IMediator _mediator = mediator;

    public async Task Handle(UpsertOfficeOwnerCommand command, CancellationToken cancellationToken)
    {
        var office = await _data.Offices.Include(x => x.Owners).GetAsync(command.OfficeId, cancellationToken)
            ?? throw new NotFoundException(nameof(Office), command.OfficeId);

        if (office.IsDisabled && _currentUser.OfficeType != OfficeType.SuperAdmin)
        {
            throw new DomainException($"Cannot modify owners for disabled office (ID: {office.Id}). Please activate the office before adding or updating owners.");
        }

        var newOwners = command.Owners.Select(x => new OfficeOwner(GetOfficeOwnerInfo(x))).ToList();

        if (newOwners.Count != 0)
        {
            office.UpsertOwner(newOwners.AsReadOnly());
        }

        _ = await _data.SaveEntitiesAsync(cancellationToken);

        await _mediator.Publish(
            new IdentityEventLogNotification(
            "Office Owners Updated",
            _currentUser.Source,
            $"Office ID: {office.Id}, Owners Updated.",
            _currentUser.UserId,
            office.Id),
            cancellationToken);
    }

    private static OfficeOwner.OfficeOwnerInfo GetOfficeOwnerInfo(OfficeOwnerInfoCommand ownerInfo)
    {
        return new OfficeOwner.OfficeOwnerInfo
        {
            FirstName = ownerInfo.FirstName,
            LastName = ownerInfo.LastName,
            MiddleName = ownerInfo.MiddleName,
            Email = ownerInfo.Email,
            MobilePhone = ownerInfo.MobilePhone,
            DOB = ownerInfo.DOB,
            SSN = ownerInfo.SSN,
            Ownership = ownerInfo.Ownership,
            AnnualIncome = ownerInfo.AnnualIncome,
            NetWorth = ownerInfo.NetWorth,
            Address = GetAddress(ownerInfo)
        };
    }

    private static Address GetAddress(OfficeOwnerInfoCommand ownerInfo)
    {
        // build address from command
        return new Address(new Address.AddressInfo
        {
            Type = IdentityConstants.OfficePrimaryAddressType,
            Address1 = ownerInfo.Address.Address1,
            Address2 = ownerInfo.Address.Address2,
            City = ownerInfo.Address.City,
            County = ownerInfo.Address.County,
            State = ownerInfo.Address.State,
            ZipCode = ownerInfo.Address.ZipCode,
            Country = ownerInfo.Address.Country,
            TimeZone = ownerInfo.Address.TimeZone,
            GooglePlaceId = ownerInfo.Address.GooglePlaceId
        });
    }

}
