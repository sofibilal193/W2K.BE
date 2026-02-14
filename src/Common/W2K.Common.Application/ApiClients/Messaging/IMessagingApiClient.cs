using W2K.Common.Application.Commands.Messaging;

namespace W2K.Common.Application.ApiClients;

/// <summary>
/// API client for communicating with the Messaging service.
/// </summary>
public interface IMessagingApiClient
{
    Task<UpsertMessageResponse> AddMessageAsSuperAdminAsync(UpsertMessageCommand command, CancellationToken cancel);
    Task<UpsertMessageResponse> AddMessageAsMerchantAdminAsync(UpsertMessageCommand command, CancellationToken cancel);
}
