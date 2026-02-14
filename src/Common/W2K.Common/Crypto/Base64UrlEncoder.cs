using System.Text;

namespace DFI.Common.Crypto;

public static class Base64UrlEncoder
{
    public static string Encode(string value)
    {
        return EncodeFromBytes(Encoding.UTF8.GetBytes(value));
    }

    public static string EncodeFromBytes(byte[] inArray)
    {
        var base64 = Convert.ToBase64String(inArray);
        return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    public static string Decode(string base64String)
    {
        return Encoding.UTF8.GetString(DecodeToBytes(base64String));
    }

    public static byte[] DecodeToBytes(string base64String)
    {
        base64String = base64String.Replace('-', '+').Replace('_', '/');
        var mod = base64String.Length % 4;
        if (mod > 1)
        {
            base64String += new string('=', 4 - mod);
        }
        return Convert.FromBase64String(base64String);
    }
}
