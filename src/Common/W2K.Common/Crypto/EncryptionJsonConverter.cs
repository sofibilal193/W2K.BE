using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using DFI.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DFI.Common.Crypto;

public class EncryptionJsonConverter<T> : JsonConverter<T?>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var provider = options.GetServiceProvider();
        var cryptoProvider = provider.GetRequiredService<IClientCryptoProvider>();

        var value = cryptoProvider.DecryptString(reader.GetString()?.Trim());
        if (value is null)
        {
            return default;
        }

        var underlyingType = Nullable.GetUnderlyingType(typeToConvert);
        if (underlyingType is not null)
        {
            typeToConvert = underlyingType;
        }

        var converter = TypeDescriptor.GetConverter(typeToConvert);
        return converter.IsValid(value) ? (T?)converter.ConvertFromString(value) : default;
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        var provider = options.GetServiceProvider();
        var cryptoProvider = provider.GetRequiredService<IClientCryptoProvider>();

        var encryptedValue = cryptoProvider.EncryptString(value?.ToString());
        writer.WriteStringValue(encryptedValue);
    }
}
