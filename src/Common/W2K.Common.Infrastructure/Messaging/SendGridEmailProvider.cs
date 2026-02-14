using System.Collections.ObjectModel;
using System.Net;
using W2K.Common.Application.Messaging;
using W2K.Common.Exceptions;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace W2K.Common.Infrastructure.Messaging;

/// <summary>
/// An implementation of IEmailprovider using SendGrid
/// </summary>
public class SendGridEmailProvider(
    ISendGridClient client) : IEmailProvider
{
    #region Private Properties

    private readonly ISendGridClient _client = client;

    #endregion

    #region IEmailProvider Methods

    public async Task<bool> SendAsync(
        ReadOnlyCollection<EmailMessage> messages,
        CancellationToken cancel = default)
    {
        foreach (var msg in messages)
        {
            var message = BuildSendGridMessage(msg);

            if (message.Personalizations?.Count > 0)
            {
                await SendMessageAsync(message, cancel);
            }
        }
        return true;
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

    private static SendGridMessage BuildSendGridMessage(EmailMessage msg)
    {
        var message = new SendGridMessage
        {
            From = new SendGrid.Helpers.Mail.EmailAddress(msg.From.Address, msg.From.Name),
            Subject = msg.Subject,
            PlainTextContent = msg.IsHtml ? null : msg.Body,
            HtmlContent = msg.IsHtml ? msg.Body : null,
        };

        // set replyto
        if (msg.ReplyTo.HasValue)
        {
            message.ReplyTo = new SendGrid.Helpers.Mail.EmailAddress
                (msg.ReplyTo.Value.Address, msg.ReplyTo.Value.Name);
        }

        // add To recipients
        if (msg.To is not null)
        {
            foreach (var item in GetUniqueEmails(msg.To))
            {
                message.AddTo(new SendGrid.Helpers.Mail.EmailAddress { Email = item.Address, Name = item.Name });
            }
        }

        // add Cc recipients
        if (msg.Cc is not null)
        {
            foreach (var item in GetUniqueEmails(msg.Cc))
            {
                message.AddCc(new SendGrid.Helpers.Mail.EmailAddress { Email = item.Address, Name = item.Name });
            }
        }

        // add Bcc recipients
        if (msg.Bcc is not null)
        {
            foreach (var item in GetUniqueEmails(msg.Bcc))
            {
                message.AddBcc(new SendGrid.Helpers.Mail.EmailAddress { Email = item.Address, Name = item.Name });
            }
        }

        // set email template if needed
        if (!string.IsNullOrEmpty(msg.TemplateId))
        {
            message.SetTemplateId(msg.TemplateId);
        }

        // set template data
        if (msg.TemplateData is not null)
        {
            message.SetTemplateData(msg.TemplateData);
        }

        // set unsubscribe id
        if (msg.UnSubscribeGroupId.HasValue)
        {
            message.Asm = new ASM
            {
                GroupId = msg.UnSubscribeGroupId.Value,
                GroupsToDisplay = [msg.UnSubscribeGroupId.Value]
            };
        }

        return message;
    }

    private async Task SendMessageAsync(SendGridMessage message, CancellationToken cancel)
    {
        var response = await _client.SendEmailAsync(message, cancel);
        if (response is not null)
        {
            var code = response.StatusCode;
            if (code is not HttpStatusCode.OK and not HttpStatusCode.Accepted)
            {
                var result = await response.Body.ReadAsStringAsync(cancel);
                throw new DomainException(result);
            }
        }
    }

    private static ReadOnlyCollection<Application.Messaging.EmailAddress> GetUniqueEmails(ReadOnlyCollection<Application.Messaging.EmailAddress> emailAddresses)
    {
        var list = new List<Application.Messaging.EmailAddress>();
        var filteredAddresses = emailAddresses.Where(x => !list.Any(x => x.Address.Equals(x.Address, StringComparison.OrdinalIgnoreCase)));
        foreach (var address in filteredAddresses)
        {
            list.Add(address);
        }
        return list.AsReadOnly();
    }

    #endregion
}
