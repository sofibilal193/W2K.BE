using DFI.Common.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace DFI.Common.Infrastructure.Crypto;

public record GetSecretRepoOptions : SecretRepoOptions
{
    public GetSecretRepoOptions(IOptionsMonitor<CryptoSettings> settingsAccessor)
        : base(settingsAccessor.CurrentValue.AzureKeyVaultUri)
    {
    }
}
