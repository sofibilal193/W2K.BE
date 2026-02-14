using System.Collections.ObjectModel;

namespace W2K.Common.Identity;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    string? AuthProviderId { get; }

    string? FullName { get; }

    string? FirstName { get; }

    string? LastName { get; }

    string? Email { get; }

    string? MobilePhone { get; }

    string? Source { get; }

    int? UserId { get; }

    bool IsDisabled { get; }

    bool IsMultipleOffices { get; }

    bool IsClientCredentialsToken { get; }

    OfficeType OfficeType { get; }

    ReadOnlyCollection<int>? OfficeIds { get; }
}
