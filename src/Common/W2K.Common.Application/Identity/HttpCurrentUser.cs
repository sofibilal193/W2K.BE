using System.Collections.ObjectModel;
using W2K.Common.Identity;
using Microsoft.AspNetCore.Http;

namespace W2K.Common.Application.Identity;

public class HttpCurrentUser : ICurrentUser
{
    public string? AuthProviderId { get; }

    public bool IsAuthenticated { get; }

    public string? FullName { get; }

    public string? FirstName { get; }

    public string? LastName { get; }

    public string? Email { get; }

    public string? MobilePhone { get; }

    public string? Source { get; }

    public int? UserId { get; }

    public bool IsDisabled { get; }

    public bool IsMultipleOffices { get; }

    public bool IsClientCredentialsToken { get; }

    public OfficeType OfficeType { get; }

    public ReadOnlyCollection<int>? OfficeIds { get; }

    public HttpCurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        var context = httpContextAccessor.HttpContext;
        IsAuthenticated = context?.IsAuthenticated() ?? false;
        AuthProviderId = context?.GetAuthProviderId();
        FullName = context?.GetFullName();
        FirstName = context?.GetFirstName();
        LastName = context?.GetLastName();
        Email = context?.GetEmail();
        MobilePhone = context?.GetMobilePhone();
        Source = context?.GetRequestIp();
        UserId = context?.GetUserId();
        IsDisabled = context?.GetIsDisabled() ?? false;
        IsMultipleOffices = context?.GetIsMultipleOffices() ?? false;
        OfficeType = context?.User?.GetOfficeType() ?? OfficeType.Merchant;
        OfficeIds = context?.User?.GetOfficeIds() ?? null;
        IsClientCredentialsToken = context?.User?.IsClientCredentialsToken() ?? false;
    }
}
