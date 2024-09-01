```
Add-Migration MainPostgreContext001 -Context MainDbAppContext -Project DbPostgreLib -StartupProject BlankBlazorApp
Update-Database -Context MainDbAppContext -Project DbPostgreLib -StartupProject BlankBlazorApp
```

```
Add-Migration MainStorageContext001 -Context CloudParametersContext -Project DbPostgreLib -StartupProject RemoteCallLib
Update-Database -Context CloudParametersContext -Project DbPostgreLib -StartupProject RemoteCallLib
```

```
Add-Migration MainHelpdeskPostgreContext001 -Context HelpdeskContext -Project DbPostgreLib -StartupProject HelpdeskService
Update-Database -Context HelpdeskContext -Project DbPostgreLib -StartupProject HelpdeskService
```

```
Add-Migration MainTelegramBotContext001 -Context TelegramBotContext -Project DbPostgreLib -StartupProject Telegram.Bot.Polling
Update-Database -Context TelegramBotContext -Project DbPostgreLib -StartupProject Telegram.Bot.Polling
```

```
Add-Migration MainCommerceContext001 -Context CommerceContext -Project DbPostgreLib -StartupProject CommerceService
Update-Database -Context CommerceContext -Project DbPostgreLib -StartupProject CommerceService
```