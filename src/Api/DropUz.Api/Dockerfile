# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Api/DropUz.Api/DropUz.Api.csproj", "src/Api/DropUz.Api/"]
COPY ["src/Common/DropUz.Common.Infrastructure/DropUz.Common.Infrastructure.csproj", "src/Common/DropUz.Common.Infrastructure/"]
COPY ["src/Common/DropUz.Common.Application/DropUz.Common.Application.csproj", "src/Common/DropUz.Common.Application/"]
COPY ["src/Common/DropUz.Common.Domain/DropUz.Common.Domain.csproj", "src/Common/DropUz.Common.Domain/"]
COPY ["src/Common/DropUz.Common.Presentation/DropUz.Common.Presentation.csproj", "src/Common/DropUz.Common.Presentation/"]
RUN dotnet restore "./src/Api/DropUz.Api/DropUz.Api.csproj"
COPY . .
WORKDIR "/src/src/Api/DropUz.Api"
RUN dotnet build "./DropUz.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DropUz.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DropUz.Api.dll"]