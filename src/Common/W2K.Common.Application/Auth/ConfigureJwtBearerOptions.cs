using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace DFI.Common.Application.Auth;

public class ConfigureJwtBearerOptions : IPostConfigureOptions<JwtBearerOptions>
{
    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        var originalOnMessageReceived = options.Events.OnMessageReceived;
        options.Events.OnMessageReceived = async x =>
        {
            await originalOnMessageReceived(x);
            if (string.IsNullOrEmpty(x.Token))
            {
                var accessToken = x.Request.Query["access_token"];
                var path = x.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    x.Token = accessToken;
                }
            }
        };
    }
}
