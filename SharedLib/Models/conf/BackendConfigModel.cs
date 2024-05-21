////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Конфигурация WEB сервера
/// </summary>
public class BackendConfigModel
{
    /// <summary>
    /// CORS параметры (хосты)
    /// </summary>
    public string[] ClientOrignsCORS { get; set; } = [];

    /// <summary>
    /// Разрешённые хосты запросов к web серверу (* - любые)
    /// </summary>
    public string AllowedHosts { get; set; } = "*";

    /// <summary>
    /// Максимальное количество открытых соединений. Если установлено значение null, количество подключений не ограничено.
    /// </summary>
    public long? MaxConcurrentConnections { get; set; } = 100;

    /// <summary>
    /// Максимально допустимый размер любого тела запроса в байтах. Это ограничение не влияет на обновленные соединения, которые всегда неограниченны.
    /// Это можно переопределить для каждого запроса с помощью IHttpMaxRequestBodySizeFeature. По умолчанию 30 000 000 байт, что составляет примерно 28,6 МБ.
    /// </summary>
    public int MaxRequestBodySize { get; set; } = 30000000;

    /// <summary>
    /// Тайм-аут поддержания активности в секундах. По умолчанию 120 секунд (2 минуты)
    /// </summary>
    public int KeepAliveTimeout { get; set; } = 120;

    /// <summary>
    /// Максимально допустимый размер заголовков HTTP-запроса. По умолчанию 32 768 байт (32 КБ).
    /// </summary>
    public int MaxRequestHeadersTotalSize { get; set; } = 32768;

    /// <summary>
    /// Максимально допустимый размер строки HTTP-запроса. По умолчанию 8192 байта (8 КБ).
    /// </summary>
    public int MaxRequestLineSize { get; set; } = 8192;

    /// <summary>
    /// Порт обслуживающий Kestrel
    /// </summary>
    public int Port { get; set; }
}