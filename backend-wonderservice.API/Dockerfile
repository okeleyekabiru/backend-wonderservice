FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY ["backend-wonderservice.API.csproj", "backend-wonderservice.API/"]
COPY ["./backend-wonderservice.DATA/backend-wonderservice.DATA.csproj", "./backend-wonderservice.DATA/"]
RUN dotnet restore "backend-wonderservice.API/backend-wonderservice.API.csproj"
COPY . .
WORKDIR "/src/backend-wonderservice.API"
RUN dotnet build "backend-wonderservice.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "backend-wonderservice.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet backend-wonderservice.API.dll
