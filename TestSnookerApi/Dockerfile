FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS builder
WORKDIR /src

# caches restore results by copying csproj files separately
COPY ./TestSnookerApi.csproj ./TestSnookerApi.csproj
RUN dotnet restore
# then copy the rest of the sources
COPY . .

# publish
RUN dotnet publish ./TestSnookerApi.csproj --output /app/ --configuration Release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
WORKDIR /app
COPY --from=builder /app/ .
ENV ASPNETCORE_URLS "http://*:5200"
ENTRYPOINT ["dotnet", "TestSnookerApi.dll"]
