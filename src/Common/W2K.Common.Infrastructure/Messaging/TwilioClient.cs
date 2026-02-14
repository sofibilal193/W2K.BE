using W2K.Common.Infrastructure.Settings;
using Twilio.Clients;
using Twilio.Http;

namespace W2K.Common.Infrastructure.Messaging;

public class TwilioClient : ITwilioRestClient
{
    private readonly TwilioRestClient _innerClient;

    public string AccountSid => _innerClient.AccountSid;
    public string Region => _innerClient.Region;
    public Twilio.Http.HttpClient HttpClient => _innerClient.HttpClient;

    public TwilioClient(TwilioSettings settings, System.Net.Http.HttpClient httpClient)
    {
        // customize the underlying HttpClient
        httpClient.DefaultRequestHeaders.Add("X-Client-App", "DFI");
        _innerClient = new TwilioRestClient(
            settings.AccountSid,
            settings.AuthToken,
            httpClient: new SystemNetHttpClient(httpClient));
    }

    public Response Request(Request request)
    {
        return _innerClient.Request(request);
    }

    public Task<Response> RequestAsync(Request request)
    {
        return _innerClient.RequestAsync(request);
    }

}
