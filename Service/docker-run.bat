docker rm -f betsnooker-service
docker build -t betsnooker-service .
docker run -d --name betsnooker-service -p 5100:5100 betsnooker-service