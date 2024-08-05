```
Add-Migration MainMySQLContext001 -Context MainDbAppContext -Project DbMySQLLib -StartupProject ConstructorBlazorApp
Update-Database -Context MainDbAppContext -Project DbMySQLLib -StartupProject ConstructorBlazorApp
```