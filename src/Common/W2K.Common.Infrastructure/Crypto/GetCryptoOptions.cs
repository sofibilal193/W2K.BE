using DFI.Common.Crypto;
using DFI.Common.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace DFI.Common.Infrastructure.Crypto;

public record GetCryptoOptions : CryptoOptions
{
    public GetCryptoOptions(
        IOptionsMonitor<CryptoSettings> settingsAccessor,
        ISecretRepository secretRepository)
        : base(
            secretRepository.UnWrapKeyAsync(
                settingsAccessor.CurrentValue.WrappedEncryptionKeyBase64Encoded,
                settingsAccessor.CurrentValue.EncryptionWrapKeyName).GetAwaiter().GetResult() ?? [])
    {
        if (string.IsNullOrEmpty(settingsAccessor.CurrentValue.WrappedEncryptionKeyBase64Encoded))
        {
            throw new ArgumentNullException(nameof(settingsAccessor), "CryptoSettings.WrappedEncryptionKeyBase64Encoded is null/empty.");
        }

        if (string.IsNullOrEmpty(settingsAccessor.CurrentValue.EncryptionWrapKeyName))
        {
            throw new ArgumentNullException(nameof(settingsAccessor), "CryptoSettings.EncryptionWrapKeyName is null/empty.");
        }
    }
}
