```
Add-Migration IdentityContext002 -Context IdentityAppDbContext -Project IdentityLib -StartupProject BlankBlazorApp
Update-Database -Context IdentityAppDbContext -Project IdentityLib -StartupProject BlankBlazorApp
```