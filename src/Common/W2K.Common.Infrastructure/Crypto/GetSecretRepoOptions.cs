using W2K.Common.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace W2K.Common.Infrastructure.Crypto;

public record GetSecretRepoOptions : SecretRepoOptions
{
    public GetSecretRepoOptions(IOptionsMonitor<CryptoSettings> settingsAccessor)
        : base(settingsAccessor.CurrentValue.AzureKeyVaultUri)
    {
    }
}
