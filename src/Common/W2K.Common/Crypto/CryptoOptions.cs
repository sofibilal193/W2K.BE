#pragma warning disable CA1819 // Properties should not return arrays

namespace W2K.Common.Crypto;

public record CryptoOptions(byte[] Key, int KeySize = 256)
{
}

#pragma warning restore CA1819 // Properties should not return arrays
