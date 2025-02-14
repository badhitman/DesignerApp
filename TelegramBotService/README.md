### TelegramBot - служба Worker Service

- Ответ на входящее сообщение из Telegram в бота формирует реализация интерфейса `ITelegramDialogService`.
Бот определяет [имя обработчика ответа] из информации о пользователе (отправителе), а далее по этому имени получает ответ от требуемой реализации обработчика[^1].
Имя обработчика произвольно устанавливается для любого пользователя после чего служба Telegram бота обрабатывает входящие сообщения желаемым методом[^2].
Ответ обработчика отправляется пользователю от имени бота (текст сообщения и кнопки). Если имя обработчика пользователю не установлено или установленное имя не найдено среди реализаций `if (receiveService is null)`, то используется [обработчик по умолчанию *defHandlerType* `Type defHandlerType = typeof(DefaultTelegramDialogHandle)`] [^3], который в данном случае __DefaultTelegramDialogHandle__ который в базовом исполнении только приветствует клиента. + Если пользователь является авторизованным, то даёт возможность отвязать свой аккаунт Telegram от учётной записи на сайте

- У бота зарегистрировано несколько своих команд для обработки запросов от удалённых систем:
```c#
services.RegisterMqListener<GetBotUsernameReceive, object?, string?>(); // возвращает имя бота
services.RegisterMqListener<SendTextMessageTelegramReceive, SendTextMessageTelegramBotModel, int?>(); // отправка сообщения в Telegram
services.RegisterMqListener<SetWebConfigReceive, WebConfigModel, object?>(); // установка настроек web сервиса
```

### TelegramBot диалоги

Сообщения в Telegram бота от пользователей должны обрабатываться сервисом, унаследованном от `ITelegramDialogService`.
В базовой комплектации существует простая реализация `DefaultTelegramDialogHandle`. Она только приветствует клиента, а зарегистрированным клиентам (тем которые привязали Telegram аккаунт к учётной записи сайта) даёт возможность удалить эту авторизацию/связь.
На данный момент в обработку попадают `Message` и `CallbackQuery` сообщения. Оба обрабатываются идентично.

Для каждого пользователя Telegram в БД устанавливается имя реализации, который должен обрабатывать входящие сообщения.
Что бы добавить свой обработчик достаточно наследовать интерфейс `ITelegramDialogService`,  реализовать его
```C#
/// <summary>
/// TelegramDialog
/// </summary>
public interface ITelegramDialogService
{
  /// <summary>
  /// Обработка входящих сообщений из Telegram
  /// </summary>
  public Task<TelegramDialogResponseModel> TelegramDialogHandle(TelegramDialogRequestModel tgDialog);
}
```
и зарегистрировать это в DI службы `TelegramBotService`:
```c#
services.AddScoped<ITelegramDialogService, DefaultTelegramDialogHandle>();
```

#### Контекст хранения имени обработчика для пользователя
После этого объекту БД `TelegramUserModelDb` можно устанавливать имя этого обработчика.
```c#
/// <summary>
/// Тип диалога (имя реализации). Обработчик ответа на входящее сообщение Telegram
/// </summary>
public string? DialogTelegramTypeHandler { get; set; }
```
(*контекст*: `MainDbAppContext`)
```c#
/// <summary>
/// Telegram пользователи
/// </summary>
public DbSet<TelegramUserModelDb> TelegramUsers { get; set; }
```

#### Использование
Telegram бот в свою очередь попытается найти реализацию интерфейса `ITelegramDialogService` с таким именем:
```c#
ITelegramDialogService? receiveService;
using (IServiceScope scope = servicesProvider.CreateScope())
{
    receiveService = scope.ServiceProvider
        .GetServices<ITelegramDialogService>()
        .FirstOrDefault(o => o.GetType().FullName == uc.TelegramUser.DialogTelegramTypeHandler);

    if (receiveService is null)
    {
        if (!string.IsNullOrWhiteSpace(uc.TelegramUser.DialogTelegramTypeHandler))
            logger.LogError($"Ошибка в имени {nameof(uc.TelegramUser.DialogTelegramTypeHandler)}: {uc.TelegramUser.DialogTelegramTypeHandler}. error {{DCAA97B4-1AC6-45F4-84C1-48DF5464E55E}}");

        receiveService = scope.ServiceProvider
            .GetServices<ITelegramDialogService>()
            .First(o => o.GetType() == defHandlerType);
    }
}
```
В итоге пользователь получит нужный ответ.
```c#
TelegramDialogResponseModel resp = await receiveService.TelegramDialogHandle(new TelegramDialogRequestModel()
    {
        MessageText = messageText,
        MessageTelegramId = message.MessageId,
        TelegramUser = uc.TelegramUser,
        TypeMessage = MessagesTypesEnum.TextMessage,
        // или TypeMessage = MessagesTypesEnum.CallbackQuery,
    });
```

[^1]: Бот [ищет по имени](https://github.com/badhitman/BlankCRM/blob/main/TelegramBotService/Services/UpdateHandler.cs#L155) нужного обработчика. Если не находит, то [использует базовый](https://github.com/badhitman/BlankCRM/blob/main/TelegramBotService/Services/UpdateHandler.cs#L159)

[^2]: Имя обработчика ответов храниться в [контексте пользователя](https://github.com/badhitman/BlankCRM/blob/main/SharedLib/Models/TelegramUserBaseModelDb.cs#L45)

[^3]: Обработчик ответов на входящие сообщения в Telegram бота: [базовая реализация](https://github.com/badhitman/BlankCRM/blob/main/TelegramBotService/Services/DefaultTelegramDialogHandle.cs) (по умолчанию)