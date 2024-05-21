## TelegramBot диалоги

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
и зарегистрировать это в DI службы `Telegram.Bot.Polling`:
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