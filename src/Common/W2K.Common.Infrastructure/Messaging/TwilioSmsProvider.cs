using System.Collections.ObjectModel;
using W2K.Common.Application.Messaging;
using Microsoft.Extensions.Logging;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace W2K.Common.Infrastructure.Messaging;

/// <summary>
/// An implementation of ISmsProvider using Twilio
/// </summary>
public class TwilioSmsProvider(ITwilioRestClient client, ILogger<TwilioSmsProvider> logger) : ISmsProvider
{
    #region Private Properties

    private readonly ITwilioRestClient _client = client;
    private readonly ILogger<TwilioSmsProvider> _logger = logger;

    #endregion

    #region ISmsProvider Methods

    public async Task<ReadOnlyCollection<string>> SendAsync(ReadOnlyCollection<SmsMessage> messages, CancellationToken cancel = default)
    {
        var errors = new List<string>();
        foreach (var msg in messages)
        {
            try
            {
                var message = await MessageResource.CreateAsync(
                    to: new PhoneNumber(GetFormattedUSPhone(msg.To)),
                    from: new PhoneNumber(GetFormattedUSPhone(msg.From)),
                    body: msg.Body,
                    client: _client); // pass in the custom client
                if (message is not null && !string.IsNullOrEmpty(message.Sid))
                {
                    msg.SetSid(message.Sid);
                }
            }
            catch (Exception ex)
            {
                errors.Add($"SMS Message Failure. From: {msg.From}. To: {msg.To}. with exception: {ex.Message}");
                _logger.LogWarning(ex, "SMS Message Failure. From: {From}. To: {To}.", msg.From, msg.To);
            }
        }
        return errors.AsReadOnly();
    }

    #endregion

    #region IDisposable Methods

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // dispose resources
    }

    #endregion

    #region Private Methods

    private static string GetFormattedUSPhone(string phone)
    {
        var util = PhoneNumbers.PhoneNumberUtil.GetInstance();
        var number = util.Parse(phone, "US");
        return util.Format(number, PhoneNumbers.PhoneNumberFormat.E164);
    }

    #endregion

}
