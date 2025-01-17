# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5113

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ApiWebAuth/ApiWebAuth.csproj", "ApiWebAuth/"]
RUN dotnet restore "ApiWebAuth/ApiWebAuth.csproj"
COPY . .
WORKDIR "/src/ApiWebAuth"
RUN dotnet build "ApiWebAuth.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ApiWebAuth.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Ensure app.db is included in the container at the root directory
COPY ApiWebAuth/app.db /app.db

# Fix SQLite file permissions
RUN chmod 666 /app.db

ENTRYPOINT ["dotnet", "ApiWebAuth.dll"]
