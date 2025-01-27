STAGING

systemctl stop api.app.stage.service web.app.stage.service comm.app.stage.service tg.app.stage.service bus.app.stage.service constructor.app.stage.service hd.app.stage.service

# rm -r /srv/git/builds/*
rm -r /srv/services.stage/*

cp -r /srv/git/builds/ApiRestService /srv/services.stage/ApiRestService
cp -r /srv/git/builds/StorageService /srv/services.stage/StorageService
cp -r /srv/git/builds/CommerceService /srv/services.stage/CommerceService
cp -r /srv/git/builds/HelpdeskService /srv/services.stage/HelpdeskService
cp -r /srv/git/builds/ConstructorService /srv/services.stage/ConstructorService
cp -r /srv/git/builds/TelegramBotService /srv/services.stage/TelegramBotService
cp -r /srv/git/builds/BlankBlazorApp /srv/services.stage/BlankBlazorApp

chown -R www-data:www-data /srv/services.stage
chmod -R 777 /srv/services.stage

chown -R www-data:www-data /srv/git/builds
chmod -R 777 /srv/git/builds

systemctl start api.app.stage.service comm.app.stage.service web.app.stage.service bus.app.stage.service tg.app.stage.service hd.app.stage.service constructor.app.stage.service


journalctl -f -u web.app.stage.service
systemctl status constructor.app.stage.service