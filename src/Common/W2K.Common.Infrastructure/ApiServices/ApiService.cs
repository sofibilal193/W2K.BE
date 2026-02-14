using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DFI.Common.Application.ApiServices;
using DFI.Common.Application.Auth;
using DFI.Common.Application.Identity;
using DFI.Common.Application.Settings;
using DFI.Common.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace DFI.Common.Infrastructure.ApiServices;

public class ApiService(
    IHttpClientFactory clientFactory,
    IHttpContextAccessor context,
    IServiceProvider serviceProvider,
    IOptions<AppSettings> settingsOptions) : IApiService
{
    private readonly IHttpClientFactory _clientFactory = clientFactory;
    private readonly IHttpContextAccessor _context = context;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly AppSettings _settings = settingsOptions.Value;
    private bool _isDevTokenRequest;

    public async Task<TResponse?> GetAsync<TResponse>(string serviceType, string url, CancellationToken cancel = default)
    {
        var client = await GetHttpClientAsync(serviceType, cancel);
        var requestUri = CreateUri(url);
        var response = await client.GetAsync(requestUri, cancel);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
        await ValidateResponseAsync(response, cancel);
        return await ReadContentAsync<TResponse>(response, cancel);
    }

    public async Task<TResponse?> GetAsync<TResponse>(string serviceType, string url, Dictionary<string, object?> queryParameters, CancellationToken cancel = default)
    {
        var client = await GetHttpClientAsync(serviceType, cancel);
        var parameters = queryParameters.ToDictionary(x => x.Key, x => x.Value?.ToString());
        url = QueryHelpers.AddQueryString(url, parameters);
        var requestUri = CreateUri(url);
        var response = await client.GetAsync(requestUri, cancel);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
        await ValidateResponseAsync(response, cancel);
        return await ReadContentAsync<TResponse>(response, cancel);
    }

    public async Task PostAsync<TRequest>(
        string serviceType, string url, TRequest request, CancellationToken cancel = default)
    {
        var client = await GetHttpClientAsync(serviceType, cancel);
        var requestUri = CreateUri(url);
        var response = await client.PostAsJsonAsync(requestUri, request, cancel);
        await ValidateResponseAsync(response, cancel);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string serviceType, string url, TRequest request, CancellationToken cancel = default)
    {
        var client = await GetHttpClientAsync(serviceType, cancel);
        var requestUri = CreateUri(url);
        var response = await client.PostAsJsonAsync(requestUri, request, cancel);
        await ValidateResponseAsync(response, cancel);
        return await ReadContentAsync<TResponse>(response, cancel);
    }

    public async Task<TResponse?> PostContentAsync<TResponse>(
      string serviceType, string url, MultipartFormDataContent content, CancellationToken cancel = default)
    {
        var client = await GetHttpClientAsync(serviceType, cancel);
        var requestUri = CreateUri(url);
        var response = await client.PostAsync(requestUri, content, cancel);
        await ValidateResponseAsync(response, cancel);
        return await ReadContentAsync<TResponse>(response, cancel);
    }

    public async Task PutAsync<TRequest>(
        string serviceType, string url, TRequest request, CancellationToken cancel = default)
    {
        var client = await GetHttpClientAsync(serviceType, cancel);
        var requestUri = CreateUri(url);
        var response = await client.PutAsJsonAsync(requestUri, request, cancel);
        await ValidateResponseAsync(response, cancel);
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(
        string serviceType, string url, TRequest request, CancellationToken cancel = default)
    {
        var client = await GetHttpClientAsync(serviceType, cancel);
        var requestUri = CreateUri(url);
        var response = await client.PutAsJsonAsync(requestUri, request, cancel);
        await ValidateResponseAsync(response, cancel);
        return await ReadContentAsync<TResponse>(response, cancel);
    }

    public async Task DeleteAsync(string serviceType, string url, CancellationToken cancel = default)
    {
        var client = await GetHttpClientAsync(serviceType, cancel);
        var requestUri = CreateUri(url);
        var response = await client.DeleteAsync(requestUri, cancel);
        await ValidateResponseAsync(response, cancel);
    }

    public async Task<TResponse?> DeleteAsync<TResponse>(
        string serviceType, string url, CancellationToken cancel = default)
    {
        var client = await GetHttpClientAsync(serviceType, cancel);
        var requestUri = CreateUri(url);
        var response = await client.DeleteAsync(requestUri, cancel);
        await ValidateResponseAsync(response, cancel);
        return await ReadContentAsync<TResponse>(response, cancel);
    }

    private async Task<HttpClient> GetHttpClientAsync(string serviceType, CancellationToken cancel)
    {
        var client = _clientFactory.CreateClient(serviceType);
        client.DefaultRequestVersion = HttpVersion.Version20;
        client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;

        // forward auth header (except WebhookApi scheme)
        var authHeaderValue = _context.HttpContext?.GetAuthenticationHeaderValue();
        if (authHeaderValue?.Scheme != AuthConstants.WebhookApiAuthScheme)
        {
            client.DefaultRequestHeaders.Authorization = authHeaderValue;
        }

        // if app using client credentials, aquire auth token
        if (_settings.AuthSettings.Type == ApiAuthType.ClientCredentials)
        {
            if (!string.IsNullOrEmpty(_settings.AuthSettings.ClientCredentials?.ClientId))
            {
                if (string.IsNullOrEmpty(_settings.AuthSettings.ClientCredentials?.Scope))
                {
                    throw new DomainException("AppSettings.AuthSettings.ClientCredentials.Scope is required when ApiAuthType is ClientCredentials.");
                }
                string[] scopes = [_settings.AuthSettings.ClientCredentials.Value.Scope!];
                var clientApp = _serviceProvider.GetRequiredService<IConfidentialClientApplication>();
                var result = await clientApp.AcquireTokenForClient(scopes).ExecuteAsync(cancel);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthConstants.BearerAuthScheme, result.AccessToken);
            }
            else if (!_isDevTokenRequest)
            {
                // get dev token
                var command = new ValidateDevUserCommand(
                    _settings.AuthSettings.BasicAuthUserName ?? "",
                    _settings.AuthSettings.BasicAuthPassword ?? "");
                _isDevTokenRequest = true;
                var response = await PostAsync<ValidateDevUserCommand, ValidateDevUserResponse>(
                    ApiServiceTypes.Identity,
                    "devtokens",
                    command,
                    cancel);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthConstants.BearerAuthScheme, response.Token);
                _isDevTokenRequest = false;
            }
        }

        // forward API key only when no Bearer token is present
        if (client.DefaultRequestHeaders.Authorization is null || client.DefaultRequestHeaders.Authorization.Scheme != AuthConstants.BearerAuthScheme)
        {
            var apiKey = _context.HttpContext?.GetApiKey() ?? _settings.AuthSettings.ApiKey;
            if (!string.IsNullOrEmpty(apiKey))
            {
                client.DefaultRequestHeaders.Add(AuthConstants.ApiKeyHeaderName, apiKey);
            }
        }

        // forward client ip address
        var clientIp = _context.HttpContext?.GetRequestIp();
        if (!string.IsNullOrEmpty(clientIp))
        {
            client.DefaultRequestHeaders.Add(AuthConstants.XForwardedForHeaderName, clientIp);
        }

        // forward Session-Id (Application Insights traceability)
        var sessionId = _context.HttpContext?.GetHeaderValueAs<string>("Session-Id");
        if (!string.IsNullOrEmpty(sessionId))
        {
            client.DefaultRequestHeaders.Add(AuthConstants.AppInsightsSessionIdHeaderName, sessionId);
        }

        // forward X-Session-Id (app session auth)
        var xSessionId = _context.HttpContext?.GetSessionId();
        if (!string.IsNullOrEmpty(xSessionId))
        {
            client.DefaultRequestHeaders.Add(AuthConstants.SessionIdHeaderName, xSessionId);
        }

        // forward X-FingerPrint
        var xFingerPrint = _context.HttpContext?.GetHeaderValueAs<string>(AuthConstants.FingerPrintHeaderName);
        if (xFingerPrint is not null)
        {
            client.DefaultRequestHeaders.Add(AuthConstants.FingerPrintHeaderName, xFingerPrint);
        }

        // forward Internal-Service-Auth-Key
        if (!string.IsNullOrEmpty(_settings.AuthSettings.InternalServiceAuthKey))
        {
            client.DefaultRequestHeaders.Add(AuthConstants.InternalServiceAuthKeyHeaderName, _settings.AuthSettings.InternalServiceAuthKey);
        }

        return client;
    }

    private static Uri CreateUri(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new DomainException("Request URL cannot be null or whitespace.");
        }

        if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
        {
            return uri;
        }

        throw new DomainException($"Request URL '{url}' is invalid.");
    }

    private static async Task ValidateResponseAsync(HttpResponseMessage response, CancellationToken cancel)
    {
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var problemDetails = await response.Content
                .ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken: cancel);
            if (problemDetails is not null)
            {
                var errors = new List<ValidationFailure>();
                foreach (var error in problemDetails.Errors)
                {
                    errors.AddRange(error.Value.Select(x => new ValidationFailure(error.Key, x) { ErrorCode = "ApiError" }));
                }
                throw new ValidationException(errors);
            }
        }
        else if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new HttpRequestException("Unauthorized", null, HttpStatusCode.Unauthorized);
        }
        else if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new HttpRequestException("Forbidden", null, HttpStatusCode.Forbidden);
        }

        _ = response.EnsureSuccessStatusCode();
    }

    private static async Task<T?> ReadContentAsync<T>(HttpResponseMessage response, CancellationToken cancel)
    {
        var content = await response.Content.ReadAsStringAsync(cancel);
        if (string.IsNullOrEmpty(content))
        {
            return default;
        }

        return typeof(T) == typeof(string)
            ? (T)(object)content
            : await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancel);
    }
}
