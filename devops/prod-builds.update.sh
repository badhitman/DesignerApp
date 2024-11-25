systemctl stop web.app.service comm.app.service tg.app.service api.app.service bus.app.service constructor.app.service hd.app.service
7z a /srv/services-snapshots/all_services_`date +%Y-%m-%d"_"%H_%M_%S`.7z /srv/services/*
rm -r /srv/services/*
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