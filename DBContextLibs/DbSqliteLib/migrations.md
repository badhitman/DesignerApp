```
Add-Migration MainContext001 -Context MainDbAppContext -Project DbSqliteLib -StartupProject BlankBlazorApp
Update-Database -Context MainDbAppContext -Project DbSqliteLib -StartupProject BlankBlazorApp
```

```
Add-Migration MainStorageContext001 -Context CloudParametersContext -Project DbSqliteLib -StartupProject RemoteCallLib
Update-Database -Context CloudParametersContext -Project DbSqliteLib -StartupProject RemoteCallLib
```

```
Add-Migration MainHelpdeskContext001 -Context HelpdeskContext -Project DbSqliteLib -StartupProject HelpdeskService
Update-Database -Context HelpdeskContext -Project DbSqliteLib -StartupProject HelpdeskService
```

```
Add-Migration MainTelegramBotContext001 -Context TelegramBotContext -Project DbSqliteLib -StartupProject Telegram.Bot.Polling
Update-Database -Context TelegramBotContext -Project DbSqliteLib -StartupProject Telegram.Bot.Polling
```