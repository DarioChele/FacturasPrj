# =========================
# Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Backend.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

# =========================
# Final
# =========================
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY Database ./Database
ENTRYPOINT ["dotnet", "Backend.dll"]
