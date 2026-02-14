using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace W2K.Common.Infrastructure.Logging;

public class HttpClientLoggingHandler(IHostEnvironment env, ILogger<HttpClientLoggingHandler> logger, IFeatureManager featureManager) : DelegatingHandler
{
    #region Private Variables

    private readonly IHostEnvironment _env = env;
    private readonly ILogger<HttpClientLoggingHandler> _logger = logger;
    private readonly IFeatureManager _featureManager = featureManager;

    #endregion
    #region Constructors

    #endregion

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        bool enabled;
        if (_env.IsDevelopment())
        {
            enabled = true;
        }
        else
        {
            var featureName = $"httpclient-logging-{request.RequestUri?.Authority?.ToLowerInvariant().Replace(":", "-")}";
            enabled = await _featureManager.IsEnabledAsync(featureName, cancellationToken);
        }

        if (enabled)
        {
            if (await _featureManager.IsEnabledAsync("httpclient-logging-logheaders", cancellationToken))
            {
                _logger.LogInformation("Request Headers:{NewLine}{Headers}", Environment.NewLine, JsonSerializer.Serialize(request.Headers));
            }

            // Skipping Headers to log in logger so we can skip logging sensitive information like token, credentials and so on.
            var requestData = request.ToString();
            _logger.LogInformation("Request:{NewLine}{Request}", Environment.NewLine, requestData.Split("Headers")[0]);

            if (request.Content is not null)
            {
                _logger.LogInformation("Request Content:{NewLine}{Content}", Environment.NewLine, await request.Content.ReadAsStringAsync(cancellationToken));
            }
        }

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (enabled)
        {
            _logger.LogInformation("Response:{NewLine}{Response}", Environment.NewLine, response.ToString());

            if (response.Content is not null)
            {
                _logger.LogInformation("Response Content:{NewLine}{Content}", Environment.NewLine, await response.Content.ReadAsStringAsync(cancellationToken));
            }
        }

        return response;
    }
}
