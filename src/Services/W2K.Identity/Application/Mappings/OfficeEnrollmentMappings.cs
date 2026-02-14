using W2K.Identity.Entities;
using W2K.Identity.Enums;

namespace W2K.Identity.Application.Mappings;

internal static class OfficeMappings
{
    public static string MapEnrollmentStatus(Office office)
    {
        return office switch
        {
            { IsDisabled: true } => nameof(OfficeEnrollmentStatus.Inactive),
            { IsEnrollmentCompleted: false } => nameof(OfficeEnrollmentStatus.Incomplete),
            { IsReviewed: false, IsEnrollmentCompleted: true } => nameof(OfficeEnrollmentStatus.Enrolled),
            { IsReviewed: true, IsApproved: false } => nameof(OfficeEnrollmentStatus.Reviewed),
            { IsReviewed: true, IsApproved: true } => nameof(OfficeEnrollmentStatus.Approved),
            _ => nameof(OfficeEnrollmentStatus.Denied)
        };
    }
}
