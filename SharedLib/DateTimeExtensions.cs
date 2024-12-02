////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.Globalization;

namespace SharedLib;

/// <summary>
/// Расширение DateTime
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// GetCustomTime
    /// </summary>
    public static DateTime GetCustomTime(this DateTime dateTime, string timeZone = "Europe/Moscow")
        => TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(timeZone));

    static CultureInfo cultureInfo = new CultureInfo("ru-RU");
    /// <summary>
    /// Дата + время
    /// </summary>
    public static string GetHumanDateTime(this DateTime dateTime, string timeZone = "Europe/Moscow")
    {
        DateTime _cdt = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(timeZone));
        return $"{_cdt.ToString("d", cultureInfo)} {_cdt.ToString("t", cultureInfo)}";
    }


    /// <summary>
    /// Создает новый объект System.DateTime, который имеет то же количество тактов,
    /// что и указанный System.DateTime, но обозначается как местное время, всеобщее
    /// координированное время (UTC) или ни то, ни другое, как указано значением System.DateTimeKind.
    /// </summary>
    public static DateTime? SetKindUtc(this DateTime? dateTime)
    {
        if (dateTime.HasValue)
        {
            return dateTime.Value.SetKindUtc();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Создает новый объект System.DateTime, который имеет то же количество тактов,
    /// что и указанный System.DateTime, но обозначается как местное время, всеобщее
    /// координированное время (UTC) или ни то, ни другое, как указано указанным значением System.DateTimeKind.
    /// </summary>
    public static DateTime SetKindUtc(this DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc) { return dateTime; }
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }
}