```
Add-Migration MainContext001 -Context MainDbAppContext -Project DbSqliteLib -StartupProject ConstructorBlazorApp
Update-Database -Context MainDbAppContext -Project DbSqliteLib -StartupProject ConstructorBlazorApp
```