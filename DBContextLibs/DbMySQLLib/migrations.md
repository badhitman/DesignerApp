```
Add-Migration MainMySQLContext001 -Context MainDbAppContext -Project DbMySQLLib -StartupProject BlankBlazorApp
Update-Database -Context MainDbAppContext -Project DbMySQLLib -StartupProject BlankBlazorApp
```

```
Add-Migration HelpdeskMySQLContext001 -Context HelpdeskContext -Project DbMySQLLib -StartupProject HelpdeskService
Update-Database -Context HelpdeskContext -Project DbMySQLLib -StartupProject HelpdeskService
```