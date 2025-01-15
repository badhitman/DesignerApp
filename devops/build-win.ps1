Remove-Item -Recurse -Force 'C:/Users/User/Documents/publish/*'

# cd C:/Users/User/source/repos/DesignerApp
# dotnet workload update
# dotnet workload repair
# Debug/Release

dotnet publish -c Debug --output C:/Users/User/Documents/publish/IdentityService C:/Users/User/source/repos/DesignerApp/IdentityService/IdentityService.csproj
dotnet publish -c Debug --output C:/Users/User/Documents/publish/CommerceService C:/Users/User/source/repos/DesignerApp/CommerceService/CommerceService.csproj
dotnet publish -c Debug --output C:/Users/User/Documents/publish/StorageService C:/Users/User/source/repos/DesignerApp/StorageService/StorageService.csproj
dotnet publish -c Debug --output C:/Users/User/Documents/publish/ApiRestService C:/Users/User/source/repos/DesignerApp/ApiRestService/ApiRestService.csproj
dotnet publish -c Debug --output C:/Users/User/Documents/publish/HelpdeskService C:/Users/User/source/repos/DesignerApp/HelpdeskService/HelpdeskService.csproj
dotnet publish -c Debug --output C:/Users/User/Documents/publish/ConstructorService C:/Users/User/source/repos/DesignerApp/ConstructorService/ConstructorService.csproj
dotnet publish -c Debug --output C:/Users/User/Documents/publish/Telegram.Bot.Polling C:/Users/User/source/repos/DesignerApp/Telegram.Bot.Polling/Telegram.Bot.Polling.csproj
dotnet publish -c Debug --output C:/Users/User/Documents/publish/BlankBlazorApp C:/Users/User/source/repos/DesignerApp/BlankBlazorApp/BlankBlazorApp/BlankBlazorApp.csproj
