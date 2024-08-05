```
Add-Migration MainContext001 -Context MainDbAppContext -Project DbSqliteLib -StartupProject BlankBlazorApp
Update-Database -Context MainDbAppContext -Project DbSqliteLib -StartupProject BlankBlazorApp
```