#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM hub.lumigame.com/base/dotnet/core/aspnet:8.0-alpine-24h1.1 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM hub.lumigame.com/base/dotnet/core/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRounds/Tgs.GameRounds.csproj", "Tgs.GameRounds/"]
COPY ["Tgs.GameRounds.Core/Tgs.GameRounds.Core.csproj", "Tgs.GameRounds.Core/"]
COPY ["Tgs.GameRounds.Common/Tgs.GameRounds.Common.csproj", "Tgs.GameRounds.Common/"]
COPY ["Tgs.GameRounds.Repositories/Tgs.GameRounds.Repositories.csproj", "Tgs.GameRounds.Repositories/"]
RUN dotnet restore "./Tgs.GameRounds/./Tgs.GameRounds.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRounds"
RUN dotnet build "./Tgs.GameRounds.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRounds.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRounds.dll"]