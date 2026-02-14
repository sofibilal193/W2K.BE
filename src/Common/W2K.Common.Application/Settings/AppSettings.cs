namespace DFI.Common.Application.Settings;

public record AppSettings
{
    #region CORS Settings

    public string AllowedCorsOrigins { get; init; } = string.Empty;

    #endregion

    #region Auth Settings

    public AuthSettings AuthSettings { get; init; }

    #endregion

    #region General Settings

    public int RegexTimeoutSeconds { get; init; } = 3;

    public long? MaxRequestFileSizeInBytes { get; init; }

    #endregion

    #region Api Versioning Settings

    public ApiVersionOptions ApiVersionSettings { get; init; }

    #endregion

    #region OpenApi Settings

    public OpenApiOptions OpenApiSettings { get; init; }

    #endregion
}
