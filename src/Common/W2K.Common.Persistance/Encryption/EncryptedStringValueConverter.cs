using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using W2K.Common.Crypto;

namespace W2K.Common.Persistence.Encryption;

internal sealed class EncryptedStringValueConverter(ICryptoProvider? cryptoProvider = null, byte[]? encryptionKey = null, ConverterMappingHints? mappingHints = null)
    : ValueConverter<string, string>(
        x => cryptoProvider == null
                ? x
                : cryptoProvider.EncryptString(x, encryptionKey) ?? x,
        x => cryptoProvider == null
                ? x
                : cryptoProvider.DecryptString(x, encryptionKey) ?? x,
        mappingHints)
{
}
