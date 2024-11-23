docker exec -t srv_postgres_1 pg_dump -c -U dev commerce --format=p --encoding=UTF-8 --inserts | gzip > /srv/db-backups/dump_commerce_`date +%Y-%m-%d"_"%H_%M_%S`.sql.gz
docker exec -t srv_postgres_1 pg_dump -c -U dev HelpDesk --format=p --encoding=UTF-8 --inserts | gzip > /srv/db-backups/dump_HelpDesk_`date +%Y-%m-%d"_"%H_%M_%S`.sql.gz
docker exec -t srv_postgres_1 pg_dump -c -U dev StorageCloud --format=p --encoding=UTF-8 --inserts | gzip > /srv/db-backups/dump_StorageCloud_`date +%Y-%m-%d"_"%H_%M_%S`.sql.gz
docker exec -t srv_postgres_1 pg_dump -c -U dev TelegramBot --format=p --encoding=UTF-8 --inserts | gzip > /srv/db-backups/dump_TelegramBot_`date +%Y-%m-%d"_"%H_%M_%S`.sql.gz
docker exec -t srv_postgres_1 pg_dump -c -U dev Identity --format=p --encoding=UTF-8 --inserts | gzip > /srv/db-backups/dump_Identity_`date +%Y-%m-%d"_"%H_%M_%S`.sql.gz
chown -R www-data:www-data /srv/db-backups
chmod -R 777 /srv/db-backups
