# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["InstantProforms.sln", "./"]
COPY ["InstantProforms.API/InstantProforms.API.csproj", "InstantProforms.API/"]
COPY ["InstantProforms.Application/InstantProforms.Application.csproj", "InstantProforms.Application/"]
COPY ["InstantProforms.Domain/InstantProforms.Domain.csproj", "InstantProforms.Domain/"]
COPY ["InstantProforms.Infrastructure/InstantProforms.Infrastructure.csproj", "InstantProforms.Infrastructure/"]

RUN dotnet restore "InstantProforms.API/InstantProforms.API.csproj"

COPY . .

RUN dotnet publish "InstantProforms.API/InstantProforms.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "InstantProforms.API.dll"]