docker rm -f testsnookerapi-service
docker build -t testsnookerapi-service .
docker run -d --name testsnookerapi-service -p 5200:5200 testsnookerapi-service