using W2K.Common.Events;

namespace W2K.Identity.Application.Events;

/// <summary>
/// Event used when a user logs out.
/// </summary>
public record UserLoggedOutDomainEvent
     : DomainEvent
{
    public UserLoggedOutDomainEvent(int userId, string userName, string? source = null, int? officeId = null)
        : base(userId, userName, source, officeId) { }
}
