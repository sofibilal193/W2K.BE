using W2K.Common.Entities;
using W2K.Common.Events;
using W2K.Identity.Application.Events;
using W2K.Identity.Enums;

namespace W2K.Identity.Entities;

public class OfficeUser : BaseEntity
{
    public int OfficeId { get; private set; }

    public virtual Office? Office { get; init; }

    public int UserId { get; init; }

    public virtual User? User { get; init; }

    public int? RoleId { get; private set; }

    public virtual Role? Role { get; init; }

    public string? Title { get; private set; }

    public bool IsDefault { get; private set; }

    public bool IsInvited { get; private set; }

    public bool IsInviteProcessed { get; private set; }

    public UserDepartment Department { get; init; } = UserDepartment.Other;

    public bool IsManager { get; init; }

    public DateTime? InviteAcceptedDateTimeUtc { get; private set; }

    public OfficeUser(
        User user,
        int officeId,
        bool isDefault,
        int? roleId,
        string? title) : this()
    {
        User = user;
        OfficeId = officeId;
        RoleId = roleId;
        IsDefault = isDefault;
        Title = title;

        AddDomainEvent(new EntityCreatedDomainEvent<OfficeUser>(this));
    }

    public OfficeUser(
        int userId,
        int roleId,
        int officeId,
        string? title) : this()
    {
        UserId = userId;
        RoleId = roleId;
        OfficeId = officeId;
        Title = title;

        AddDomainEvent(new EntityCreatedDomainEvent<OfficeUser>(this));
    }

    public OfficeUser(
        Office office,
        int userId,
        bool isDefault,
        int? roleId,
        string? title) : this()
    {
        Office = office;
        UserId = userId;
        RoleId = roleId;
        IsDefault = isDefault;
        Title = title;

        AddDomainEvent(new EntityCreatedDomainEvent<OfficeUser>(this));
    }

    protected OfficeUser()
    {
    }

    public void Update(bool isDefault, int? roleId, string? title)
    {
        IsDefault = isDefault;
        RoleId = roleId;
        Title = title;

        AddDomainEvent(new EntityUpdatedDomainEvent<OfficeUser>(this));
    }

    public void Update(int officeId, int? roleId, bool isDisabled)
    {
        OfficeId = officeId;
        RoleId = roleId;
        if (isDisabled)
        {
            Disable();
        }
        else
        {
            Enable();
        }

        AddDomainEvent(new EntityUpdatedDomainEvent<OfficeUser>(this));
    }

    public void Invite()
    {
        IsInvited = true;
    }

    public void ProcessInvite()
    {
        if (IsInvited && !IsInviteProcessed)
        {
            IsInviteProcessed = true;
            InviteAcceptedDateTimeUtc = DateTime.UtcNow;
        }
    }

    public void SetIsDefault()
    {
        IsDefault = true;
        AddDomainEvent(new EntityUpdatedDomainEvent<OfficeUser>(this));
    }

    public void ClearIsDefault()
    {
        IsDefault = false;
        AddDomainEvent(new EntityUpdatedDomainEvent<OfficeUser>(this));
    }

    public void Delete()
    {
        AddDomainEvent(new EntityDeletedDomainEvent<OfficeUser>(this));
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
        AddDomainEvent(new EntityUpdatedDomainEvent<OfficeUser>(this));
    }
}
