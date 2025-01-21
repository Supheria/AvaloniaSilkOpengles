using System.Globalization;

namespace LocalUtilities.Extensions;

public static class DateTimeFormat
{
    public const string Data = "yyyyMMddHHmmssff";

    public const string Outlook = "yyyy.MM.dd HH:mm:ss.ffffff";

    public static DateTime ToDateTime(this string str, string format)
    {
        _ = DateTime.TryParseExact(str, format, null, DateTimeStyles.None, out var dateTime);
        return dateTime;
    }
}
