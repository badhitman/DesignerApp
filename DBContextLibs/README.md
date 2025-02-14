## Базы данных
Для упрощённого переключения между разными СУБД: **SQLite**, **MySql** и **PostgreSQL** используйте соответствующие рекомендации ниже.

> [!NOTE]
> После того как определена СУБД миграции следует применить для всех контекстов: `IdentityAppDbContext`, `MainDbAppContext`, `StorageContext`, `HelpdeskContext`, `TelegramBotContext`.

>Для работы с контекстом `IdentityAppDbContext` в консоли диспетчера пакетов используется **проект по умолчанию**: `IdentityLib`: [команды миграции подготовлены](https://github.com/badhitman/BlankCRM/blob/main/IdentityLib/migrations.md)

>Для работы с нужным контекстом в консоли диспетчера пакетов используемый/требуемый **проект по умолчанию** один из: DbMySQLLib, DbPostgreLib или DbSqliteLib. В зависимости от выбранной СУБД нужно выбрать тот или иной проект: MySQL, Postgre или Sqlite. Запускаемым проектом используйте BlazorWebApp службу.

## Примеры конфигураций
На примере `MainDbAppContext` и `MainDbAppContext`. Остальные контексты (`StorageContext`, `HelpdeskContext`, `TelegramBotContext`) устроены как `MainDbAppContext`, поэтому настройка для одного контекста наследуется на остальные.

### SQLite (установлен по умолчанию)
- Для использования **SQLite** в библиотеке `IdentityLib` дополнительных пакетов не требуется.
Настройки проекта `IdentityLib` в таком случае минимальны:
```csproj
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<TargetFramework>net8.0</TargetFramework>
		<ImplicitUsings>enable</ImplicitUsings>
		<Nullable>enable</Nullable>
		<GenerateDocumentationFile>True</GenerateDocumentationFile>
	</PropertyGroup>

	<ItemGroup>
		<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.5" />
	</ItemGroup>

	<ItemGroup>
	  <ProjectReference Include="..\SharedLib\SharedLib.csproj" />
	</ItemGroup>

</Project>
```
- В секреты укажите строки подключения:
```json
{
  "ConnectionStrings": {
    "IdentityConnection": "Data Source=..\\..\\blank-identity-db.sqlite",
    "MainConnection": "Data Source=..\\..\\blank-main-db.sqlite"
  }
}
```
В общем такие строки подключения могут оказаться и в **appsettings.json**, но тогда имейте ввиду, что строки подключения в секретах имеют больший приоритет. Если строки подключений указаны в секретах, тогда строки из **appsettings.json** будут проигнорированы.
- В библиотеке `ServerLib` установите зависимость на `..\DBContextLibs\DbSqliteLib\DbSqliteLib.csproj`. Убедитесь, что отсутствуют зависимости от двух других `DBContextLibs` библиотек: `DbPostgreLib` и `DbMySQLLib`. Зависимость одновременно от разных `DBContextLibs` библиотек не допускается.
- Установите нужный `Use` в **BlazorWebApp** `Program.cs` : в случае **SQLite** будет UseSqlite. Пример:
```c#
string connectionIdentityString = builder.Configuration.GetConnectionString("IdentityConnection") ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");
builder.Services.AddDbContextFactory<IdentityAppDbContext>(opt =>
    opt.UseNpgsql(connectionIdentityString));

string connectionMainString = builder.Configuration.GetConnectionString("MainConnection") ?? throw new InvalidOperationException("Connection string 'MainConnection' not found.");
builder.Services.AddDbContextFactory<MainDbAppContext>(opt =>
    opt.UseNpgsql(connectionMainString));
builder.Services.AddDbContext<MainDbAppContext>();
```
> [!TIP]
> В исходном состоянии установлен именно SQLite, поэтому предварительных настроек не требуется, а вот миграции нужно/можно использовать.
```Batchfile
Add-Migration MainContext001 -Context MainDbAppContext -Project DbSqliteLib -StartupProject ConstructorBlazorApp
Update-Database -Context MainDbAppContext -Project DbSqliteLib -StartupProject ConstructorBlazorApp

Add-Migration IdentityContext001 -Context IdentityAppDbContext -Project IdentityLib -StartupProject ConstructorBlazorApp
Update-Database -Context IdentityAppDbContext -Project IdentityLib -StartupProject ConstructorBlazorApp
```

### PostgreSQL
- Для использования **PostgreSQL** в библиотеке `IdentityLib` добавьте пакет `Npgsql.EntityFrameworkCore.PostgreSQL`. Пример настроек проекта `IdentityLib` в таком случае:
```csproj
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<TargetFramework>net8.0</TargetFramework>
		<ImplicitUsings>enable</ImplicitUsings>
		<Nullable>enable</Nullable>
		<GenerateDocumentationFile>True</GenerateDocumentationFile>
	</PropertyGroup>

	<ItemGroup>
		<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.5" />
		<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.4" />
	</ItemGroup>

	<ItemGroup>
	  <ProjectReference Include="..\SharedLib\SharedLib.csproj" />
	</ItemGroup>

</Project>
```
- В секреты укажите строки подключения:
```json
{
  "ConnectionStrings": {
    "IdentityConnection": "User ID=postgres;Password=mysecretpassword;Host=localhost;Port=5432;Database=blank_identity;",
    "MainConnection": "User ID=postgres;Password=mysecretpassword;Host=localhost;Port=5432;Database=blank_main;"
  }
}
```
- В библиотеке `ServerLib` установите зависимость на `..\DBContextLibs\DbPostgreLib\DbPostgreLib.csproj`. Убедитесь, что отсутствуют зависимости от двух других `DBContextLibs` библиотек: `DbSqliteLib` и `DbMySQLLib`. Зависимость одновременно от разных `DBContextLibs` библиотек не допускается.
- Установите нужный `Use` в **BlazorWebApp** `Program.cs` : в случае **PostgreSQL** будет UseNpgsql. Пример:
```c#
string connectionIdentityString = builder.Configuration.GetConnectionString("IdentityConnection") ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");
builder.Services.AddDbContextFactory<IdentityAppDbContext>(opt =>
    opt.UseNpgsql(connectionIdentityString));

string connectionMainString = builder.Configuration.GetConnectionString("MainConnection") ?? throw new InvalidOperationException("Connection string 'MainConnection' not found.");
builder.Services.AddDbContextFactory<MainDbAppContext>(opt =>
    opt.UseNpgsql(connectionMainString));
```


### MySql
- Для использования **MySql** в библиотеке `IdentityLib` добавьте пакет `MySql.EntityFrameworkCore`. Пример настроек проекта `IdentityLib` в таком случае:
```csproj
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<TargetFramework>net8.0</TargetFramework>
		<ImplicitUsings>enable</ImplicitUsings>
		<Nullable>enable</Nullable>
		<GenerateDocumentationFile>True</GenerateDocumentationFile>
	</PropertyGroup>

	<ItemGroup>
		<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.5" />
		<PackageReference Include="MySql.EntityFrameworkCore" Version="8.0.2" />
	</ItemGroup>

	<ItemGroup>
	  <ProjectReference Include="..\SharedLib\SharedLib.csproj" />
	</ItemGroup>

</Project>
```
- В секреты укажите строки подключения:
```json
{
  "ConnectionStrings": {
    "IdentityConnection": "Server=localhost;Database=blank_identity;Uid=root;Pwd=my-secret-pw;",
    "MainConnection": "Server=localhost;Database=blank_main;Uid=root;Pwd=my-secret-pw;"
  }
}
```
- В библиотеке `ServerLib` установите зависимость на `..\DBContextLibs\DbMySQLLib\DbMySQLLib.csproj`. Убедитесь, что отсутствуют зависимости от двух других `DBContextLibs` библиотек: `DbSqliteLib` и `DbPostgreLib`. Зависимость одновременно от разных `DBContextLibs` библиотек не допускается.
- Установите нужный `Use` в **BlazorWebApp** `Program.cs` : в случае **MySql** будет `UseMySQL`. Пример:
```c#
string connectionIdentityString = builder.Configuration.GetConnectionString("IdentityConnection") ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");
builder.Services.AddDbContextFactory<IdentityAppDbContext>(opt =>
    opt.UseMySQL(connectionIdentityString));

string connectionMainString = builder.Configuration.GetConnectionString("MainConnection") ?? throw new InvalidOperationException("Connection string 'MainConnection' not found.");
builder.Services.AddDbContextFactory<MainDbAppContext>(opt =>
    opt.UseMySQL(connectionMainString));
builder.Services.AddDbContext<MainDbAppContext>();
```