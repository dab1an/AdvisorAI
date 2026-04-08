FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore first for better layer caching.
COPY server.csproj ./
RUN dotnet restore server.csproj

# Copy source and publish a release build.
COPY . ./
RUN dotnet publish server.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Azure Container Apps commonly targets port 8080.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "server.dll"]
