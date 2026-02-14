#pragma warning disable CA1724 // Type names should not match namespaces
using W2K.Common.Entities;
using W2K.Common.Events;
using W2K.Common.Identity;
using W2K.Identity.Enums;

namespace W2K.Identity.Entities;

public class Role : BaseEntity
{
    private readonly List<Permission> _permissions;

    public RoleType Type { get; private set; }

    public OfficeType OfficeType { get; private set; }

    public int? OfficeId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Department { get; private set; }

    public string? Description { get; private set; }

    public virtual IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    public Role(
        RoleType type,
        OfficeType officeType,
        int? officeId,
        string name,
        string? department,
        string? description,
        ICollection<Permission>? permissions) : this()
    {
        Type = type;
        OfficeType = officeType;
        Name = name;
        Department = department;
        Description = description;
        OfficeId = officeId;
        _permissions = permissions is null ? [] : [.. permissions];
        AddDomainEvent(new EntityCreatedDomainEvent<Role>(this, OfficeId));
    }

    protected Role()
    {
        _permissions = [];
    }

    public bool AddPermissions(IEnumerable<Permission> permissions)
    {
        bool saveChanges = false;
        var missingPermissions = permissions.Where(x => Permissions?.Any(drp => drp.Name == x.Name) == false).ToList();
        if (missingPermissions.Count > 0)
        {
            saveChanges = true;
            _permissions?.AddRange(missingPermissions);
        }
        return saveChanges;
    }

    public void Update(string name, string? description, ICollection<Permission> permissions)
    {
        Name = name;
        Description = description;
        _ = (_permissions?.RemoveAll(x => !permissions.Contains(x)));
        _permissions?.AddRange(permissions.Where(x => !_permissions.Contains(x)));
        AddDomainEvent(new EntityUpdatedDomainEvent<Role>(this, OfficeId));
    }

    public void ToggleStatus(bool isDisabled)
    {
        if (isDisabled)
        {
            Disable();
        }
        else
        {
            Enable();
        }
        AddDomainEvent(new EntityUpdatedDomainEvent<Role>(this, OfficeId));
    }
}
#pragma warning restore CA1724 // Type names should not match namespaces
