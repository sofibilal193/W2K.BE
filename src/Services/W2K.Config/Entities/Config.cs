#pragma warning disable CA1724 // Type names should not match namespaces
using W2K.Common.Entities;

namespace W2K.Config.Entities;

public class Config : BaseEntity
{
    #region Public Properties

    /// <summary>
    /// The type of config.
    /// </summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// The name of config.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The description of config.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// The value of config.
    /// </summary>
    public string? Value { get; private set; }

    /// <summary>
    /// The display order of config.
    /// </summary>
    public short DisplayOrder { get; private set; }

    /// <summary>
    /// indicates whether this config is private and should not be returned to the F/E for regular office users
    /// this may be returned to the F/E for super admin clients depending on the requirements
    /// </summary>
    public bool IsInternal { get; init; }

    /// <summary>
    /// indicates if the value is encrypted in the database
    /// this is used to determine if the value should be decrypted before being returned to the API caller
    /// </summary>
    public bool IsEncrypted { get; init; }

    #endregion

    #region Constructors

    public Config()
    {
        // Parameterless constructor for constructor chaining
    }

    public Config(ConfigItemInfo info) : this()
    {
        Type = info.Type ?? string.Empty;
        Name = info.Name ?? string.Empty;
        Description = info.Description;
        Value = info.Value;
        DisplayOrder = info.DisplayOrder;
        IsInternal = info.IsInternal;
        IsEncrypted = info.IsEncrypted;
    }

    #endregion

    #region Public Methods

    public void Update(string? value, string? description, short displayOrder)
    {
        Value = value;
        Description = description;
        DisplayOrder = displayOrder;
    }

    #endregion

    public readonly record struct ConfigItemInfo(
        string? Type,
        string? Name,
        string? Description = null,
        string? Value = null,
        short DisplayOrder = 0,
        bool IsInternal = false,
        bool IsEncrypted = false
    );
}
#pragma warning restore CA1724 // Type names should not match namespaces
