using System.Collections.ObjectModel;
using W2K.Common.Entities;
using W2K.Common.Events;
using W2K.Common.Identity;
using W2K.Common.ValueObjects;

namespace W2K.Identity.Entities;

public class Office : BaseEntity
{
    #region Private Fields

    private readonly List<Address> _addresses;
    private readonly List<OfficeUser> _users;
    private readonly List<OfficeOwner> _owners;
    private readonly List<OfficeBankAccount> _bankAccounts;
    private readonly List<OfficeGroup> _groups;

    #endregion

    public OfficeType Type { get; private set; } = OfficeType.Merchant;

    public string? Tenant { get; private set; }

    public string Category { get; private set; } = string.Empty;

    public string? Code { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string LegalName { get; private set; } = string.Empty;

    public string Phone { get; private set; } = string.Empty;

    public string? Fax { get; private set; }

    public string? Website { get; private set; }

    public string? LicenseNo { get; private set; }

    public string? LicenseState { get; private set; }

    public DateOnly? OriginalLicenseDate { get; private set; }

    public short? YearsCurrentOwnership { get; private set; }

    public string TaxId { get; private set; } = string.Empty;

    public string BusinessType { get; private set; } = string.Empty;

    public int? AnnualRevenue { get; private set; }

    public string? PromoCode { get; private set; }

    public string? HowHeard { get; private set; }

    public bool IsEnrollmentCompleted { get; private set; }

    public bool IsReviewed { get; private set; }

    public bool IsApproved { get; private set; }

    public string? UtmSource { get; private set; }

    public string? UtmMedium { get; private set; }

    public string? UtmCampaign { get; private set; }

    public DateTime? LastLoginDateTimeUtc { get; private set; }

    #region Owned Entities

    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    public virtual IReadOnlyCollection<OfficeUser> Users => _users.AsReadOnly();

    public IReadOnlyCollection<OfficeOwner> Owners => _owners.AsReadOnly();

    public IReadOnlyCollection<OfficeBankAccount> BankAccounts => _bankAccounts.AsReadOnly();

    public IReadOnlyCollection<OfficeGroup> Groups => _groups.AsReadOnly();

    #endregion

    public Office(OfficeInfo info)
        : this()
    {
        Type = info.Type ?? OfficeType.Merchant;
        Tenant = info.Tenant;
        Category = info.Category ?? string.Empty;
        Code = info.Code;
        Name = info.Name ?? string.Empty;
        LegalName = info.LegalName ?? string.Empty;
        Phone = info.Phone ?? string.Empty;
        Fax = info.Fax;
        Website = info.Website;
        LicenseNo = info.LicenseNo;
        LicenseState = info.LicenseState;
        OriginalLicenseDate = info.OriginalLicenseDate;
        YearsCurrentOwnership = info.YearsCurrentOwnership;
        TaxId = info.TaxId ?? string.Empty;
        BusinessType = info.BusinessType ?? string.Empty;
        AnnualRevenue = info.AnnualRevenue;
        PromoCode = info.PromoCode;
        HowHeard = info.HowHeard;
        UtmSource = info.UtmSource;
        UtmMedium = info.UtmMedium;
        UtmCampaign = info.UtmCampaign;
        if (info.Address is not null)
        {
            _addresses.Add(info.Address);
        }

        AddDomainEvent(new EntityCreatedDomainEvent<Office>(this));
    }

    protected Office()
    {
        _addresses = [];
        _users = [];
        _owners = [];
        _bankAccounts = [];
        _groups = [];
    }

    public void Update(OfficeInfo info)
    {
        Tenant = info.Tenant ?? Tenant;
        Category = info.Category ?? Category;
        Code = info.Code ?? Code;
        Name = info.Name ?? Name;
        LegalName = info.LegalName ?? LegalName;
        Phone = info.Phone ?? Phone;
        Fax = info.Fax ?? Fax;
        Website = info.Website;
        LicenseNo = info.LicenseNo;
        LicenseState = info.LicenseState;
        OriginalLicenseDate = info.OriginalLicenseDate ?? OriginalLicenseDate;
        YearsCurrentOwnership = info.YearsCurrentOwnership;
        TaxId = info.TaxId ?? TaxId;
        BusinessType = info.BusinessType ?? BusinessType;
        AnnualRevenue = info.AnnualRevenue;
        PromoCode = info.PromoCode ?? PromoCode;
        HowHeard = info.HowHeard ?? HowHeard;
        UtmSource = info.UtmSource ?? UtmSource;
        UtmMedium = info.UtmMedium ?? UtmMedium;
        UtmCampaign = info.UtmCampaign ?? UtmCampaign;

        if (info.Address is not null)
        {
            UpdateAddress(info.Address);
        }

        AddDomainEvent(new EntityUpdatedDomainEvent<Office>(this));
    }

    public void UpdateAddress(Address? address)
    {
        ArgumentNullException.ThrowIfNull(address);

        if (_addresses.Any(x => x.Type == address.Type))
        {
            // Update existing address
            var existingAddress = _addresses.First(x => x.Type == address.Type);
            existingAddress.Update(address);
        }
        else
        {
            // Add new address
            _addresses.Add(address);
        }

        AddDomainEvent(new EntityUpdatedDomainEvent<Office>(this));
    }

    public void SetEnrollment(bool isEnrollmentCompleted)
    {
        IsEnrollmentCompleted = isEnrollmentCompleted;
        AddDomainEvent(new EntityUpdatedDomainEvent<Office>(this));
    }

    public void SetReviewApproved(bool isReviewed, bool isApproved)
    {
        IsReviewed = isReviewed;
        IsApproved = isApproved;
        AddDomainEvent(new EntityUpdatedDomainEvent<Office>(this));
    }

    public void SetReviewed(bool isReviewed)
    {
        IsReviewed = isReviewed;
        AddDomainEvent(new EntityUpdatedDomainEvent<Office>(this));
    }

    public void SetApproved(bool isApproved)
    {
        IsApproved = isApproved;
        AddDomainEvent(new EntityUpdatedDomainEvent<Office>(this));
    }

    public void SetActiveStatus(bool isActive)
    {
        if (isActive)
        {
            Enable();
        }
        else
        {
            Disable();
        }
        AddDomainEvent(new EntityUpdatedDomainEvent<Office>(this));
    }

    public void AddGroup(int parentOfficeId)
    {
        if (parentOfficeId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(parentOfficeId), "Parent office ID must be greater than zero.");
        }

        var group = new OfficeGroup(parentOfficeId);
        _groups.Add(group);
    }

    public void RemoveGroup(int parentOfficeId)
    {
        if (parentOfficeId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(parentOfficeId), "Parent office ID must be greater than zero.");
        }

        var group = _groups.FirstOrDefault(x => x.ParentOfficeId == parentOfficeId);
        if (group is not null)
        {
            _ = _groups.Remove(group);
        }
    }

    public void UpsertUser(int userId, int? roleId, string? title = null, bool isDefault = false)
    {
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero.");
        }

        var user = _users.FirstOrDefault(x => x.UserId == userId);
        if (user is not null)
        {
            // Update existing office
            user.Update(isDefault, roleId, title);
            return;
        }

        user = new OfficeUser(this, userId, isDefault, roleId, title);
        user.Invite();
        _users.Add(user);

        AddDomainEvent(new EntityUpdatedDomainEvent<Office>(this));
    }

    public void RemoveUser(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero.");
        }

        var user = _users.FirstOrDefault(x => x.UserId == userId);
        if (user is not null)
        {
            _ = _users.Remove(user);
        }

        AddDomainEvent(new EntityUpdatedDomainEvent<Office>(this));
    }

    public void DisableUser(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero.");
        }

        var user = _users.FirstOrDefault(x => x.UserId == userId);
        user?.Disable();

        AddDomainEvent(new EntityUpdatedDomainEvent<Office>(this));
    }

    public void Login()
    {
        LastLoginDateTimeUtc = DateTime.UtcNow;
    }

    public void AddUsers(ReadOnlyCollection<OfficeUser> users)
    {
        _users.Clear();
        _users.AddRange(users);
    }

    public void UpsertOwner(ReadOnlyCollection<OfficeOwner> owners)
    {
        foreach (var owner in owners)
        {
            var officeOwner = _owners.FirstOrDefault(x => x.SSN == owner.SSN && x.DOB == owner.DOB);

            if (officeOwner is null)
            {
                _owners.Add(owner);
            }
            else
            {
                officeOwner.UpdateInfo(owner);
            }
        }

        var ownersToRemove = _owners.Where(x => !owners.Any(y => y.SSN == x.SSN && y.DOB == x.DOB)).ToList();

        if (ownersToRemove.Count != 0)
        {
            RemoveOwners(ownersToRemove.AsReadOnly());
        }
        AddDomainEvent(new EntityUpdatedDomainEvent<Office>(this));
    }

    public void RemoveOwners(ReadOnlyCollection<OfficeOwner> owners)
    {
        _ = _owners.RemoveAll(owners.Contains);
    }

    public void UpsertBankAccount(OfficeBankAccount bankAccount)
    {
        // Find the existing primary account
        var existingPrimaryAccount = _bankAccounts.FirstOrDefault(x => x.IsPrimary);

        if (existingPrimaryAccount is null)
        {
            // Add the new primary account
            _bankAccounts.Add(bankAccount);
        }
        else
        {
            // Update the existing primary account
            existingPrimaryAccount.UpdateInfo(bankAccount);
        }

        AddDomainEvent(new EntityUpdatedDomainEvent<Office>(this));
    }

    public readonly record struct OfficeInfo(
        OfficeType? Type,
        string? Tenant,
        string? Category,
        string? Code,
        string? Name,
        string? LegalName,
        string? Phone,
        string? Fax,
        string? Website,
        string? LicenseNo,
        string? LicenseState,
        DateOnly? OriginalLicenseDate,
        short? YearsCurrentOwnership,
        string? TaxId,
        string? BusinessType,
        int? AnnualRevenue,
        string? PromoCode,
        string? HowHeard,
        string? UtmSource,
        string? UtmMedium,
        string? UtmCampaign,
        Address? Address
    );

}
