using W2K.Common.Events;

namespace W2K.Identity.Application.Events;

/// <summary>
/// Event used when a user changed their password.
/// </summary>
public record UserPwdChangedDomainEvent
     : DomainEvent
{
    public UserPwdChangedDomainEvent(int userId, string userName, string? source = null)
        : base(userId, userName, source) { }
}
