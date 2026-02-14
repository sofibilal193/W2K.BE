using ProtoBuf;

namespace DFI.Common.Utils;

public static class SerializeUtil
{
    public static byte[]? ToByteArray(this object obj)
    {
        if (obj is null)
        {
            return null;
        }

        using var memoryStream = new MemoryStream();
        Serializer.Serialize(memoryStream, obj);
        return memoryStream.ToArray();
    }

    public static T? FromByteArray<T>(this byte[] byteArray)
    {
        if (byteArray is null)
        {
            return default;
        }

        using var memoryStream = new MemoryStream();
        // Ensure that our stream is at the beginning.
        memoryStream.Write(byteArray, 0, byteArray.Length);
        _ = memoryStream.Seek(0, SeekOrigin.Begin);

        var val = Serializer.Deserialize<T>(memoryStream);

        if (val is T t)
        {
            return t;
        }
        try
        {
            return (T?)Convert.ChangeType(val, typeof(T));
        }
        catch (InvalidCastException)
        {
            return default;
        }
    }
}
