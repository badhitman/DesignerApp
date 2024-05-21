////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Конфигурация хоста
/// </summary>
public class HostConfigModel
{
    /// <summary>
    /// Хост
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Порт (tcpip)
    /// </summary>
    public int Port { get; set; } = 5501;

    /// <summary>
    /// Схема (https/ssl)
    /// </summary>
    public string HttpSheme { get; set; } = "http";

    /// <summary>
    /// Uri конфигурации хоста
    /// </summary>
    public Uri Url
    {
        get
        {
            string url = $"{HttpSheme}://{Host}";

            if (HttpSheme.Equals("http", StringComparison.CurrentCultureIgnoreCase) && Port == 80)
            {

            }
            else if (HttpSheme.Equals("https", StringComparison.CurrentCultureIgnoreCase) && Port == 443)
            {

            }
            else
            {
                url += $":{Port}";
            }
            url += "/";
            return new Uri(url);
        }
    }

    /// <summary>
    /// Преобразовать в строку конфигурации хоста
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{HttpSheme}://{Host}{(Port == 80 ? string.Empty : $":{Port}")}";

    /// <summary>
    /// Получить полный URL
    /// </summary>
    /// <param name="path">Путь (относительный)</param>
    /// <returns>Строка полного URL пути</returns>
    public string GetFullUrl(string path) => $"{ToString()}/{(path.StartsWith("/") ? path[1..] : path)}";
}