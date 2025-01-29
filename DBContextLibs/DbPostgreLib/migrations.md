```
Add-Migration MainPostgreContext002 -Context MainDbAppContext -Project DbPostgreLib -StartupProject BlankBlazorApp
Update-Database -Context MainDbAppContext -Project DbPostgreLib -StartupProject BlankBlazorApp
```

```
Add-Migration StorageContext015 -Context StorageContext -Project DbPostgreLib -StartupProject StorageService
Update-Database -Context StorageContext -Project DbPostgreLib -StartupProject StorageService
```

```
Add-Migration HelpdeskPostgreContext014 -Context HelpdeskContext -Project DbPostgreLib -StartupProject HelpdeskService
Update-Database -Context HelpdeskContext -Project DbPostgreLib -StartupProject HelpdeskService
```

```
Add-Migration TelegramBotContext005 -Context TelegramBotContext -Project DbPostgreLib -StartupProject TelegramBotService
Update-Database -Context TelegramBotContext -Project DbPostgreLib -StartupProject TelegramBotService
```

```
Add-Migration CommerceContext037 -Context CommerceContext -Project DbPostgreLib -StartupProject CommerceService
Update-Database -Context CommerceContext -Project DbPostgreLib -StartupProject CommerceService
```

```
Add-Migration ConstructorContext004 -Context ConstructorContext -Project DbPostgreLib -StartupProject ConstructorService
Update-Database -Context ConstructorContext -Project DbPostgreLib -StartupProject ConstructorService
```

```
Add-Migration NLogsContext001 -Context NLogsContext -Project DbPostgreLib -StartupProject ApiRestService
Update-Database -Context NLogsContext -Project DbPostgreLib -StartupProject ApiRestService
```