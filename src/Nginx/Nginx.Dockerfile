FROM nginx:latest

COPY /src/Nginx/nginx.conf /etc/nginx/nginx.conf
COPY /src/Nginx/localhost.crt /etc/ssl/certs/localhost.crt
COPY /src/Nginx/localhost.key /etc/ssl/private/localhost.key

#COPY nginx.conf /etc/nginx/nginx.conf
#COPY certificate.pem /etc/ssl/certs/certificate.pem
#COPY private.pem /etc/ssl/private/private.pem