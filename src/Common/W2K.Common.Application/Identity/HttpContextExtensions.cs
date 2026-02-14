using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using DFI.Common.Application.Auth;
using DFI.Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace DFI.Common.Application.Identity;

public static class HttpContextExtensions
{
    public static bool IsAuthenticated(this HttpContext context)
    {
        return context.User.Identity?.IsAuthenticated == true;
    }

    public static T? GetHeaderValueAs<T>(this HttpContext context, string headerName)
    {
        if (context.Request.Headers.TryGetValue(headerName, out StringValues values))
        {
            var rawValues = values.ToString(); // writes out as Csv when there are multiple.
            if (!rawValues.IsNullOrWhitespace())
            {
                return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
        }
        return default;
    }

    public static AuthenticationHeaderValue? GetAuthenticationHeaderValue(this HttpContext context)
    {
        if (context.IsAuthenticated())
        {
            var value = context.GetHeaderValueAs<string>("Authorization");
            if (value?.Contains(' ', StringComparison.OrdinalIgnoreCase) == true)
            {
                var parts = value.Split(' ');
                return new AuthenticationHeaderValue(parts[0], parts[1]);
            }
        }
        return null;
    }

    public static int? GetUserId(this HttpContext context)
    {
        int? id = null;
        if (context.IsAuthenticated())
        {
            id = context.User.FindFirst(IdentityConstants.UserIdClaimTypeName)?.Value?.ToNullableInt();
        }
        return id;
    }

    public static bool IsApiKeyAuth(this HttpContext context)
    {
        return !string.IsNullOrEmpty(context.GetHeaderValueAs<string>(AuthConstants.ApiKeyHeaderName))
            && context.GetUserId() is null;
    }

    public static string? GetApiKey(this HttpContext context)
    {
        return context.IsAuthenticated() ? context.GetHeaderValueAs<string>(AuthConstants.ApiKeyHeaderName) : null;
    }

    public static string? GetSessionId(this HttpContext context)
    {
        return context.GetHeaderValueAs<string>(AuthConstants.SessionIdHeaderName);
    }

    public static string? GetAuthProviderId(this HttpContext context)
    {
        string? id = null;
        if (context.IsAuthenticated())
        {
            id = context.User.FindFirst(IdentityConstants.ObjectIdClaimTypeName)?.Value;
        }
        return id;
    }

    public static string? GetFirstName(this HttpContext context)
    {
        string? name = null;
        if (context.IsAuthenticated())
        {
            name = context.User.FindFirst(ClaimTypes.GivenName)?.Value;
        }
        return name;
    }

    public static string? GetLastName(this HttpContext context)
    {
        string? name = null;
        if (context.IsAuthenticated())
        {
            name = context.User.FindFirst(ClaimTypes.Surname)?.Value;
        }
        return name;
    }

    public static string? GetFullName(this HttpContext context)
    {
        string? name = null;
        if (context.IsAuthenticated() && context.User.HasClaim(x => x.Type == ClaimTypes.GivenName))
        {
            name = string.Concat(
                context.User.FindFirst(ClaimTypes.GivenName)?.Value,
                " ",
                context.User.FindFirst(ClaimTypes.Surname)?.Value);
        }
        return name;
    }

    public static string? GetEmail(this HttpContext context)
    {
        string? email = null;
        if (context.IsAuthenticated())
        {
            email = context.User.FindFirst(ClaimTypes.Email)?.Value ?? context.User.FindFirst(IdentityConstants.EmailsClaimTypeName)?.Value;
        }
        return email;
    }

    public static string? GetMobilePhone(this HttpContext context)
    {
        string? phone = null;
        if (context.IsAuthenticated())
        {
            phone = context.User.FindFirst(IdentityConstants.MobilePhoneClaimTypeName)?.Value;
        }
        return phone;
    }

    public static bool GetIsDisabled(this HttpContext context)
    {
        bool val = false;
        if (context.IsAuthenticated())
        {
            val = context.User.FindFirst(IdentityConstants.IsDisabledClaimTypeName)?.Value.ToBool() ?? false;
        }
        return val;
    }

    public static bool GetIsMultipleOffices(this HttpContext context)
    {
        bool val = false;
        if (context.IsAuthenticated())
        {
            val = context.User.FindFirst(IdentityConstants.IsMultipleOfficesClaimTypeName)?.Value.ToBool() ?? false;
        }
        return val;
    }

    public static string? GetRequestIp(this HttpContext context, bool tryUseXForwardHeader = true)
    {
        string? ip = null;

        // need to support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

        // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
        // for 99% of cases however it has been suggested that a better (although tedious)
        // approach might be to read each IP from right to left and use the first public IP.
        // http://stackoverflow.com/a/43554000/538763
        //
        if (tryUseXForwardHeader)
        {
            ip = context.GetHeaderValueAs<string>(AuthConstants.XForwardedForHeaderName)?.SplitCsv()?.FirstOrDefault();
        }

        // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
        if (ip.IsNullOrWhitespace())
        {
            ip = context.Connection.RemoteIpAddress?.ToString();
        }

        if (ip.IsNullOrWhitespace())
        {
            ip = context.GetHeaderValueAs<string>(AuthConstants.RemoteAddressHeaderName);
        }

        if (ip.IsNullOrWhitespace())
        {
            ip = context.GetHeaderValueAs<string>(AuthConstants.ClientIpHeaderName);
        }

        // _httpContextAccessor.HttpContext.Request?.Host this is the local host.

        if (ip.IsNullOrWhitespace())
        {
            ip = "unknown";
        }

        // check for existence of port in ip address
        if (ip != "::1" && !ip.IsIPV6() && ip!.Contains(':', StringComparison.OrdinalIgnoreCase))
        {
            var i = ip.LastIndexOf(':');
            ip = ip[..i];
        }

        if (ip.IsNullOrWhitespace())
        {
            ip = "unknown";
        }

        return ip;
    }

    private static bool IsIPV6(this string? s)
    {
        _ = IPAddress.TryParse(s, out IPAddress? ip);
        return ip?.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
    }

}
