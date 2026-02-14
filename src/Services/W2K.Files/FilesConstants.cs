namespace W2K.Files;

public static class FilesConstants
{
    /// <summary>
    /// Maximum file size in bytes (10 MB).
    /// </summary>
    public const long MaxFileSizeBytes = 10 * 1024 * 1024;

    public static readonly string ApplicationName = "W2K.Files";

    #region File Container Names
    public static readonly string OfficesContainerName = "offices";

    #endregion

    #region Directory Names
    public static readonly string MessagingDirectoryName = "messaging";

    public static readonly string LoanAppsDirectoryName = "loanapps";

    public static readonly string LoansDirectoryName = "loans";

    #endregion

    #region File Tags Names
    public static readonly string MessageThreadId_FileTagName = "MessageThreadId";

    public static readonly string LoanAppId_FileTagName = "LoanAppId";

    public static readonly string LoanId_FileTagName = "LoanId";
    #endregion


    /// <summary>
    /// Allowed content types for file uploads.
    /// </summary>
    public static readonly string[] AllowedContentTypes =
    [
        "application/pdf",
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/heic",
        "image/heif",
        "image/tiff",
        "image/x-tiff"
    ];
}
