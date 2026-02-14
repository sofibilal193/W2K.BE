using W2K.Common.Events;

namespace W2K.Identity.Application.Events;

/// <summary>
/// Event used when a user logs in.
/// </summary>
public record UserLoggedInDomainEvent
     : DomainEvent
{
    public UserLoggedInDomainEvent(int userId, string userName, string? source = null, int? officeId = null)
        : base(userId, userName, source, officeId) { }
}
