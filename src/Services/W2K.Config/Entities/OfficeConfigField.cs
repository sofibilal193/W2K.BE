using W2K.Common.Config;
using W2K.Common.Entities;
using W2K.Common.Identity;

namespace W2K.Config.Entities;

public class OfficeConfigField : BaseEntity
{
    #region Private Fields

    private readonly List<OfficeConfigFieldValue> _values;

    private readonly List<FieldValue> _fieldValues;

    #endregion

    #region Public Properties

    /// <summary>
    /// The type of office this field applies for.
    /// </summary>
    public OfficeType? OfficeType { get; init; }

    /// <summary>
    /// The category of the field.
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// The type of the field.
    /// </summary>
    public FieldType FieldType { get; init; }

    /// <summary>
    /// The name of the field.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The description of the field.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The default value of the field.
    /// </summary>
    public string? DefaultValue { get; init; }

    /// <summary>
    /// The display order of the field.
    /// </summary>
    public short DisplayOrder { get; init; }

    /// <summary>
    /// The minimum length or value
    /// </summary>
    public decimal? MinValue { get; init; }

    /// <summary>
    /// The maximum length or value
    /// </summary>
    public decimal? MaxValue { get; init; }

    /// <summary>
    /// The regular expression to use to validate the value
    /// </summary>
    public string? RegexValidator { get; init; }

    /// <summary>
    /// Indicates whether this field is visible/editable by offices themselves
    /// OR if it is only visible/editable by super admin users
    /// </summary>
    public bool IsInternal { get; init; }

    /// <summary>
    /// indicates if the value is encrypted in the database
    /// this is used to determine if the value should be decrypted before being returned to the API caller
    /// </summary>
    public bool IsEncrypted { get; init; }

    #region Has Entities

    public virtual IReadOnlyCollection<OfficeConfigFieldValue> Values => _values.AsReadOnly();

    /// <summary>
    /// contains possible drop-down values for this field
    /// this is used for fields of type DropDown, DropDownMulti, Radio, etc.
    /// </summary>
    public IReadOnlyCollection<FieldValue> FieldValues => _fieldValues.AsReadOnly();

    #endregion

    #endregion

    #region Constructors

    public OfficeConfigField(OfficeConfigFieldInfo info) : this()
    {
        OfficeType = info.OfficeType;
        Category = info.Category ?? string.Empty;
        FieldType = info.FieldType;
        Name = info.Name ?? string.Empty;
        Description = info.Description;
        DefaultValue = info.DefaultValue;
        DisplayOrder = info.DisplayOrder;
        MinValue = info.MinValue;
        MaxValue = info.MaxValue;
        RegexValidator = info.RegexValidator;
        IsInternal = info.IsInternal;
        IsEncrypted = info.IsEncrypted;
    }

    protected OfficeConfigField()
    {
        _values = [];
        _fieldValues = [];
    }

    #endregion

    #region Public Methods

    #endregion

    #region Nested Types

    public readonly record struct OfficeConfigFieldInfo(
        OfficeType? OfficeType,
        string? Category,
        FieldType FieldType,
        string? Name,
        string? Description = null,
        string? DefaultValue = null,
        short DisplayOrder = 0,
        decimal? MinValue = null,
        decimal? MaxValue = null,
        string? RegexValidator = null,
        bool IsInternal = false,
        bool IsEncrypted = false
    );

    #endregion
}
