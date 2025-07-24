# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj files and restore dependencies
COPY ["src/Presentation/Presentation.csproj", "src/Presentation/"]
COPY ["src/Common/Common.csproj", "src/Common/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]

COPY TicketHub.sln .

# Restore dependencies
RUN dotnet restore "src/Presentation/Presentation.csproj"

# Copy rest of procject
COPY . .

# ---- Publish stage ----
WORKDIR "/app/src/Presentation"
RUN dotnet publish -c Release -o /app/publish

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Presentation.dll"]