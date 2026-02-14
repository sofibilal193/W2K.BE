using System.Reflection;
using FluentValidation;
using FluentValidation.Results;

namespace DFI.Common.Application.Validations;

public static class ValidationCodes
{
    [Message("'{PropertyName}' should be in the format (999) 999-9999.")]
    public static readonly string PhoneNumberValidator = "PhoneNumberValidator";

    [Message("'{PropertyName}' should be in the format 99999 or 99999-9999.")]
    public static readonly string ZipCodeValidator = "ZipCodeValidator";

    [Message("'{PropertyName}' is not a valid email address.")]
    public static readonly string EmailAddressValidator = "EmailAddressValidator";

    [Message("'{PropertyName}' is not a valid URL.")]
    public static readonly string UrlValidator = "UrlValidator";

    [Message("'{PropertyName}' should be in the format 999-99-9999.")]
    public static readonly string SocialSecurityValidator = "SocialSecurityValidator";

    [Message("'{PropertyName}' should be in the format 99-9999999.")]
    public static readonly string FederalTaxIdValidator = "FederalTaxIdValidator";

    [Message("'{PropertyName}' contains characters not normally used in a person's name.")]
    public static readonly string PersonNameValidator = "PersonNameValidator";

    [Message("'{PropertyName}' contains characters not normally used in a '{PropertyName}'.")]
    public static readonly string AlphabeticValidator = "AlphabeticValidator";

    [Message("'{PropertyName}' should contains numbers only.")]
    public static readonly string NumericValidator = "NumericValidator";

    [Message("'{PropertyName}' should be in the format yyyy-MM-dd.")]
    public static readonly string DateOfBirthValidator = "DateOfBirthValidator";

    [Message("Session ID HTTP header is required.")]
    public static readonly string SessionIdHeaderRequired = "SessionIdHeaderRequired";

    [Message("{PropertyName}' must be a valid Base-64 encoded RSA public key.")]
    public static readonly string RsaPublicKey = "RsaPublicKey";

    [Message("{PropertyName}' must be a valid Guid.")]
    public static readonly string GuidValidator = "GuidValidator";

    [Message("'{PropertyName}' must be a valid Base64 encoded string.")]
    public static readonly string Base64Validator = "Base64Validator";

    [Message("'{PropertyName}' must be an 8-digit numeric code.")]
    public static readonly string OtpCodeValidator = "OtpCodeValidator";

    [Message("'{PropertyName}' must not be in the future.")]
    public static readonly string DateNotInFuture = "DateNotInFuture";

    [Message("Start date must be less than or equal to end date.")]
    public static readonly string InvalidDateRange = "InvalidDateRange";

    [Message("'{PropertyName}' is not valid.")]
    public static readonly string ValidRoutingNumberValidator = "ValidRoutingNumberValidator";

    [Message("'{PropertyName}' should be a valid account number.")]
    public static readonly string AccountNumberValidator = "AccountNumberValidator";

    [Message("'{PropertyName}' must be specified as true.")]
    public static readonly string IsPrimaryValidator = "IsPrimaryValidator";

    [Message("At least one Owner and one Bank Account must be entered to submit your enrollment.")]
    public static readonly string OwnerAndBankAccountRequired = "OwnerAndBankAccountRequired";

    [Message("We have already received a previous enrollment for this office.")]
    public static readonly string EnrollmentAlreadyCompleted = "EnrollmentAlreadyCompleted";

    [Message("File content exceeds the maximum allowed size of 10 MB.")]
    public static readonly string FileMaxSize = "FileMaxSize";

    [Message("Each file label must have a corresponding uploaded document.")]
    public static readonly string MissingMatchingDocumentValidator = "MissingMatchingDocumentValidator";

    [Message("This email address is already taken. Please contact DecisionFi Support for further assistance.")]
    public static readonly string UserAlreadyExistsValidator = "UserAlreadyExistsValidator";

    [Message("User is not associated with the specified office.")]
    public static readonly string UserNotAssociatedWithOffice = "UserNotAssociatedWithOffice";

    [Message("This link has expired. Please contact the office that sent you this link for further assistance.")]
    public static readonly string InitiateLoanAppLinkExpired = "InitiateLoanAppLinkExpired";

    [Message("User deletion requires elevated permissions.")]
    public static readonly string SuperAdminUserDeletion = "SuperAdminUserDeletion";

    [Message("Cannot delete User without Azure AD ProviderId. UserId: {PropertyValue}")]
    public static readonly string UserProviderIdRequiredToDelete = "UserProviderIdRequiredToDelete";

    [Message("Cannot activate/deactivate User without Azure AD ProviderId. UserId: {PropertyValue}")]
    public static readonly string UserProviderIdRequired = "UserProviderIdRequired";

    [Message("Failed to update SuperAdmin user status in Azure AD B2C. ProviderId: {PropertyValue}")]
    public static readonly string FailedToUpdateUserInB2C = "FailedToUpdateUserInB2C";

    [Message("You cannot delete your own SuperAdmin account.")]
    public static readonly string SuperAdminDeleteSelfNotAllowed = "SuperAdminDeleteSelfNotAllowed";

    [Message("You cannot activate or deactivate your own SuperAdmin account.")]
    public static readonly string SuperAdminActivateDeactivateSelfNotAllowed = "SuperAdminActivateDeactivateSelfNotAllowed";

    [Message("You cannot delete your own account.")]
    public static readonly string DeleteSelfNotPermitted = "DeleteSelfNotPermitted";

    [Message("You cannot activate or deactivate your own account.")]
    public static readonly string ActivateDeactivateSelfNotPermitted = "ActivateDeactivateSelfNotPermitted";

    [Message("You have exceeded the maximum limit for failed login attempts. Please try again after {PropertyValue} minutes.")]
    public static readonly string BorrowerValidationAttemptsExceeded = "BorrowerValidationAttemptsExceeded";

    [Message("Failed to submit loan application to lender. Please try again or contact support.")]
    public static readonly string FailedToSubmitLoanApplication = "FailedToSubmitLoanApplication";

    [Message("Failed to update loan amount and get offers. Please try again or contact support.")]
    public static readonly string FailedToUpdateLoanAmountAndGetOffers = "FailedToUpdateLoanAmountAndGetOffers";

    [Message("This loan application cannot be cancelled as it is already progress or has been finalized as a loan.")]
    public static readonly string CannotCancelLoanApplication = "CannotCancelLoanApplication";

    [Message("Thread office context does not match the requested office.")]
    public static readonly string ThreadOfficeContextMismatch = "ThreadOfficeContextMismatch";

    [Message("Message edit window has expired. Messages can only be edited within {PropertyValue} days of creation.")]
    public static readonly string MessageEditWindowExpired = "MessageEditWindowExpired";

    [Message("Message delete window has expired. Messages can only be deleted within {PropertyValue} days of creation.")]
    public static readonly string MessageDeleteWindowExpired = "MessageDeleteWindowExpired";

    [Message("Message revert window has expired. Deleted messages can only be reverted within {PropertyValue} days of creation.")]
    public static readonly string MessageRevertWindowExpired = "MessageRevertWindowExpired";

    [Message("You are not authorized to edit this message. Only the message author can edit their own messages.")]
    public static readonly string MessageEditUnauthorized = "MessageEditUnauthorized";

    [Message("The message you are trying to edit no longer exists.")]
    public static readonly string MessageEditDeletedNotAllowed = "MessageEditDeletedNotAllowed";

    [Message("You are not authorized to delete this message. Only the message author can delete their own messages.")]
    public static readonly string MessageDeleteUnauthorized = "MessageDeleteUnauthorized";

    [Message("You are not authorized to delete this file. Only the original file uploader can delete their own files.")]
    public static readonly string FileDeleteUnauthorized = "FileDeleteUnauthorized";

    [Message("This message contains only file attachments. Please specify a FileId to delete individual files instead of deleting the entire message.")]
    public static readonly string MessageFileOnlyDeleteRequiresFileId = "MessageFileOnlyDeleteRequiresFileId";

    [Message("You are not authorized to flag this message. Only the message author can flag their own messages.")]

    public static readonly string MessageFlagUnauthorized = "MessageFlagUnauthorized";

    [Message("You are not authorized to revert this message. Only the message author can revert their own messages.")]
    public static readonly string MessageRevertUnauthorized = "MessageRevertUnauthorized";

    [Message("Cannot create message threads with SuperAdmin offices.")]
    public static readonly string MessageThreadSuperAdminOfficeNotAllowed = "MessageThreadSuperAdminOfficeNotAllowed";

    [Message("You are not authorized to create message threads for this office.")]
    public static readonly string MessageThreadOfficeUnauthorized = "MessageThreadOfficeUnauthorized";

    [Message("Only SuperAdmins can flag internal messages.")]
    public static readonly string MessageFlagInternalUnauthorized = "MessageFlagInternalUnauthorized";

    [Message("A message thread already exists for this entity. Please use the existing thread.")]
    public static readonly string MessageThreadAlreadyExists = "MessageThreadAlreadyExists";

    [Message("Failed to enroll office. Please try again or contact support.")]
    public static readonly string FailedToEnrollOffice = "FailedToEnrollOffice";

    [Message("Invalid OTP code. Please check and try again.")]
    public static readonly string OtpInvalid = "OtpInvalid";

    [Message("Maximum OTP attempts exceeded. Please try again after {PropertyName} minutes.")]
    public static readonly string OtpMaxAttemptsExceeded = "OtpMaxAttemptsExceeded";

    [Message("OTP session not found or expired. Please request a new code.")]
    public static readonly string OtpSessionNotFound = "OtpSessionNotFound";

    [Message("Too many OTP requests. Please try again later.")]
    public static readonly string OtpGenerationRateLimitExceeded = "OtpGenerationRateLimitExceeded";

    [Message("Configuration Error: Missing Funding OTP Settings.")]
    public static readonly string MissingFundingOTPSettings = "MissingFundingOTPSettings";

    [Message("Authorization token not found or expired.")]
    public static readonly string AuthorizationTokenExpired = "AuthorizationTokenExpired";

    [Message("Authorization token is invalid.")]
    public static readonly string AuthorizationTokenInvalid = "AuthorizationTokenInvalid";

    [Message("You have reached the maximum number of concurrent sessions. Please sign out from existing sessions you may have on other devices or browsers and try again.")]
    public static readonly string MaximumConcurrentSessionsReached = "MaximumConcurrentSessionsReached";

    [Message("An active session already exists for this device.")]
    public static readonly string SessionFingerprintAlreadyExists = "SessionFingerprintAlreadyExists";

    [Message("The session does not exist or has expired.")]
    public static readonly string SessionNotFound = "SessionNotFound";

    [Message("The session is not valid for this device.")]
    public static readonly string SessionFingerprintMismatch = "SessionFingerprintMismatch";

    [Message("The session does not belong to the current user.")]
    public static readonly string SessionNotOwnedByUser = "SessionNotOwnedByUser";

    public static string? FetchErrorMessage(string validationCode)
    {
        // Use LINQ to retrieve the field and its associated MessageAttribute
        var field = typeof(ValidationCodes).GetFields(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(x => x.Name == validationCode);

        return field?.GetCustomAttribute<MessageAttribute>()?.Message;
    }

    public static ValidationFailure GenerateValidationFailure(string propertyName, string validationCode, string? errorMessage = null, string? propertyValue = null)
    {
        // Use the provided error message or fetch it using the validation code
        var message = errorMessage ?? FetchErrorMessage(validationCode);

        // Format the message by replacing the placeholder
        var formattedMessage = message?.Replace("{PropertyName}", propertyName, StringComparison.OrdinalIgnoreCase);

        // Format the message by replacing the {PropertyValue} placeholder if propertyValue is provided
        if (propertyValue is not null)
        {
            formattedMessage = formattedMessage?.Replace("{PropertyValue}", propertyValue, StringComparison.OrdinalIgnoreCase);
        }

        // Return the ValidationFailure object
        return new ValidationFailure(propertyName, formattedMessage)
        {
            ErrorCode = validationCode
        };
    }

    public static ValidationException GenerateValidationException(string propertyName, string validationCode, string? errorMessage = null, string? propertyValue = null)
    {
        // Directly create and return a ValidationException
        return new ValidationException(
            [GenerateValidationFailure(propertyName, validationCode, errorMessage, propertyValue)]);
    }
}
