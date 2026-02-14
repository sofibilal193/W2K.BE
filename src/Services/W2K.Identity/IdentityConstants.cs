namespace W2K.Identity;

public static class IdentityConstants
{
    public static readonly string ApplicationName = "W2K.Identity";

    public static readonly string OfficePrimaryAddressType = "Primary";

    public static readonly int DefaultCacheExpirationMinutes = 60;

    public static string AdministratorRoleName => "Administrator";

    public static string MerchantEmailSubject => "Welcome to DecisionFi";

    public static string MerchantEmailButtonText => "Sign-In to Access";

    public static string SuperAdminEmailButtonText => "Start New Office Enrollment";

    public static string NewOfficeEnrollment => "New Office Enrollment";
}
