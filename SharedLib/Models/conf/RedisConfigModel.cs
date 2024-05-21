////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Конфигурация Redis
/// </summary>
public class RedisConfigModel
{
    /// <summary>
    /// ENG:
    /// Indicates whether endpoints should be resolved via DNS before connecting.
    /// If enabled the ConnectionMultiplexer will not re-resolve DNS when attempting to
    /// re-connect after a connection failure.
    /// RUS:
    /// Указывает, должны ли конечные точки разрешаться через DNS перед подключением. 
    /// Если этот параметр включен, ConnectionMultiplexer не будет повторно разрешать DNS при попытке повторного подключения после сбоя подключения.
    /// </summary>
    public bool ResolveDns { get; set; } = false;

    /// <summary>
    /// Пароль для аутентификации на сервере.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Пользователь, который будет использоваться для аутентификации на сервере.
    /// </summary>
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// Specifies the time in seconds at which connections should be pinged to ensure validity
    /// </summary>
    public int KeepAlive { get; set; } = 5;

    /// <summary>
    /// Конечные точки, определенные для этой конфигурации
    /// </summary>
    public string EndPoint { get; set; } = "localhost:6379";

    /// <summary>
    /// Возвращает или задает, следует ли явным образом уведомлять о тайм-аутах подключения / конфигурации с помощью TimeoutException.
    /// </summary>
    public bool AbortOnConnectFail { get; set; } = false;

    /// <summary>
    /// Указывает время в миллисекундах, которое должно быть разрешено для подключения (по умолчанию 5 секунд, если SyncTimeout не больше)
    /// </summary>
    public int ConnectTimeout { get; set; } = 10000;

    /// <summary>
    /// Канал, используемый для трансляции и прослушивания уведомлений об изменении конфигурации
    /// </summary>
    public string ConfigurationChannel { get; set; } = string.Empty;

    /// <summary>
    /// Сколько раз повторять начальный цикл подключения, если ни один сервер не отвечает незамедлительно.
    /// </summary>
    public int ConnectRetry { get; set; } = 5;

    /// <summary>
    /// Имя клиента, которое будет использоваться для всех подключений.
    /// </summary>
    public string ClientName { get; set; } = string.Empty;

    /// <summary>
    /// Задает время в миллисекундах, в течение которого система должна разрешать асинхронные операции (по умолчанию SyncTimeout)
    /// </summary>
    public int AsyncTimeout { get; set; } = 10000;

    /// <summary>
    /// Указывает, должны ли быть разрешены административные операции
    /// </summary>
    public bool AllowAdmin { get; set; } = true;

    /// <summary>
    /// Указывает, следует ли зашифровать соединение
    /// </summary>
    public bool Ssl { get; set; } = false;

    /// <summary>
    /// Целевой хост для использования при проверке сертификата SSL; установка значения здесь включает режим SSL
    /// </summary>
    public string SslHost { get; set; } = string.Empty;

    /// <summary>
    /// Задает время в миллисекундах, в течение которого система должна разрешать синхронные операции (по умолчанию 5 секунд)
    /// </summary>
    public int SyncTimeout { get; set; } = 10000;
}