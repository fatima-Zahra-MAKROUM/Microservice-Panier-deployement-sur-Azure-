# Image de base légère pour l'exécution (ASP.NET Core 8.0)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Image complète avec le SDK pour compiler le projet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copie uniquement le .csproj d'abord → meilleure utilisation du cache Docker
COPY ["*.csproj", "./"]
RUN dotnet restore

# Copie tout le reste du code source
COPY . .
# Compilation en mode Release
RUN dotnet build -c Release -o /app/build

# Étape de publication
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Image finale ultra-légère (~120 Mo)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Point d'entrée : lance le microservice
ENTRYPOINT ["dotnet", "CartMicroservice.dll"]