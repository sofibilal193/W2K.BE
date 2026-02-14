using System.Collections.ObjectModel;

namespace W2K.Common.Application.Messaging;

public interface ISmsProvider : IDisposable
{
    Task<ReadOnlyCollection<string>> SendAsync(
        ReadOnlyCollection<SmsMessage> messages,
        CancellationToken cancel = default);
}
