server {
    listen 80;
    server_name api.my-site.ru www.api.my-site.ru;
    # return 301 https://$host$request_uri;
	large_client_header_buffers 4 128k;
	
	access_log /var/log/nginx/nginx.http.api.my-site.ru.access.log;
	error_log /var/log/nginx/nginx.http.api.my-site.ru.error.log;

	add_header X-Content-Type-Options "nosniff";

    location / {
		proxy_pass http://localhost:5000;
		proxy_http_version 1.1;
		proxy_set_header Upgrade $http_upgrade;
		proxy_set_header Connection $http_connection;
		proxy_set_header Host $host;
		proxy_cache_bypass $http_upgrade;
			
		if ($request_method = 'OPTIONS') {
			add_header 'Access-Control-Allow-Origin' '*';
			add_header 'Access-Control-Allow-Credentials' 'true';
			add_header 'Access-Control-Allow-Methods' 'GET, DELETE, POST, PUT, PATCH, OPTIONS';
	 
			add_header 'Access-Control-Allow-Headers' 'token-session-id,DNT,X-CustomHeader,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type';
	 
			add_header 'Access-Control-Max-Age' 1728000;
			add_header 'Content-Type' 'text/plain charset=UTF-8';
			add_header 'Content-Length' 0;
			return 204;
		 }
		 if ($request_method = 'POST') {
			add_header 'Access-Control-Allow-Credentials' 'true';
			add_header 'Access-Control-Allow-Methods' 'GET, DELETE, POST, PUT, PATCH, OPTIONS';
			add_header 'Access-Control-Allow-Headers' 'token-session-id,DNT,X-CustomHeader,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type';
		 }
		 if ($request_method = 'PUT') {
			add_header 'Access-Control-Allow-Credentials' 'true';
			add_header 'Access-Control-Allow-Methods' 'GET, DELETE, POST, PUT, PATCH, OPTIONS';
			add_header 'Access-Control-Allow-Headers' 'token-session-id,DNT,X-CustomHeader,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type';
		 }
		 if ($request_method = 'PATCH') {
			add_header 'Access-Control-Allow-Credentials' 'true';
			add_header 'Access-Control-Allow-Methods' 'GET, DELETE, POST, PUT, PATCH, OPTIONS';
			add_header 'Access-Control-Allow-Headers' 'token-session-id,DNT,X-CustomHeader,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type';
		 }
		 if ($request_method = 'GET') {
			add_header 'Access-Control-Allow-Credentials' 'true' always;
			add_header 'Access-Control-Allow-Methods' 'GET, DELETE, POST, PUT, PATCH, OPTIONS' always;
			add_header 'Access-Control-Allow-Headers' 'token-session-id,DNT,X-CustomHeader,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Range' always;
		}
		 if ($request_method = 'DELETE') {
			add_header 'Access-Control-Allow-Credentials' 'true';
			add_header 'Access-Control-Allow-Methods' 'GET, DELETE, POST, PUT, PATCH, OPTIONS';
			add_header 'Access-Control-Allow-Headers' 'token-session-id,DNT,X-CustomHeader,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type';
		}
    }
}

server {
	listen 443 ssl;
	listen [::]:443 ssl;
	server_name api.my-site.ru www.api.my-site.ru;
	ssl_certificate /etc/ssl/my-site/my-site.crt;
	ssl_certificate_key /etc/ssl/my-site/my-site.key;
	large_client_header_buffers 4 128k;

	access_log /var/log/nginx/nginx.https.api.my-site.ru.access.log;
	error_log /var/log/nginx/nginx.https.api.my-site.ru.error.log;

	add_header X-Content-Type-Options "nosniff";

    location / {
		proxy_pass http://localhost:5000;
		proxy_http_version 1.1;
		proxy_set_header Upgrade $http_upgrade;
		proxy_set_header Connection $http_connection;
		proxy_set_header Host $host;
		proxy_cache_bypass $http_upgrade;
			
		if ($request_method = 'OPTIONS') {
			add_header 'Access-Control-Allow-Origin' '*';
			add_header 'Access-Control-Allow-Credentials' 'true';
			add_header 'Access-Control-Allow-Methods' 'GET, DELETE, POST, PUT, PATCH, OPTIONS';
	 
			add_header 'Access-Control-Allow-Headers' 'token-session-id,DNT,X-CustomHeader,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type';
	 
			add_header 'Access-Control-Max-Age' 1728000;
			add_header 'Content-Type' 'text/plain charset=UTF-8';
			add_header 'Content-Length' 0;
			return 204;
		 }
		 if ($request_method = 'POST') {
			add_header 'Access-Control-Allow-Credentials' 'true';
			add_header 'Access-Control-Allow-Methods' 'GET, DELETE, POST, PUT, PATCH, OPTIONS';
			add_header 'Access-Control-Allow-Headers' 'token-session-id,DNT,X-CustomHeader,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type';
		 }
		 if ($request_method = 'PUT') {
			add_header 'Access-Control-Allow-Credentials' 'true';
			add_header 'Access-Control-Allow-Methods' 'GET, DELETE, POST, PUT, PATCH, OPTIONS';
			add_header 'Access-Control-Allow-Headers' 'token-session-id,DNT,X-CustomHeader,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type';
		 }
		 if ($request_method = 'PATCH') {
			add_header 'Access-Control-Allow-Credentials' 'true';
			add_header 'Access-Control-Allow-Methods' 'GET, DELETE, POST, PUT, PATCH, OPTIONS';
			add_header 'Access-Control-Allow-Headers' 'token-session-id,DNT,X-CustomHeader,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type';
		 }
		 if ($request_method = 'GET') {
			add_header 'Access-Control-Allow-Credentials' 'true' always;
			add_header 'Access-Control-Allow-Methods' 'GET, DELETE, POST, PUT, PATCH, OPTIONS' always;
			add_header 'Access-Control-Allow-Headers' 'token-session-id,DNT,X-CustomHeader,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Range' always;
		}
		 if ($request_method = 'DELETE') {
			add_header 'Access-Control-Allow-Credentials' 'true';
			add_header 'Access-Control-Allow-Methods' 'GET, DELETE, POST, PUT, PATCH, OPTIONS';
			add_header 'Access-Control-Allow-Headers' 'token-session-id,DNT,X-CustomHeader,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type';
		}
    }
}