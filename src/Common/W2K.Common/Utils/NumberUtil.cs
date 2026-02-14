using System.Security.Cryptography;

namespace W2K.Common.Utils;

public static class NumberUtil
{
    public static int GenerateRandomNumber(int min, int max)
    {
        return RandomNumberGenerator.GetInt32(min, max);
    }

    public static int ToHundredthInt(this decimal amount)
    {
        return (int)(Math.Round(Math.Abs(amount), 2) * 100);
    }
}
