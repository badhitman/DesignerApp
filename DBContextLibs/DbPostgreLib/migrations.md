```
Add-Migration MainPostgreContext001 -Context MainDbAppContext -Project DbPostgreLib -StartupProject BlankBlazorApp
Update-Database -Context MainDbAppContext -Project DbPostgreLib -StartupProject BlankBlazorApp
```

```
Add-Migration MainHelpdeskPostgreContext001 -Context HelpdeskContext -Project DbPostgreLib -StartupProject HelpdeskService
Update-Database -Context HelpdeskContext -Project DbPostgreLib -StartupProject HelpdeskService
```

```
Add-Migration MainStorageContext001 -Context CloudParametersContext -Project DbPostgreLib -StartupProject RemoteCallLib
Update-Database -Context CloudParametersContext -Project DbPostgreLib -StartupProject RemoteCallLib
```