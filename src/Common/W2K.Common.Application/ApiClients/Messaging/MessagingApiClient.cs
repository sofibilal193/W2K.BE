using DFI.Common.Application.ApiServices;
using DFI.Common.Application.Commands.Messaging;

namespace DFI.Common.Application.ApiClients;

/// <summary>
/// API client implementation for communicating with the Messaging service.
/// </summary>
public class MessagingApiClient(IApiService apiService) : IMessagingApiClient
{
    private static readonly string ServiceType = ApiServiceTypes.Messaging;
    private readonly IApiService _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));

    /// <inheritdoc />
    public async Task<UpsertMessageResponse> AddMessageAsSuperAdminAsync(UpsertMessageCommand command, CancellationToken cancel)
    {
        var url = $"superadmin/offices/{command.OfficeId}/threads/{command.ThreadId}/messages?isInternal={command.IsInternal}";

        return await _apiService.PostAsync<UpsertMessageCommand, UpsertMessageResponse>(ServiceType, url, command, cancel);
    }

    public async Task<UpsertMessageResponse> AddMessageAsMerchantAdminAsync(UpsertMessageCommand command, CancellationToken cancel)
    {
        var url = $"offices/{command.OfficeId}/threads/{command.ThreadId}/messages";

        return await _apiService.PostAsync<UpsertMessageCommand, UpsertMessageResponse>(ServiceType, url, command, cancel);
    }
}

