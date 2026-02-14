using W2K.Common.ValueObjects;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public record OfficeEnrollmentSubmittedNotification(
int OfficeId,
string OfficeName,
string OfficePhone,
Address? Address,
bool IsSuperAdminNotification,
bool IsOfficeApproved
) : INotification;
