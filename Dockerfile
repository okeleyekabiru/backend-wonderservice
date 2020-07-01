FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY Backend-wonderservice.sln .

COPY ["backend-wonderservice.API/backend-wonderservice.API.csproj", "backend-wonderservice.API/"]
COPY ["backend-wonderservice.DATA/backend-wonderservice.DATA.csproj", "backend-wonderservice.DATA/"]
RUN dotnet restore
COPY . .

FROM build AS publish
WORKDIR /src
RUN dotnet publish -c Release -o /src/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=publish /src/publish .
# heroku uses the following
CMD ASPNETCORE_URLS=http://*:$PORT dotnet backend-wonderservice.API.dll