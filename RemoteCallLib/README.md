## RabbitMQ транспорт (+ общие/разделяемые параметры)

Транспорт устроен просто. Сначала нужно определить тип параметра для вызова команды (объект будет отправлен серверу как *payload* запроса), а так же тип возвращаемого ответа. Далее нужно реализовать единственный метод интерфейса [IResponseReceive](https://github.com/badhitman/DesignerApp/blob/main/RemoteCallLib/base/IResponseReceive.cs) с указанием этих самых типов запроса/ответа. Как пример вот как это выглядит в базовых реализациях:

> [!WARNING]
> Кроме того нужно зарезервировать имена MQ очередей. В данном случае они сгруппированы в `public static class TransmissionQueues` [^2]

- TelegramBot
```c#
public class UpdateTelegramUserReceive(ITelegramWebService tgWebRepo, ILogger<UpdateTelegramUserReceive> _logger)
    : IResponseReceive<CheckTelegramUserHandleModel?, CheckTelegramUserModel?>
{
  public static string QueueName => GlobalStaticConstants.TransmissionQueues.UpdateTelegramUserReceive;
  public async Task<TResponseModel<CheckTelegramUserModel?>> ResponseHandleAction(CheckTelegramUserHandleModel? user)
  {
    ... код обработчика ...
  }
}
public class UpdateTelegramMainUserMessageReceive(ITelegramWebService tgWebRepo, ILogger<UpdateTelegramMainUserMessageReceive> _logger)
    : IResponseReceive<MainUserMessageModel?, object?>
{
  public static string QueueName => GlobalStaticConstants.TransmissionQueues.UpdateTelegramMainUserMessageReceive;
  public async Task<TResponseModel<object?>> ResponseHandleAction(MainUserMessageModel? setMainMessage)
  {
    ... код обработчика ...
  }
}
public class TelegramJoinAccountDeleteReceive(ITelegramWebService tgWebRepo, ILogger<TelegramJoinAccountDeleteReceive> _logger) 
    : IResponseReceive<long, object?>
{
  public static string QueueName => GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountDeleteReceive;
  public async Task<TResponseModel<object?>> ResponseHandleAction(long payload)
  {
    ... код обработчика ...
  }
}
public class TelegramJoinAccountConfirmReceive(ITelegramWebService tgWebRepo, ILogger<TelegramJoinAccountConfirmReceive> _logger)
    : IResponseReceive<TelegramJoinAccountConfirmModel?, object?>
{
  public static string QueueName => GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountConfirmReceive;
  public async Task<TResponseModel<object?>> ResponseHandleAction(TelegramJoinAccountConfirmModel? confirm)
  {
    ... код обработчика ...
  }
}
public class GetWebConfigReceive(IOptions<WebConfigModel> webConfig)
    : IResponseReceive<object?, WebConfigModel?>
{
  public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetWebConfigReceive;
  public Task<TResponseModel<WebConfigModel?>> ResponseHandleAction(object? payload = null)
  {
    ... код обработчика ...
  }
}
public class GetTelegramUserReceive(ITelegramWebService tgWebRepo, ILogger<GetTelegramUserReceive> _logger)
    : IResponseReceive<long, TelegramUserBaseModelDb?>
{
  public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetTelegramUserReceive;
  public async Task<TResponseModel<TelegramUserBaseModelDb?>> ResponseHandleAction(long payload)
  {
    ... код обработчика ...
  }
}
```

- BlazorWebApp
```c#
public class SetWebConfigReceive(WebConfigModel webConfig, ILogger<SetWebConfigReceive> _logger)
    : IResponseReceive<WebConfigModel?, object?>
{
  public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetWebConfigReceive;
  public Task<TResponseModel<object?>> ResponseHandleAction(WebConfigModel? payload)
  {
    ... код обработчика ...
  }
}
public class SendTextMessageTelegramReceive(ITelegramBotClient _botClient, IWebRemoteTransmissionService webRemoteCall, ILogger<SendTextMessageTelegramReceive> _logger) 
    : IResponseReceive<SendTextMessageTelegramBotModel?, int?>
{
  public static string QueueName => GlobalStaticConstants.TransmissionQueues.SendTextMessageTelegramReceive;
  public async Task<TResponseModel<int?>> ResponseHandleAction(SendTextMessageTelegramBotModel? message)
  {
    ... код обработчика ...
  }
}
public class GetBotUsernameReceive(ITelegramBotClient _botClient, ILogger<GetBotUsernameReceive> _logger)
    : IResponseReceive<object?, string?>
{
  public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetBotUsernameReceive;
  public async Task<TResponseModel<string?>> ResponseHandleAction(object? payload = null)
  {
    ... код обработчика ...
  }
}
```

Как видно в каждом случае наследуется интерфейс `IResponseReceive` с указанием типов для запроса/TRequest и ответа/TResponse.
Под эти типы данных формируется требуемая сигнатура метода `Task<TResponseModel<TResponse?>> ResponseHandleAction(TRequest? payload)`. Обратите внимание, что тип возвращаемых данных в конечном итоге будет не `TResponse`, а  `TResponseModel<TResponse?>`.
Там где не ожидается полезной нагрузки запрос используется `object?`, что бы в реализациях задействовать подобную заглушку `object? payload = null`. А для возвращаемых типов от `object?` получается `TResponseModel<object?>`, который по меньшей мере несёт в себе данные по ошибкам, предупреждениям или информирование.

После того как ваш обработчик готов его можно регистрировать в службе: в данном случае есть обработчики команд в обоих службах и каждый в свою очередь регистрирует свой набор:
- TelegramBot
```c#
services.RegisterMqListener<GetBotUsernameReceive, object?, string?>();
services.RegisterMqListener<SendTextMessageTelegramReceive, SendTextMessageTelegramBotModel, int?>();
services.RegisterMqListener<SetWebConfigReceive, WebConfigModel, object?>();
```

- BlazorWebApp
```c#
builder.Services.RegisterMqListener<UpdateTelegramUserReceive, CheckTelegramUserHandleModel, CheckTelegramUserModel?>();
builder.Services.RegisterMqListener<TelegramJoinAccountConfirmReceive, TelegramJoinAccountConfirmModel, object?>();
builder.Services.RegisterMqListener<TelegramJoinAccountDeleteReceive, long, object?>();
builder.Services.RegisterMqListener<GetWebConfigReceive, object?, WebConfigModel>();
builder.Services.RegisterMqListener<UpdateTelegramMainUserMessageReceive, MainUserMessageModel, object?>();
builder.Services.RegisterMqListener<GetTelegramUserReceive, long, TelegramUserBaseModelDb>();
```

Этого достаточно, что бы ответственная служба начала обрабатывать входящие команды и отвечать на них. Теперь нужен клиент, который зарегистрирован в DI так `<IRabbitClient, RabbitClient>`. Т.е. обращение к клиенту доступен через `IRabbitClient`. У службы единственный обобщённый метод `MqRemoteCall`, которому нужно указать тип возвращаемых данных и передать объект полезной нагрузки запроса для отправке серверу в виде параметра вызова. Для базовых команд эти вызовы сгруппированы в двух разных сервисах:
#### TelegramBot [TransmissionTelegramService](https://github.com/badhitman/DesignerApp/blob/main/RemoteCallLib/TransmissionTelegramService.cs)
```C#
/// <summary>
/// Удалённый вызов команд в TelegramBot службе
/// </summary>
public class TransmissionTelegramService(IRabbitClient rabbitClient) : ITelegramRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GetBotUsername()
        => await rabbitClient.MqRemoteCall<string?>(GlobalStaticConstants.TransmissionQueues.GetBotUsernameReceive);

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> SendTextMessageTelegram(SendTextMessageTelegramBotModel message_telegram)
        => await rabbitClient.MqRemoteCall<int?>(GlobalStaticConstants.TransmissionQueues.SendTextMessageTelegramReceive, message_telegram);

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> SetWebConfig(WebConfigModel webConf)
        => await rabbitClient.MqRemoteCall<object?>(GlobalStaticConstants.TransmissionQueues.SetWebConfigReceive, webConf);
}
```

#### BlazorWebApp [TransmissionWebService](https://github.com/badhitman/DesignerApp/blob/main/RemoteCallLib/TransmissionWebService.cs)
```c#
/// <summary>
/// Удалённый вызов команд в Web службе
/// </summary>
public class TransmissionWebService(IRabbitClient rabbitClient) : IWebRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> TelegramJoinAccountConfirmToken(TelegramJoinAccountConfirmModel req)
        => await rabbitClient.MqRemoteCall<object?>(GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountConfirmReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<CheckTelegramUserModel?>> CheckTelegramUser(CheckTelegramUserHandleModel user)
        => await rabbitClient.MqRemoteCall<CheckTelegramUserModel?>(GlobalStaticConstants.TransmissionQueues.UpdateTelegramUserReceive, user);

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> TelegramJoinAccountDelete(long telegramId)
        => await rabbitClient.MqRemoteCall<object?>(GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountDeleteReceive, telegramId);

    /// <inheritdoc/>
    public async Task<TResponseModel<WebConfigModel?>> GetWebConfig()
        => await rabbitClient.MqRemoteCall<WebConfigModel?>(GlobalStaticConstants.TransmissionQueues.GetWebConfigReceive);

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> UpdateTelegramMainUserMessage(MainUserMessageModel setMainMessage)
        => await rabbitClient.MqRemoteCall<object?>(GlobalStaticConstants.TransmissionQueues.UpdateTelegramMainUserMessageReceive, setMainMessage);

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramUserBaseModelDb?>> GetTelegramUser(long telegramUserId)
        => await rabbitClient.MqRemoteCall<TelegramUserBaseModelDb?>(GlobalStaticConstants.TransmissionQueues.GetTelegramUserReceive, telegramUserId);
}
```

Видно, что для вызова удалённой команды нужно указать типы данных запроса/ответа и указать имя очереди обработчика.
Теперь клиент с сервером готовы обмениваться командами и ответами на них.
Что бы получить ответ на запрос от удалённой системы вызывающий клиент `<IRabbitClient, RabbitClient>` отправляемому сообщению [указывает](https://github.com/badhitman/DesignerApp/blob/main/RemoteCallLib/base/RabbitClient.cs#L56) [имя очереди в которой ожидается ответ](https://github.com/badhitman/DesignerApp/blob/main/RemoteCallLib/base/RabbitClient.cs#L49). Имя этой очереди формируется по шаблону:
```c#
string response_topic = $"{RabbitConfigRepo.QueueMqNamePrefixForResponse}{queue}_{Guid.NewGuid()}";
```
Где `RabbitConfigRepo.QueueMqNamePrefixForResponse` - префикс имени очереди из конфигов (*по умолчанию*: **response.transit-**), `queue` - исходное имя очереди и GUID для контроля уникальности.
Вот на эту временную очередь отправитель ожидает ответ. Пример:
```
response.transit-Transmission.Receives\web\configuration\read_c68fbf43-0229-4df7-9d48-6bdc8a9384ef
```

#### Общие/разделяемые параметры
На базе этого же решения RabbitMQ транспорта была развёрнута система общих/разделяемых параметров. Благодаря ей можно хранить и читать параметры разным сервисам обращаясь к обще службе, которая автономно хранит все данные в своей БД. Хранить можно любые `сериализуемые` данные.
```c#
builder.Services.AddScoped<ISerializeStorageRemoteTransmissionService, SerializeStorageRemoteTransmissionService>();
```

[^1]: С примерами реализаций можно ознакомиться на командах, которые были реализованы в рамках данного решения. Несколько команд есть для [Telegram бота](./Receives/telegram) и некоторое количество сделано для [BlazorWebApp](./Receives/web) службы

[^2]: При реализации интерфейса `IResponseReceive` статическое свойство `public static string QueueName => ` определяет какую MQ очередь будет обслуживать данный обработчик. Вызывающий сервис должен отправлять сообщения в соответствующие очереди (указывая при этом адрес для ответа в MQ заголовок [ReplyTo](https://github.com/badhitman/DesignerApp/blob/main/RemoteCallLib/base/RabbitClient.cs#L56)), что бы успешно получать [обратную связь](https://github.com/badhitman/DesignerApp/blob/main/RemoteCallLib/base/RabbitMqListenerService.cs#L88). Реализация базовых инструментов: отправка команд в сторону [Telegram бота](./RemoteCallLib#telegrambot-transmissiontelegramservice) и сервера [BlazorWebApp](./RemoteCallLib#blazorwebapp-transmissionwebservice). Зарезервированные имена MQ очередей размещена в своём отдельном [public static class TransmissionQueues](https://github.com/badhitman/DesignerApp/blob/main/SharedLib/GlobalStaticConstants.cs#L59)