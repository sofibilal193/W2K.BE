namespace W2K.Common.Entities;

public abstract class BaseEntityLogProps
{
    /// <summary>
    /// Date record was created in UTC time.
    /// </summary>
    public DateTime? CreateDateTimeUtc { get; private set; }

    /// <summary>
    /// Id of user that created the record.
    /// </summary>
    public int? CreateUserId { get; private set; }

    /// <summary>
    /// Name of user that created the record.
    /// </summary>
    public string? CreateUserName { get; private set; }

    /// <summary>
    /// Ip address of user that created the record.
    /// </summary>
    public string? CreateSource { get; private set; }

    /// <summary>
    /// Date record was modified in UTC time.
    /// </summary>
    public DateTime? ModifyDateTimeUtc { get; private set; }

    /// <summary>
    /// Id of user that modified the record.
    /// </summary>
    public int? ModifyUserId { get; private set; }

    /// <summary>
    /// Name of user that modified the record.
    /// </summary>
    public string? ModifyUserName { get; private set; }

    /// <summary>
    /// Ip address of user that modified the record.
    /// </summary>
    public string? ModifySource { get; private set; }

    #region Public Methods

    public void Set(int? userId, string? userName, string? source)
    {
        CreateDateTimeUtc ??= DateTime.UtcNow;
        CreateUserId ??= userId;
        CreateUserName ??= userName;
        CreateSource ??= source;
        ModifyDateTimeUtc = DateTime.UtcNow;
        ModifyUserId = userId;
        ModifyUserName = userName;
        ModifySource = source;
    }

    #endregion

    #region Protected Methods

    protected void SetSource(string? source)
    {
        if (!string.IsNullOrEmpty(source))
        {
            CreateSource ??= source;
            ModifySource = source;
        }
    }

    #endregion
}
