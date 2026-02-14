namespace DFI.Common.Utils;

/// <summary>
/// Utility class for DateTime operations and timezone conversions.
/// </summary>
public static class DateTimeUtil
{
    /// <summary>
    /// Converts a UTC DateTime to Eastern Time (EST/EDT).
    /// </summary>
    /// <param name="utcDateTime">The UTC DateTime to convert.</param>
    /// <returns>The DateTime converted to Eastern Time.</returns>
    public static DateTime ToEasternTime(this DateTime utcDateTime)
    {
        var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, easternTimeZone);
    }

    /// <summary>
    /// Converts a string representation of years into the equivalent number of months.
    /// </summary>
    /// <param name="years"></param>
    /// <returns></returns>
    public static int ToMonths(this string years)
    {
        return int.TryParse(years, out int intyears) ? (intyears * 12) : 0;
    }

    /// <summary>
    /// Converts a nullable DateOnly to a nullable DateTime with UTC kind at midnight.
    /// </summary>
    /// <param name="dateOnly">The DateOnly value to convert.</param>
    /// <param name="timeOnly">The TimeOnly value to use for the time component. default is TimeOnly.MinValue (00:00:00).</param>
    /// <returns>A DateTime with UTC kind at midnight if the value is not null; otherwise, null.</returns>
    public static DateTime? ToDateTimeUtc(this DateOnly? dateOnly, TimeOnly timeOnly = default)
    {
        return dateOnly?.ToDateTime(timeOnly, DateTimeKind.Utc);
    }

    /// <summary>
    /// Converts a nullable DateTime to a nullable DateOnly.
    /// </summary>
    /// <param name="dateTime">The nullable DateTime to convert.</param>
    /// <returns>A DateOnly if the DateTime has a value; otherwise, null.</returns>
    public static DateOnly? ToDateOnly(this DateTime? dateTime)
    {
        return dateTime.HasValue ? DateOnly.FromDateTime(dateTime.Value) : null;
    }

    /// <summary>
    /// Converts a DateTime to a DateOnly.
    /// </summary>
    /// <param name="dateTime">The DateTime to convert.</param>
    /// <returns>A DateOnly representing the date portion of the DateTime.</returns>
    public static DateOnly ToDateOnly(this DateTime dateTime)
    {
        return DateOnly.FromDateTime(dateTime);
    }
}
