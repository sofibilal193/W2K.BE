using W2K.Identity.Entities;
using W2K.Identity.Enums;

namespace W2K.Identity.Application.Mappings;

public static class UserMappings
{
    private const string Invited = nameof(UserEnrollmentStatus.Invited);
    private const string Accepted = nameof(UserEnrollmentStatus.Accepted);
    private const string Inactive = nameof(UserEnrollmentStatus.Inactive);

    public static string MapUserStatus(User? user)
    {
        if (user is null) return Invited;

        if (user.IsDisabled || user.Offices?.All(x => x.IsDisabled) != false) return Inactive;

        var office = user.Offices?.FirstOrDefault(x => x.IsDefault);
        return office is { IsInvited: true, IsInviteProcessed: true } ? Accepted : Invited;
    }
}
