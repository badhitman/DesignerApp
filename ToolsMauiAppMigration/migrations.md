```
Add-Migration ToolsAppContext002 -Context ToolsAppContext -Project ToolsMauiAppMigration -StartupProject ToolsMauiAppMigration
Update-Database -Context ToolsAppContext -Project ToolsMauiAppMigration -StartupProject ToolsMauiAppMigration
```