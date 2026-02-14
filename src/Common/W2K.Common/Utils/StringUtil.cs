using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using DFI.Common.Constants;

namespace DFI.Common.Utils;

public static partial class StringUtil
{
    public static string? DigitsOnly(this string? s)
    {
        return string.IsNullOrEmpty(s) ? null : Regex.Replace(s, RegularExpressions.NumbersOnly, "", RegexOptions.NonBacktracking).Trim();
    }

    public static int? ToNullableInt(this string s)
    {
        return int.TryParse(s.DigitsOnly(), out int i) ? i : null;
    }

    public static bool IsNullOrWhitespace(this string? s)
    {
        return string.IsNullOrWhiteSpace(s);
    }

    public static bool ToBool(this string s)
    {
        return bool.TryParse(s, out bool b) && b;
    }

    public static ReadOnlyCollection<string>? SplitCsv(this string? csvList, bool nullOrWhitespaceInputReturnsNull = false)
    {
        if (csvList.IsNullOrWhitespace())
        {
#pragma warning disable IDE0046 // Convert to conditional expression
            if (nullOrWhitespaceInputReturnsNull)
            {
                return null;
            }
            else
            {
                return new ReadOnlyCollection<string>([]);
            }
#pragma warning restore IDE0046 // Convert to conditional expression
        }
        return new ReadOnlyCollection<string>(
            [.. csvList!.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)]
        );
    }

    // Suppress Sonar warning: S3358 - Ternary operators should not be nested
#pragma warning disable S3358

    public static ReadOnlyCollection<string>? SplitToList(this string list, char seperator, bool nullOrWhitespaceInputReturnsNull = false)
    {
        return list.IsNullOrWhitespace()
            ? nullOrWhitespaceInputReturnsNull ? null : new ReadOnlyCollection<string>(Array.Empty<string>())
            : new ReadOnlyCollection<string>([.. list.Split(seperator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)]);
    }

#pragma warning restore S3358

    /// <summary>
    /// Returns a masked phone number in the format (XXX) XXX-1234 where only the last 4 real digits show.
    /// If fewer than 4 digits exist, those positions become 'X'.
    /// Returns null if input is null/whitespace.
    /// </summary>
    public static string? MaskPhoneNumber(this string? phoneNumber)
    {
        return string.IsNullOrWhiteSpace(phoneNumber) ? null : string.Concat("(XXX) XXX-", phoneNumber.Substring(phoneNumber.Length - 4, 4));
    }

    /// <summary>
    /// Returns the input string with only alphanumeric characters (letters and digits).
    /// Returns an empty string if input is null/whitespace.
    /// </summary>
    public static string ToAlphaNumericOnly(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }
        else
        {
            return new string(input.Where(char.IsLetterOrDigit).ToArray());
        }
    }

    public static string? ToName(this string? s, int maxLen = 25)
    {
        if (string.IsNullOrEmpty(s))
        {
            return null;
        }
        return NameRegex().Replace(s, string.Empty).Trim(maxLen);
    }

    public static string? Trim(this string? s, int maxLen)
    {
        if (string.IsNullOrEmpty(s))
        {
            return null;
        }
        var val = s.Trim();
        return val[..(val.Length > maxLen ? maxLen : val.Length)];
    }

    public static string? ToWords(this string? s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return null;
        }
        var words = WordsRegex().Matches(s)
            .Cast<Match>()
            .Select(x => x.Value);
        return string.Join(" ", words);
    }

    [GeneratedRegex(@"[^ 'A-Za-z-\.]*")]
    private static partial Regex NameRegex();

    [GeneratedRegex("([A-Z][a-z]+)")]
    private static partial Regex WordsRegex();
}
