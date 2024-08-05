```
Add-Migration MainPostgreContext001 -Context MainDbAppContext -Project DbPostgreLib -StartupProject ConstructorBlazorApp
Update-Database -Context MainDbAppContext -Project DbPostgreLib -StartupProject ConstructorBlazorApp
```