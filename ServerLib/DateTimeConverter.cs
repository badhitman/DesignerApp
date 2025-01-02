using Newtonsoft.Json.Converters;

namespace ServerLib;

/// <summary>
/// DateTimeConverter
/// </summary>
public class DateTimeConverter : IsoDateTimeConverter
{
    /// <summary>
    /// 
    /// </summary>
    public DateTimeConverter() => DateTimeFormat = "dd.MM.yyyy";
}
