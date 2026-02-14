using System.Collections.ObjectModel;

namespace W2K.Common.Application.Messaging;

public interface IEmailProvider : IDisposable
{
    Task<bool> SendAsync(
        ReadOnlyCollection<EmailMessage> messages,
        CancellationToken cancel = default);
}
