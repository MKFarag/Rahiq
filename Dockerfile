# ── Stage 1: Build ──────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files first (Docker cache optimization)
COPY Domain/Domain.csproj             Domain/
COPY Application/Application.csproj   Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Presentation/Presentation.csproj  Presentation/

# Restore dependencies
RUN dotnet restore Presentation/Presentation.csproj

# Copy the rest of the source code
COPY . .

# Publish in Release mode
RUN dotnet publish Presentation/Presentation.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Stage 2: Runtime ────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/nightly/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

# Render sets $PORT automatically — fallback to 8080
CMD ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} dotnet Presentation.dll"]
