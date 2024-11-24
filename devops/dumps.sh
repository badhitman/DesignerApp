rm -r /srv/tmp/dumps/*
docker exec -t srv_postgres_1 pg_dump -c -U dev commerce --format=p --encoding=UTF-8 --inserts | gzip > /srv/tmp/dumps/dump_commerce_`date +%Y-%m-%d"_"%H_%M_%S`.sql.gz
docker exec -t srv_postgres_1 pg_dump -c -U dev HelpDesk --format=p --encoding=UTF-8 --inserts | gzip > /srv/tmp/dumps/dump_HelpDesk_`date +%Y-%m-%d"_"%H_%M_%S`.sql.gz
docker exec -t srv_postgres_1 pg_dump -c -U dev StorageCloud --format=p --encoding=UTF-8 --inserts | gzip > /srv/tmp/dumps/dump_StorageCloud_`date +%Y-%m-%d"_"%H_%M_%S`.sql.gz
docker exec -t srv_postgres_1 pg_dump -c -U dev TelegramBot --format=p --encoding=UTF-8 --inserts | gzip > /srv/tmp/dumps/dump_TelegramBot_`date +%Y-%m-%d"_"%H_%M_%S`.sql.gz
docker exec -t srv_postgres_1 pg_dump -c -U dev Identity --format=p --encoding=UTF-8 --inserts | gzip > /srv/tmp/dumps/dump_Identity_`date +%Y-%m-%d"_"%H_%M_%S`.sql.gz
docker exec -t srv_mongodb_1 mongodump --db files-system -u dev -p dev --authenticationDatabase admin --out /srv/tmp/dumps/mongodump/`date +"%Y-%m-%d"_"%H_%M_%S"`
7z a /srv/db-backups/all_dumps_`date +%Y-%m-%d"_"%H_%M_%S`.7z /srv/tmp/dumps/*
