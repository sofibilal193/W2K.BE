#pragma warning disable CA1724 // Type names should not match namespaces
using W2K.Common.Entities;
using W2K.Common.Events;
using W2K.Identity.Application.Events;

namespace W2K.Identity.Entities;

public class User : BaseEntity
{
    #region Private Fields

    private readonly List<OfficeUser> _offices;

    #endregion

    public string? ProviderId { get; private set; }

    public string? Role { get; init; }

    public string? FirstName { get; private set; }

    public string? LastName { get; private set; }

    public string Email { get; private set; } = string.Empty;

    public string? MobilePhone { get; private set; }

    public DateTime? LastLoginDateTimeUtc { get; private set; }

    public DateTime? LastLogoutDateTimeUtc { get; private set; }

    public string FullName => string.Concat(FirstName, " ", LastName);

    #region Collections

    public virtual IReadOnlyCollection<OfficeUser> Offices => _offices.AsReadOnly();

    #endregion

    #region Public Properties

    public OfficeUser? DefaultOffice => _offices.Find(x => !x.IsDisabled && x.IsDefault);

    public int? DefaultOfficeId => DefaultOffice?.OfficeId;

    #endregion

    #region Constructors

    public User(
        string? providerId,
        string? firstName,
        string? lastName,
        string email,
        string? mobilePhone,
        string? lastLoginIpAddress)
        : this()
    {
        ProviderId = providerId;
        FirstName = firstName;
        LastName = lastName;
        Email = email.ToLowerInvariant();
        MobilePhone = mobilePhone;
        SetSource(lastLoginIpAddress);
        AddDomainEvent(new EntityCreatedDomainEvent<User>(this));
    }

    protected User()
    {
        _offices = [];
    }

    #endregion

    #region Public Methods

    public void UpdateLastLoginSource(string? lastLoginIpAddress)
    {
        SetSource(lastLoginIpAddress);
    }

    public void Login(string? lastLoginIpAddress)
    {
        LastLoginDateTimeUtc = DateTime.UtcNow;
        SetSource(lastLoginIpAddress);
        AddDomainEvent(new UserLoggedInDomainEvent(Id, FullName, ModifySource));
    }

    public void Logout()
    {
        LastLogoutDateTimeUtc = DateTime.UtcNow;
        AddDomainEvent(new UserLoggedOutDomainEvent(Id, FullName, ModifySource));
    }

    public override void Disable()
    {
        base.Disable();
        AddDomainEvent(new EntityDisabledDomainEvent<User>(this));
    }

    public void ProcessUserInvitation(string providerId, string? lastLoginIpAddress)
    {
        ProviderId = providerId;
        SetSource(lastLoginIpAddress);
        LastLoginDateTimeUtc = DateTime.UtcNow;

        // set invites to processed
        foreach (var office in _offices.Where(x => x.IsInvited && !x.IsInviteProcessed))
        {
            office.ProcessInvite();
        }
    }

    public void UpdateMobilePhone(string mobilePhone, string? lastLoginIpAddress)
    {
        MobilePhone = mobilePhone;
        SetSource(lastLoginIpAddress);
        AddDomainEvent(new EntityUpdatedDomainEvent<User>(this));
    }

    public void UpdateName(string firstName, string lastName, string? lastLoginIpAddress)
    {
        FirstName = firstName;
        LastName = lastName;
        SetSource(lastLoginIpAddress);

        AddDomainEvent(new EntityUpdatedDomainEvent<User>(this));
    }

    public void PwdChanged(string? userName, string? lastLoginIpAddress)
    {
        SetSource(lastLoginIpAddress);
        AddDomainEvent(new UserPwdChangedDomainEvent(Id, userName ?? FullName));
    }

    public void UpsertOffice(int officeId, int? roleId, string? title = null, bool isDefault = false)
    {
        if (officeId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(officeId), "Office ID must be greater than zero.");
        }

        var office = _offices.FirstOrDefault(x => x.OfficeId == officeId);
        if (office is not null)
        {
            // Update existing office
            office.Update(isDefault, roleId, title);
            office.Invite();
            AddDomainEvent(new EntityUpdatedDomainEvent<User>(this));
            return;
        }

        office = new OfficeUser(this, officeId, isDefault, roleId, title);
        office.Invite();
        _offices.Add(office);
        AddDomainEvent(new EntityUpdatedDomainEvent<User>(this));
    }

    public void RemoveOffice(int officeId)
    {
        if (officeId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(officeId), "Office ID must be greater than zero.");
        }

        var office = _offices.FirstOrDefault(x => x.OfficeId == officeId);
        if (office is not null)
        {
            _ = _offices.Remove(office);
            AddDomainEvent(new EntityDeletedDomainEvent<User>(this));
        }
    }

    #endregion
}
#pragma warning restore CA1724 // Type names should not match namespaces
