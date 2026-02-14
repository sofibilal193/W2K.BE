#pragma warning disable CA1724 // Type names should not match namespaces
using W2K.Common.Entities;
using W2K.Identity.Enums;

namespace W2K.Identity.Entities;

public class Permission : BaseEntity
{
    private readonly List<Role> _roles;

    public string Category { get; private set; } = string.Empty;

    public PermissionType Type { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public virtual IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    public Permission(
        string category,
        PermissionType type,
        string name,
        string? description) : this()
    {
        Category = category;
        Name = name;
        Description = description;
        Type = type;
    }

    protected Permission()
    {
        _roles = [];
    }
}
#pragma warning restore CA1724 // Type names should not match namespaces
