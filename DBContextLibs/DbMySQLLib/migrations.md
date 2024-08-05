```
Add-Migration MainMySQLContext001 -Context MainDbAppContext -Project DbMySQLLib -StartupProject BlankBlazorApp

Update-Database -Context MainDbAppContext -Project DbMySQLLib -StartupProject BlankBlazorApp
```