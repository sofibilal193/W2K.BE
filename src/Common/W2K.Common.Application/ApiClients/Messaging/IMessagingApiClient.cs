using DFI.Common.Application.Commands.Messaging;

namespace DFI.Common.Application.ApiClients;

/// <summary>
/// API client for communicating with the Messaging service.
/// </summary>
public interface IMessagingApiClient
{
    Task<UpsertMessageResponse> AddMessageAsSuperAdminAsync(UpsertMessageCommand command, CancellationToken cancel);
    Task<UpsertMessageResponse> AddMessageAsMerchantAdminAsync(UpsertMessageCommand command, CancellationToken cancel);
}
