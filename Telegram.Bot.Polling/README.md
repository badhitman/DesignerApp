### TelegramBot - служба Worker Service

- Ответ на входящее сообщение из Telegram в бота формирует реализация интерфейса `ITelegramDialogService` [^1].
Бот определяет [имя обработчика ответа] из информации о пользователе (отправителе), а далее по этому имени получает ответ от требуемой реализации обработчика[^4].
Имя обработчика произвольно устанавливается для любого пользователя после чего служба Telegram бота обрабатывает входящие сообщения желаемым методом[^2].
Ответ обработчика отправляется пользователю от имени бота (текст сообщения и кнопки). Если имя обработчика пользователю не установлено или установленное имя не найдено среди реализаций `if (receiveService is null)`, то используется [обработчик по умолчанию *defHandlerType* `Type defHandlerType = typeof(DefaultTelegramDialogHandle)`] [^3], который в данном случае __DefaultTelegramDialogHandle__ который в базовом исполнении только приветствует клиента. + Если пользователь является авторизованным, то даёт возможность отвязать свой аккаунт Telegram от учётной записи на сайте

- У бота зарегистрировано несколько своих команд для обработки запросов от удалённых систем:
```c#
services.RegisterMqttListener<GetBotUsernameReceive, object?, string?>(); // возвращает имя бота
services.RegisterMqttListener<SendTextMessageTelegramReceive, SendTextMessageTelegramBotModel, int?>(); // отправка сообщения в Telegram
services.RegisterMqttListener<SetWebConfigReceive, WebConfigModel, object?>(); // установка настроек web сервиса
```

[^1]: Для создания собственного обработчика диалога подробнее можно узнать [тут](https://github.com/badhitman/DesignerApp/tree/main/ServerLib/Services/TelegramDialog)

[^2]: Имя обработчика ответов храниться в [контексте пользователя](https://github.com/badhitman/DesignerApp/blob/main/SharedLib/Models/TelegramUserBaseModelDb.cs#L45). О хранении имени обработчика в контексте пользователя можно узнать [тут](https://github.com/badhitman/DesignerApp/tree/main/ServerLib/Services/TelegramDialog#контекст-хранения-имени-обработчика-для-пользователя)

[^3]: Обработчик ответов на входящие сообщения в Telegram бота: [базовая реализация](https://github.com/badhitman/DesignerApp/blob/main/ServerLib/Services/TelegramDialog/DefaultTelegramDialogHandle.cs) (по умолчанию).

[^4]: Бот [ищет по имени](https://github.com/badhitman/DesignerApp/blob/main/Telegram.Bot.Polling/Services/UpdateHandler.cs#L155) нужного обработчика. Если не находит, то [использует базовый](https://github.com/badhitman/DesignerApp/blob/main/Telegram.Bot.Polling/Services/UpdateHandler.cs#L159).