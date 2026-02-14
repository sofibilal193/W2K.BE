#pragma warning disable CA1819 // Properties should not return arrays

namespace DFI.Common.Crypto;

public record CryptoKeys(byte[] Key, byte[] Iv);

#pragma warning restore CA1819 // Properties should not return arrays
