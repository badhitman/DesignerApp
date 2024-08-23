```
Add-Migration MainMySQLContext001 -Context MainDbAppContext -Project DbMySQLLib -StartupProject BlankBlazorApp
Update-Database -Context MainDbAppContext -Project DbMySQLLib -StartupProject BlankBlazorApp
```

```
Add-Migration MainStorageContext001 -Context CloudParametersContext -Project DbMySQLLib -StartupProject RemoteCallLib
Update-Database -Context CloudParametersContext -Project DbMySQLLib -StartupProject RemoteCallLib
```

```
Add-Migration MainHelpdeskMySQLContext001 -Context HelpdeskContext -Project DbMySQLLib -StartupProject HelpdeskService
Update-Database -Context HelpdeskContext -Project DbMySQLLib -StartupProject HelpdeskService
```

```
Add-Migration MainTelegramBotContext001 -Context TelegramBotContext -Project DbMySQLLib -StartupProject Telegram.Bot.Polling
Update-Database -Context TelegramBotContext -Project DbMySQLLib -StartupProject Telegram.Bot.Polling
```