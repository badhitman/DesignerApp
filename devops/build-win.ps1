cd 'C:/Users/User/Documents/publish/'
Remove-Item -Recurse -Force 'C:/Users/User/Documents/publish/*'

# cd C:/Users/User/source/repos/BlankCRM
# dotnet workload update
# dotnet workload repair
# Debug/Release

dotnet publish -c Debug --output C:/Users/User/Documents/publish/IdentityService C:/Users/User/source/repos/BlankCRM/IdentityService/IdentityService.csproj
dotnet publish -c Debug --output C:/Users/User/Documents/publish/CommerceService C:/Users/User/source/repos/BlankCRM/CommerceService/CommerceService.csproj
dotnet publish -c Debug --output C:/Users/User/Documents/publish/StorageService C:/Users/User/source/repos/BlankCRM/StorageService/StorageService.csproj
dotnet publish -c Debug --output C:/Users/User/Documents/publish/ApiRestService C:/Users/User/source/repos/BlankCRM/ApiRestService/ApiRestService.csproj
dotnet publish -c Debug --output C:/Users/User/Documents/publish/HelpdeskService C:/Users/User/source/repos/BlankCRM/HelpdeskService/HelpdeskService.csproj
dotnet publish -c Debug --output C:/Users/User/Documents/publish/ConstructorService C:/Users/User/source/repos/BlankCRM/ConstructorService/ConstructorService.csproj
dotnet publish -c Debug --output C:/Users/User/Documents/publish/TelegramBotService C:/Users/User/source/repos/BlankCRM/TelegramBotService/TelegramBotService.csproj
dotnet publish -c Debug --output C:/Users/User/Documents/publish/BlankBlazorApp C:/Users/User/source/repos/BlankCRM/BlankBlazorApp/BlankBlazorApp/BlankBlazorApp.csproj

$7zipPath = "$env:ProgramFiles\7-Zip\7z.exe"
if (-not (Test-Path -Path $7zipPath -PathType Leaf)) {
    throw "7 zip executable '$7zipPath' not found"
}
Set-Alias Start-SevenZip $7zipPath
Start-SevenZip a -mx=9 -sdel "C:/Users/User/Documents/publish/publish.7z" "C:/Users/User/Documents/publish/*"
#Remove-Item -Recurse -Force -Exclude publish.7z 'C:/Users/User/Documents/publish/*'
