namespace W2K.Common.Utils;

public static class PhoneUtil
{
    public static string? FromB2CPhoneNumber(this string? phone)
    {
        if (!string.IsNullOrEmpty(phone) && phone.StartsWith("+1", StringComparison.OrdinalIgnoreCase))
        {
            phone = phone[2..].Trim();
            string area = phone[..3];
            string major = phone[3..6];
            string minor = phone[6..];
            return string.Format("({0}) {1}-{2}", area, major, minor);
        }
        return phone;
    }

    public static string? ToB2CPhoneNumber(this string? phone)
    {
        if (!string.IsNullOrEmpty(phone) && phone.StartsWith('('))
        {
            phone = phone.DigitsOnly();
            return string.Format("+1{0}", phone);
        }
        return phone;
    }

    public static string? ToUSPhoneDigitsOnly(this string? phone)
    {
        if (!string.IsNullOrEmpty(phone))
        {
            phone = phone.DigitsOnly();
            return !string.IsNullOrEmpty(phone) && phone.Length <= 10
                ? phone
                : phone?.Substring(1, 10);
        }
        return phone;
    }
}
