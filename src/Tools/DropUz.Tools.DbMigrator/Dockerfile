FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore "src/Tools/DropUz.Tools.DbMigrator/DropUz.Tools.DbMigrator.csproj"
RUN dotnet publish "src/Tools/DropUz.Tools.DbMigrator/DropUz.Tools.DbMigrator.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
USER $APP_UID
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DropUz.Tools.DbMigrator.dll"]
