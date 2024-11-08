apt-get update -y && upgrade -y && dist-upgrade -y && install git -y

apt-get install wget
apt-get install nano
apt-get install sqlite3

apt-get install ufw
ufw allow 22
ufw allow 443
ufw allow 80
ufw enable

# our .net8

chown -R www-data:www-data /root/.vs-debugger
chmod -R 777 /root/.vs-debugger

ln -s /etc/nginx/sites-available/web.app /etc/nginx/sites-enabled/
ln -s /etc/nginx/sites-available/api.app /etc/nginx/sites-enabled/

systemctl reload nginx
docker-compose up -d

apt update -y && apt upgrade -y && apt dist-upgrade -y && apt install git -y

cd /srv/git
rm -r *
git clone https://github.com/badhitman/DesignerApp.git
git clone https://github.com/badhitman/HtmlGenerator.git

dotnet publish -c Debug --output /srv/git/builds/ApiRestService /srv/git/DesignerApp/ApiRestService/ApiRestService.csproj
dotnet publish -c Debug --output /srv/git/builds/StorageService /srv/git/DesignerApp/StorageService/StorageService.csproj
dotnet publish -c Debug --output /srv/git/builds/CommerceService /srv/git/DesignerApp/CommerceService/CommerceService.csproj
dotnet publish -c Debug --output /srv/git/builds/HelpdeskService /srv/git/DesignerApp/HelpdeskService/HelpdeskService.csproj
dotnet publish -c Debug --output /srv/git/builds/ConstructorService /srv/git/DesignerApp/ConstructorService/ConstructorService.csproj
dotnet publish -c Debug --output /srv/git/builds/Telegram.Bot.Polling /srv/git/DesignerApp/Telegram.Bot.Polling/Telegram.Bot.Polling.csproj

#  *** этот билд требует значительной мощьности железа. на стоковом сервере не соберЄтс€ (ресурсоЄмкий процесс, который веро€тно не сможет корректно завершитьс€)
#  cd /srv/git/DesignerApp/BlankBlazorApp/BlankBlazorApp/
#  dotnet tool install -g Microsoft.Web.LibraryManager.Cli
#  dotnet workload restore
#  libman restore
#  dotnet publish -c Release --output /srv/git/builds/BlankBlazorApp /srv/git/DesignerApp/BlankBlazorApp/BlankBlazorApp/BlankBlazorApp.csproj
#  *** поэтому € его отдельно собираю локально, отправл€ю через sftp, распаковываю и продолжаю дальше буд-то команды корректно отработали

systemctl stop web.app.service comm.app.service tg.app.service api.app.service bus.app.service hd.app.service constructor.app.service

# rm -r /srv/git/builds/*

# cd /srv/services/
# rm -r /srv/services/*
cp -r /srv/git/builds/ApiRestService /srv/services/ApiRestService
cp -r /srv/git/builds/StorageService /srv/services/StorageService
cp -r /srv/git/builds/CommerceService /srv/services/CommerceService
cp -r /srv/git/builds/HelpdeskService /srv/services/HelpdeskService
cp -r /srv/git/builds/ConstructorService /srv/services/ConstructorService
cp -r /srv/git/builds/Telegram.Bot.Polling /srv/services/Telegram.Bot.Polling
cp -r /srv/git/builds/BlankBlazorApp /srv/services/BlankBlazorApp

chown -R www-data:www-data /srv/services
chmod -R 777 /srv/services

systemctl start comm.app.service web.app.service bus.app.service tg.app.service api.app.service hd.app.service constructor.app.service

journalctl -f -u web.app.service
