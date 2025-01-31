```
Add-Migration MainContext001 -Context MainDbAppContext -Project DbSqliteLib -StartupProject BlankBlazorApp
Update-Database -Context MainDbAppContext -Project DbSqliteLib -StartupProject BlankBlazorApp
```

```
Add-Migration StorageContext001 -Context StorageContext -Project DbSqliteLib -StartupProject StorageService
Update-Database -Context StorageContext -Project DbSqliteLib -StartupProject StorageService
```

```
Add-Migration NLogsContext001 -Context NLogsContext -Project DbSqliteLib -StartupProject StorageService
Update-Database -Context NLogsContext -Project DbSqliteLib -StartupProject StorageService
```

```
Add-Migration HelpdeskContext001 -Context HelpdeskContext -Project DbSqliteLib -StartupProject HelpdeskService
Update-Database -Context HelpdeskContext -Project DbSqliteLib -StartupProject HelpdeskService
```

```
Add-Migration TelegramBotContext001 -Context TelegramBotContext -Project DbSqliteLib -StartupProject TelegramBotService
Update-Database -Context TelegramBotContext -Project DbSqliteLib -StartupProject TelegramBotService
```

```
Add-Migration CommerceContext001 -Context CommerceContext -Project DbSqliteLib -StartupProject CommerceService
Update-Database -Context CommerceContext -Project DbSqliteLib -StartupProject CommerceService
```