namespace DFI.Common.Application.Validations;

public static class StringValidators
{
    /// <summary>
    /// Check whether the routing number is valid or not.
    /// </summary>
    public static bool ValidRoutingNumber(this string? routingNumber)
    {
        if (string.IsNullOrEmpty(routingNumber) || routingNumber.Length != 9 || !routingNumber.All(char.IsDigit))
        {
            return false;
        }

        int[] weights = [3, 7, 1];
        int checksum = 0;

        for (int i = 0; i < routingNumber.Length; i++)
        {
            checksum += (routingNumber[i] - '0') * weights[i % 3];
        }

        return checksum % 10 == 0;
    }
}
