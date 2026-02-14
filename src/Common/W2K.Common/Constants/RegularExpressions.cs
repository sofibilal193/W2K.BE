#pragma warning disable CA1724 // Type names should not match namespaces

namespace DFI.Common.Constants;

/// <summary>
/// Common regular expression strings.
/// </summary>
public static class RegularExpressions
{
    /// <summary>
    /// Matches strings with numeric characters only.
    /// </summary>
    public const string NumbersOnly = "[^0-9]";

    /// <summary>
    /// Matches strings with format: (999) 999-9999.
    /// </summary>
    public static readonly string PhoneNumber = @"^\(\d{3}\) \d{3}-\d{4}$";

    /// <summary>
    /// Matches strings with format: 99999 or 99999-9999.
    /// </summary>
    public static readonly string ZipCode = @"^\d{5}(-\d{4})?$";

    /// <summary>
    /// Matches strings containing a validly formatted email address.
    /// </summary>
    public static readonly string Email = @"^[\w\.\-\+]{1,}@{1}[\w\-]{1,}(\.{1}[\w\-]{1,}){1,}$";

    /// <summary>
    /// Matches strings containing a validly formatted web URL.
    /// </summary>
    public static readonly string Url = @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)";

    /// <summary>
    /// Matches strings with format: 999-99-9999.
    /// </summary>
    public static readonly string SocialSecurityNumber = @"^\d{3}-\d{2}-\d{4}$";

    /// <summary>
    /// Matches strings with format: 99-9999999.
    /// </summary>
    public static readonly string FederalTaxId = @"^\d{2}-\d{7}$";

    /// <summary>
    /// Matches strings that only contain valid characters that should be used in personal names
    /// </summary>
    public static readonly string PersonName = @"^[A-Za-z\u00C0-\u00FF][A-Za-z\u00C0-\u00FF'\-]+([\ A-Za-z\u00C0-\u00FF][A-Za-z\u00C0-\u00FF'\-]+)*$";

    /// <summary>
    /// Expression to check only alphabetic characters.
    /// </summary>
    public static readonly string AlphabeticCharacters = @"^[A-Za-z\s]*$";

    /// Matches strings containing upper-case letters, lower-case letters, numbers, underscore, and/or hyphen.
    /// </summary>
    public static readonly string AlphaNumericExtended = "^[a-zA-Z0-9_-]*$";

    /// <summary>
    /// Matches strings containing numbers only.
    /// </summary>
    public static readonly string Numeric = "^[0-9]*$";

    /// <summary>
    /// Matches strings containing numbers only.
    /// </summary>
    public static readonly string SanitizeJson = "[\"\'<>]*";

    /// <summary>
    /// Matches strings with format: yyyy-MM-dd.
    /// </summary>
    public static readonly string DateOfBirth = "^\\d{4}-\\d{2}-\\d{2}$";

    /// <summary>
    /// Matches strings with valid account number format.
    /// </summary>
    public static readonly string AccountNumber = "^[a-zA-Z0-9]+$";

    /// <summary>
    /// Matches numbers with exactly <paramref name="length"/> digits.
    /// </summary>
    public static string ValidateOtpCode(int length)
    {
        return $"^\\d{{{length}}}$";
    }
}

#pragma warning restore CA1724 // Type names should not match namespaces
