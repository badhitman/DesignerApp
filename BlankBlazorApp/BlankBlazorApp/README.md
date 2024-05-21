## Blazor WebApp служба

### Пользователи

Добавлен минимальный набор инструментов управления ролями пользователей

![img](../../img/users-profiles.png)

можно отправить сообщение пользователю

![img](../../img/users-profiles-tg-button.png)

клиенты Telegram

![img](../../img/users-profiles-tg-clients.png)

#### Привязка и отключение Telegram с учётной записью сайта
В `ApplicationUser : IdentityUser` есть свойство:
```c#
/// <summary>
/// Telegram Id
/// </summary>
public long? TelegramId { get; set; }
```
это значение указывает на связь с аккаунтом Telegram. Информацию по самим пользователям Telegram можно получить из контекста `MainDbAppContext`. В таблице `TelegramUsers`
```c#
/// <summary>
/// Telegram пользователи
/// </summary>
public DbSet<TelegramUserModelDb> TelegramUsers { get; set; }
```
по свойству:
```c#
/// <summary>
/// Уникальный идентификатор Telegram пользователя (или бота)
/// </summary>
public long TelegramId { get; set; }
```
можно получить данные, которые проверяются/кэшируются в БД при каждом входящем запросе в TelegramBot.

Постороннему пользователю в данном случае отвечает простым приветсвием: как это реализовано в базовом `ITelegramDialogService: DefaultTelegramDialogHandle`

![img](../../img/tg-not-auth.png)

Управление привязкой Telegram аккаунта к своей учётной записи возможно из личного раздела `/Account/Manage/Telegram`. Пользователь может создать временную одноразовую ссылку/код для подтверждения связи через свой же TelegramBot. Кнопка "Создать токен":

![img](../../img/tg-join-null.png)

Создать новый токен можно и после привязки Telegram к у/з, но каждое новое подтверждение Telegram аккаунта перезаписывает прежнюю связь. Подключение одновременно нескольких Telegram аккаунтов не поддерживается.
Созданный токен можно использовать как ссылку напрямую в Telegram бота

![img](../../img/tg-auth-btn-init.png)

, а можно отправить боту токен (guid) как текст.

![img](../../img/tg-join-init.png)

Если токен существует и не просрочился, то устанавливается связь между Telegram аккаунтом и учётной записью на сайте:

![img](../../img/tg-join-success.png)

#### Синхронизация настроек между службами BlazorWebApp и Telegram.Bot.Polling.
При каждом запуске каждой из служб происходит попытка синхронизации настроек `WebConfigModel`. В `BlazorWebApp` они существуют как типовая опция IOptions<WebConfigModel>
```c#
builder.Services.Configure<WebConfigModel>(builder.Configuration.GetSection("WebConfig"));
```
, а в службе `Telegram.Bot.Polling` этот объект существует в виде Singleton объекта
```c#
services.AddSingleton<WebConfigModel>();
```

Дело в том, что сама `Telegram.Bot.Polling` не конфигурирует эти параметры, а запрашивает их при старте от `BlazorWebApp`. В свою очередь `BlazorWebApp` при запуске отправляет эти конфиги в сторону `Telegram.Bot.Polling`. В конечном итоге обеспечивается своевременная актуализация этих настроек, которые конфигурируются в одном месте на стороне `BlazorWebApp` службы, а вторая служба их получает так или иначе. В этих настройках в частности передаётся адрес сайта Web части и срок действия токена привязки Telegram аккаунта к учётной записи сайта.