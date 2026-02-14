using System.Text.Json.Serialization;

namespace W2K.Common.Crypto;

[AttributeUsage(AttributeTargets.Property)]
public sealed class JsonEncryptedAttribute<T> : JsonConverterAttribute
{
    public JsonEncryptedAttribute() : base(typeof(EncryptionJsonConverter<T>))
    {
    }
}
