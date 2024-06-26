////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// RabbitMQ configuration
/// </summary>
public class RabbitMQConfigModel
{
    /// <inheritdoc/>
    public required string UserName { get; set; } = "guest";

    /// <inheritdoc/>
    public required string Password { get; set; } = "guest";

    /// <inheritdoc/>
    public required string VirtualHost { get; set; } = "/";

    /// <inheritdoc/>
    public required string HostName { get; set; } = "localhost";

    /// <inheritdoc/>
    public required int Port { get; set; } = 5672;

    /// <inheritdoc/>
    public required string ClientProvidedName { get; set; } = "guest-client";

    /// <summary>
    /// Таймаут ожидания ответа на удалённый вызов
    /// </summary>
    public int RemoteCallTimeoutMs { get; set; } = 10000;

    /// <summary>
    /// Префикс имён очередей для ответов на удалённые команды
    /// </summary>
    public string QueueMqNamePrefixForResponse { get; set; } = "response.transit-";
}