////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Расширение DateTime
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Создает новый объект System.DateTime, который имеет то же количество тактов,
    /// что и указанный System.DateTime, но обозначается как местное время, всеобщее
    /// координированное время (UTC) или ни то, ни другое, как указано указанным значением System.DateTimeKind.
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