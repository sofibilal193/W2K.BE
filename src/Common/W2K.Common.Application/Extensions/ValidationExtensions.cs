using System.Security.Cryptography;
using DFI.Common.Application.Validations;
using DFI.Common.Constants;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace DFI.Common.Application.Extensions;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, string?> PhoneNumber<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularExpressions.PhoneNumber).WithErrorCode(ValidationCodes.PhoneNumberValidator);
    }

    public static IRuleBuilderOptions<T, string?> ZipCode<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularExpressions.ZipCode).WithErrorCode(ValidationCodes.ZipCodeValidator);
    }

    public static IRuleBuilderOptions<T, string?> EmailAddress<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularExpressions.Email).WithErrorCode(ValidationCodes.EmailAddressValidator);
    }

    public static IRuleBuilderOptions<T, string?> Url<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularExpressions.Url).WithErrorCode(ValidationCodes.UrlValidator);
    }

    public static IRuleBuilderOptions<T, string?> SocalSecurityNumber<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularExpressions.SocialSecurityNumber).WithErrorCode(ValidationCodes.SocialSecurityValidator);
    }

    public static IRuleBuilderOptions<T, string?> FederalTaxId<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularExpressions.FederalTaxId).WithErrorCode(ValidationCodes.FederalTaxIdValidator);
    }

    public static IRuleBuilderOptions<T, string?> PersonName<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularExpressions.PersonName).WithErrorCode(ValidationCodes.PersonNameValidator);
    }

    public static IRuleBuilderOptions<T, string?> NumericOnly<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularExpressions.Numeric).WithErrorCode(ValidationCodes.NumericValidator);
    }

    public static IRuleBuilderOptions<T, string?> DateOfBirth<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularExpressions.DateOfBirth).WithErrorCode(ValidationCodes.DateOfBirthValidator);
    }

    public static IRuleBuilderOptions<T, string?> ValidRoutingNumber<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(x => x.ValidRoutingNumber()).WithErrorCode(ValidationCodes.ValidRoutingNumberValidator);
    }

    public static IRuleBuilderOptions<T, string?> AccountNumber<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularExpressions.AccountNumber).WithErrorCode(ValidationCodes.AccountNumberValidator);
    }

    public static IRuleBuilderOptions<T, byte[]?> FileMaxSize<T>(this IRuleBuilder<T, byte[]?> ruleBuilder)
    {
        return ruleBuilder.Must(x => x.ValidFileSize()).WithErrorCode(ValidationCodes.FileMaxSize);
    }

    public static IRuleBuilderOptions<T, string?> EmailMustBeUniqueInDatabase<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        Func<string, CancellationToken, Task<bool>> emailExistsAsync)
    {
        return ruleBuilder.MustAsync(
                async (email, cancel) =>
                {
                    return !await emailExistsAsync(email!, cancel);
                })
            .WithErrorCode(ValidationCodes.UserAlreadyExistsValidator);
    }

    public static IRuleBuilderOptions<T, IEnumerable<TFile>> MustHaveMatchingDocuments<T, TFile>(
        this IRuleBuilder<T, IEnumerable<TFile>> ruleBuilder,
        Func<T, IEnumerable<IFormFile>> getDocuments,
        Func<TFile, string> getLabel)
    {
        return ruleBuilder.Must(
                (command, files) =>
                files.HasMatchingDocuments(command, getDocuments, getLabel))
            .WithErrorCode(ValidationCodes.MissingMatchingDocumentValidator);
    }

    public static IRuleBuilderOptions<T, string?> ValidGuid<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(x => x is null || Guid.TryParse(x, out _))
            .WithErrorCode(ValidationCodes.GuidValidator);
    }

    public static IRuleBuilderOptions<T, string> RsaPublicKey<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(BeValidRsaPublicKey).WithErrorCode(ValidationCodes.RsaPublicKey);
    }

    public static IRuleBuilderOptions<T, string?> OtpCode<T>(this IRuleBuilder<T, string?> ruleBuilder, int length)
    {
        return ruleBuilder.Matches(RegularExpressions.ValidateOtpCode(length)).WithErrorCode(ValidationCodes.OtpCodeValidator);
    }

    public static IRuleBuilderOptions<T, DateOnly?> NotInFuture<T>(this IRuleBuilder<T, DateOnly?> ruleBuilder)
    {
        return ruleBuilder.Must(BeNotInFuture).WithErrorCode(ValidationCodes.DateNotInFuture);
    }

    public static IRuleBuilderOptions<T, T> DateRangeMustBeValid<T>(
        this IRuleBuilder<T, T> ruleBuilder,
        Func<T, DateOnly?> getStartDate,
        Func<T, DateOnly?> getEndDate,
        string startDatePropertyName = "StartDateUtc")
    {
        return ruleBuilder
            .Must(x => IsValidDateRange(getStartDate(x), getEndDate(x)))
            .WithErrorCode(ValidationCodes.InvalidDateRange)
            .OverridePropertyName(startDatePropertyName);
    }

    public static IRuleBuilderOptions<T, string?> MustBeBase64Encoded<T>(this IRuleBuilder<T, string?> ruleBuilder, int? maxLength = null)
    {
        return ruleBuilder
            .Must(BeValidBase64)
            .WithErrorCode(ValidationCodes.Base64Validator)
            .Must(x => BeValidBase64WithMaxDecodedLength(x, maxLength))
            .WithMessage($"{{PropertyName}} must not exceed {maxLength} characters.");
    }

    public static IRuleBuilderOptions<T, T> MustBeAssociatedWithOffice<T>(
        this IRuleBuilder<T, T> ruleBuilder,
        Func<T, int> getUserId,
        Func<T, int> getOfficeId,
        Func<int, int, CancellationToken, Task<bool>> associationExistsAsync)
    {
        return ruleBuilder
            .MustAsync(
                async (command, cancel) =>
                {
                    var userId = getUserId(command);
                    var officeId = getOfficeId(command);
                    return await associationExistsAsync(userId, officeId, cancel);
                })
            .WithErrorCode(ValidationCodes.UserNotAssociatedWithOffice);
    }

    private static bool BeValidRsaPublicKey(string base64Key)
    {
        if (string.IsNullOrWhiteSpace(base64Key))
        {
            return false;
        }
        try
        {
            var keyBytes = Convert.FromBase64String(base64Key);
            using var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(keyBytes, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool BeValidBase64(string? base64String)
    {
        if (string.IsNullOrWhiteSpace(base64String))
        {
            return false;
        }

        var requiredLength = ((base64String.Length * 3) + 3) / 4;
        Span<byte> buffer = stackalloc byte[requiredLength];
        return Convert.TryFromBase64Chars(base64String.AsSpan(), buffer, out _);
    }

    private static bool BeValidBase64WithMaxDecodedLength(string? base64String, int? maxDecodedLength)
    {
        if (maxDecodedLength is null)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(base64String))
        {
            return false;
        }

        try
        {
            var decodedBytes = Convert.FromBase64String(base64String);
            var decodedString = System.Text.Encoding.UTF8.GetString(decodedBytes);
            return decodedString.Length <= maxDecodedLength.Value;
        }
        catch
        {
            return false;
        }
    }

    private static bool BeNotInFuture(DateOnly? date)
    {
        if (!date.HasValue)
        {
            return true;
        }

        return date.Value <= DateOnly.FromDateTime(DateTime.UtcNow);
    }

    private static bool IsValidDateRange(DateOnly? startDate, DateOnly? endDate)
    {
        if (!startDate.HasValue || !endDate.HasValue)
        {
            return true;
        }

        return startDate.Value <= endDate.Value;
    }
}
