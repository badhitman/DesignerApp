```
Add-Migration MainPostgreContext001 -Context MainDbAppContext -Project DbPostgreLib -StartupProject BlankBlazorApp
Update-Database -Context MainDbAppContext -Project DbPostgreLib -StartupProject BlankBlazorApp
```