```
Add-Migration MainContext001 -Context MainDbAppContext -Project DbSqliteLib -StartupProject BlankBlazorApp
Update-Database -Context MainDbAppContext -Project DbSqliteLib -StartupProject BlankBlazorApp
```

```
Add-Migration HelpdeskContext001 -Context HelpdeskContext -Project DbSqliteLib -StartupProject HelpdeskService
Update-Database -Context HelpdeskContext -Project DbSqliteLib -StartupProject HelpdeskService
```