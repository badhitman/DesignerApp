docker exec -t srv_postgres_1 pg_dump -c -U dev commerce --format=p --encoding=UTF-8 --inserts | gzip > /srv/db-backups/dump_commerce_`date +%Y-%m-%d"_"%H_%M_%S`.sql.gz
docker exec -t srv_postgres_1 pg_dump -c -U dev HelpDesk --format=p --encoding=UTF-8 --inserts | gzip > /srv/db-backups/dump_HelpDesk_`date +%Y-%m-%d"_"%H_%M_%S`.sql.gz
docker exec -t srv_postgres_1 pg_dump -c -U dev StorageCloud --format=p --encoding=UTF-8 --inserts | gzip > /srv/db-backups/dump_StorageCloud_`date +%Y-%m-%d"_"%H_%M_%S`.sql.gz
docker exec -t srv_postgres_1 pg_dump -c -U dev TelegramBot --format=p --encoding=UTF-8 --inserts | gzip > /srv/db-backups/dump_TelegramBot_`date +%Y-%m-%d"_"%H_%M_%S`.sql.gz
docker exec -t srv_postgres_1 pg_dump -c -U dev Identity --format=p --encoding=UTF-8 --inserts | gzip > /srv/db-backups/dump_Identity_`date +%Y-%m-%d"_"%H_%M_%S`.sql.gz
docker exec -t srv_mongodb_1 mongodump --db files-system -u dev -p dev --authenticationDatabase admin --out /srv/db-backups/mongodump/`date +"%Y-%m-%d"_"%H_%M_%S"`
7z a /srv/tmp/all_dumps_`date +%Y-%m-%d"_"%H_%M_%S`.7z /srv/db-backups/*
rm -r /srv/db-backups/*
chown -R www-data:www-data /srv
chmod -R 777 /srv
