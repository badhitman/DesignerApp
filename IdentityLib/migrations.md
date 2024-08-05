```
Add-Migration IdentityContext001 -Context IdentityAppDbContext -Project IdentityLib -StartupProject BlankBlazorApp
Update-Database -Context IdentityAppDbContext -Project IdentityLib -StartupProject BlankBlazorApp
```