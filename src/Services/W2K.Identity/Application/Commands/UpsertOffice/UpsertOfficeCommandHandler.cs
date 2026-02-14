using W2K.Common.Exceptions;
using W2K.Common.Identity;
using W2K.Common.ValueObjects;
using W2K.Identity.Application.Notifications;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpsertOfficeCommandHandler(IIdentityUnitOfWork data, IMediator mediator, ICurrentUser currentUser, ILogger<UpsertOfficeCommandHandler> logger) : IRequestHandler<UpsertOfficeCommand, int?>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMediator _mediator = mediator;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ILogger<UpsertOfficeCommandHandler> _logger = logger;

    public async Task<int?> Handle(UpsertOfficeCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(command.Address);

        if (command.OfficeId is null)
        {
            return await AddOfficeAsync(command, cancellationToken);
        }
        else
        {
            await UpdateOfficeAsync(command, cancellationToken);
            return command.OfficeId;
        }
    }

    private static Address GetAddress(UpsertOfficeCommand command)
    {
        // build address from command
        return new Address(new Address.AddressInfo
        {
            Type = IdentityConstants.OfficePrimaryAddressType,
            Address1 = command.Address.Address1,
            Address2 = command.Address.Address2,
            City = command.Address.City,
            County = command.Address.County,
            State = command.Address.State,
            ZipCode = command.Address.ZipCode,
            Country = command.Address.Country,
            TimeZone = command.Address.TimeZone,
            GooglePlaceId = command.Address.GooglePlaceId
        });
    }

    private static Office.OfficeInfo GetOfficeInfo(UpsertOfficeCommand command)
    {
        return new Office.OfficeInfo
        {
            Tenant = command.Tenant,
            Category = command.Category,
            Code = command.Code,
            Name = command.Name,
            LegalName = command.LegalName,
            Phone = command.Phone,
            Fax = command.Fax,
            Website = command.Website,
            LicenseNo = command.LicenseNo,
            LicenseState = command.LicenseState,
            OriginalLicenseDate = command.OriginalLicenseDate,
            YearsCurrentOwnership = command.YearsCurrentOwnership,
            TaxId = command.TaxId,
            BusinessType = command.BusinessType,
            AnnualRevenue = command.AnnualRevenue,
            PromoCode = command.PromoCode,
            HowHeard = command.HowHeard,
            UtmSource = command.UtmSource,
            UtmMedium = command.UtmMedium,
            UtmCampaign = command.UtmCampaign,
            Address = GetAddress(command)
        };
    }

    private async Task<int> AddOfficeAsync(UpsertOfficeCommand command, CancellationToken cancel)
    {
        var office = new Office(GetOfficeInfo(command));
        _data.Offices.Add(office);

        var role = await _data.Roles.FirstOrDefaultAsync(x => x.Name == IdentityConstants.AdministratorRoleName, cancel)
            ?? throw new DomainException("Admin role not found");

        if (command.IsFirstTimeEnroll && command.AdminUser is not null)
        {
            var adminUserCommand = new AdminUserCommand
            {
                FirstName = command.AdminUser.FirstName,
                LastName = command.AdminUser.LastName,
                Email = command.AdminUser.Email,
                MobilePhone = command.AdminUser.MobilePhone,
            };

            var adminUserId = await _mediator.Send(adminUserCommand, cancel);

            if (adminUserId > 0)
            {
                office.UpsertUser(adminUserId, role.Id, command.AdminUser.Title, isDefault: true);
            }
        }

        _ = await _data.SaveEntitiesAsync(cancel);

        // Publish notification after office is created
        var notification = new IdentityEventLogNotification(
            "Office Created",
            _currentUser.Source,
            $"Office Name: {office.Name}.",
            _currentUser.UserId,
            office.Id);
        await _mediator.Publish(notification, cancel);

        return office.Id;
    }

    private async Task UpdateOfficeAsync(UpsertOfficeCommand command, CancellationToken cancel)
    {
        if (command.OfficeId is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var office = await _data.Offices.Include(x => x.Addresses).FirstOrDefaultAsync(x => x.Id == command.OfficeId, cancel)
            ?? throw new NotFoundException(nameof(Office), command.OfficeId.Value);

        office.Update(GetOfficeInfo(command));

        // Update enrollment and review status (SuperAdmin only)
        if (_currentUser.OfficeType == OfficeType.SuperAdmin)
        {
            UpdateSuperAdminFields(command, office);
        }

        _data.Offices.Update(office);
        _ = await _data.SaveEntitiesAsync(cancel);

        // Publish notification after office is created
        var notification = new IdentityEventLogNotification(
            "Office Updated",
            _currentUser.Source,
            $"Office Name: {office.Name}.",
            _currentUser.UserId,
            office.Id);
        await _mediator.Publish(notification, cancel);
    }

    private void UpdateSuperAdminFields(UpsertOfficeCommand command, Office office)
    {
        UpdateFieldIfProvided(
            command.IsEnrollmentCompleted,
            office.SetEnrollment,
            "enrollment status",
            office.Id);

        UpdateFieldIfProvided(
            command.IsReviewed,
            office.SetReviewed,
            "review status",
            office.Id);

        UpdateFieldIfProvided(
            command.IsActive,
            office.SetActiveStatus,
            "active status",
            office.Id);
    }

    private void UpdateFieldIfProvided(bool? value, Action<bool> updateAction, string fieldName, int officeId)
    {
        if (value.HasValue)
        {
            updateAction(value.Value);
            _logger.LogInformation(
                "SuperAdmin {UserId} updated {FieldName} to {Status} for Office {OfficeId}",
                _currentUser.UserId,
                fieldName,
                value.Value,
                officeId);
        }
    }
}
