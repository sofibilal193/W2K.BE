using W2K.Common.Entities;

namespace W2K.Config.Entities;

public class OfficeConfigFieldValue : BaseEntity
{
    #region Private Fields

    #endregion

    #region Public Properties

    /// <summary>
    /// The Id of the OfficeConfigField
    /// </summary>
    public int FieldId { get; init; }

    /// <summary>
    /// The Id of the OfficeConfigField
    /// </summary>
    public virtual OfficeConfigField? Field { get; init; }

    /// <summary>
    /// The Id of the Office
    /// </summary>
    public int OfficeId { get; init; }

    /// <summary>
    /// The value of the field.
    /// </summary>
    public string? Value { get; init; }

    #endregion

    #region Constructors

    protected OfficeConfigFieldValue()
    {
    }

    #endregion

    #region Public Methods

    #endregion
}
