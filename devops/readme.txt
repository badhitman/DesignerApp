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

ln -s /etc/nginx/sites-available/web.app /etc/nginx/sites-enabled/
ln -s /etc/nginx/sites-available/api.app /etc/nginx/sites-enabled/

systemctl reload nginx

apt-get update -y && upgrade -y && dist-upgrade -y && install git -y
cd /srv/git
rm -r DesignerApp
rm -r HtmlGenerator
rm -r builds
git clone https://github.com/badhitman/DesignerApp.git
git clone https://github.com/badhitman/HtmlGenerator.git

dotnet publish -c Release --output /srv/git/builds/ApiRestService /srv/git/DesignerApp/ApiRestService/ApiRestService.csproj
dotnet publish -c Release --output /srv/git/builds/StorageService /srv/git/DesignerApp/StorageService/StorageService.csproj
dotnet publish -c Release --output /srv/git/builds/CommerceService /srv/git/DesignerApp/CommerceService/CommerceService.csproj
dotnet publish -c Release --output /srv/git/builds/HelpdeskService /srv/git/DesignerApp/HelpdeskService/HelpdeskService.csproj
dotnet publish -c Release --output /srv/git/builds/Telegram.Bot.Polling /srv/git/DesignerApp/Telegram.Bot.Polling/Telegram.Bot.Polling.csproj

#  *** этот билд требует значительной мощьности сервера. на стоковом сервере не соберЄтс€ (ресурсоЄмкий процесс, который веро€тно не сможет корректно завершитьс€)
#  cd /srv/git/DesignerApp/BlankBlazorApp/BlankBlazorApp/
#  dotnet tool install -g Microsoft.Web.LibraryManager.Cli
#  dotnet workload restore
#  libman restore
#  dotnet publish -c Release --output /srv/git/builds/BlankBlazorApp /srv/git/DesignerApp/BlankBlazorApp/BlankBlazorApp/BlankBlazorApp.csproj
#  *** поэтому € его отдельно собираю локально и отправл€ю через sftp, после чего распаковываю

systemctl stop comm.app.service web.app.service tg.app.service api.app.service bus.app.service hd.app.service

rm -r /srv/services
mv /srv/git/builds/ /srv/services
sudo chown -R www-data:www-data /srv/services
chmod -R 777 /srv/services

systemctl start bus.app.service comm.app.service tg.app.service api.app.service hd.app.service web.app.service

journalctl -f -u web.app.service