```
Add-Migration MainPostgreContext002 -Context MainDbAppContext -Project DbPostgreLib -StartupProject BlankBlazorApp
Update-Database -Context MainDbAppContext -Project DbPostgreLib -StartupProject BlankBlazorApp
```

```
Add-Migration StorageContext004 -Context StorageContext -Project DbPostgreLib -StartupProject StorageService
Update-Database -Context StorageContext -Project DbPostgreLib -StartupProject StorageService
```

```
Add-Migration HelpdeskPostgreContext005 -Context HelpdeskContext -Project DbPostgreLib -StartupProject HelpdeskService
Update-Database -Context HelpdeskContext -Project DbPostgreLib -StartupProject HelpdeskService
```

```
Add-Migration TelegramBotContext002 -Context TelegramBotContext -Project DbPostgreLib -StartupProject Telegram.Bot.Polling
Update-Database -Context TelegramBotContext -Project DbPostgreLib -StartupProject Telegram.Bot.Polling
```

```
Add-Migration CommerceContext006 -Context CommerceContext -Project DbPostgreLib -StartupProject CommerceService
Update-Database -Context CommerceContext -Project DbPostgreLib -StartupProject CommerceService
```