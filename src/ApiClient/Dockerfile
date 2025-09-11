#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled-extra AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM docker-repository.intern.neusta.de/neusta/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ApiClient/ApiClient.csproj", "src/ApiClient/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["NuGet.config", "NuGet.config"]
RUN dotnet restore "src/ApiClient/ApiClient.csproj"
COPY . .

WORKDIR "/src/src/ApiClient"
RUN dotnet build "ApiClient.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ApiClient.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
# Add (NEUSTA root certificates) certificates - only possible with neusta/dotnet/sdk image
COPY --from=build /etc/ssl/certs /etc/ssl/certs
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Neusta.Jira.Connector.ApiClient.dll"]
