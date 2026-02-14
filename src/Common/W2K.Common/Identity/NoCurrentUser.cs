using System.Collections.ObjectModel;

namespace DFI.Common.Identity;

public class NoCurrentUser : ICurrentUser
{
    public bool IsAuthenticated => false;

    public string? AuthProviderId => null;

    public string? FullName => null;

    public string? FirstName => null;

    public string? LastName => null;

    public string? Email => null;

    public string? MobilePhone => null;

    public string? Source => null;

    public int? UserId => null;

    public bool IsDisabled => false;

    public bool IsMultipleOffices => false;

    public bool IsClientCredentialsToken => false;

    public OfficeType OfficeType { get; } = OfficeType.Merchant;

    public ReadOnlyCollection<int>? OfficeIds { get; }
}
