apt-get update
apt-get upgrade
apt-get dist-upgrade

apt-get install redis-server 
apt-get install wget
apt-get install nano
apt-get install sqlite3

apt-get install ufw
ufw allow 22
ufw allow 443
ufw enable

# install .net8

systemctl enable web.app
systemctl start web.app
journalctl -u web.app

ln -s /etc/nginx/sites-available/web.app /etc/nginx/sites-enabled/

systemctl reload nginx

ln -s /etc/nginx/sites-available/iq-s.pro.conf /etc/nginx/sites-enabled/

systemctl reload nginx


apt-get update -y && upgrade -y && dist-upgrade -y && install git -y
cd /srv/git
rm -r DesignerApp
rm -r HtmlGenerator
rm -r builds
git clone https://github.com/badhitman/DesignerApp.git
git clone https://github.com/badhitman/HtmlGenerator.git
cd /srv/git/DesignerApp/BlankBlazorApp/BlankBlazorApp/
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
dotnet workload restore
libman restore
dotnet publish -c Release --output /srv/git/builds/StorageService /srv/git/DesignerApp/StorageService/StorageService.csproj
dotnet publish -c Release --output /srv/git/builds/ApiRestService /srv/git/DesignerApp/ApiRestService/ApiRestService.csproj
dotnet publish -c Release --output /srv/git/builds/CommerceService /srv/git/DesignerApp/CommerceService/CommerceService.csproj
dotnet publish -c Release --output /srv/git/builds/HelpdeskService /srv/git/DesignerApp/HelpdeskService/HelpdeskService.csproj
dotnet publish -c Release --output /srv/git/builds/Telegram.Bot.Polling /srv/git/DesignerApp/Telegram.Bot.Polling/Telegram.Bot.Polling.csproj

dotnet publish -c Release --output /srv/git/builds/BlankBlazorApp /srv/git/DesignerApp/BlankBlazorApp/BlankBlazorApp/BlankBlazorApp.csproj

systemctl stop comm.app.service
systemctl stop web.app.service
systemctl stop tg.app.service
systemctl stop api.app.service
systemctl stop bus.app.service
systemctl stop hd.app.service

rm -r /srv/services
mv /srv/git/builds/ /srv/services
sudo chown -R www-data:www-data /srv/services
chmod -R 777 /srv/services

systemctl start comm.app.service
systemctl start web.app.service
systemctl start tg.app.service
systemctl start api.app.service
systemctl start bus.app.service
systemctl start hd.app.service
