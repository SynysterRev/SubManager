FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copie la solution et tous les projets
COPY SubManager.sln ./
COPY SubManager.API/SubManager.API.csproj SubManager.API/
COPY SubManager.Application/SubManager.Application.csproj SubManager.Application/
COPY SubManager.Domain/SubManager.Domain.csproj SubManager.Domain/
COPY SubManager.Infrastructure/SubManager.Infrastructure.csproj SubManager.Infrastructure/
COPY SubManager.Tests/SubManager.Tests.csproj SubManager.Tests/

# Restore
RUN dotnet restore SubManager.sln

# Copie tout le code source
COPY . .

# Build le projet API
WORKDIR /src/SubManager.API
RUN dotnet build SubManager.API.csproj -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish SubManager.API.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Image finale
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Railway utilise la variable PORT
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}

ENTRYPOINT ["dotnet", "SubManager.API.dll"]