FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY PulseWatch.sln ./
COPY backend/PulseWatch.Domain/PulseWatch.Domain.csproj backend/PulseWatch.Domain/
COPY backend/PulseWatch.Application/PulseWatch.Application.csproj backend/PulseWatch.Application/
COPY backend/PulseWatch.Infrastructure/PulseWatch.Infrastructure.csproj backend/PulseWatch.Infrastructure/
COPY backend/PulseWatch.API/PulseWatch.API.csproj backend/PulseWatch.API/

RUN dotnet restore PulseWatch.sln

# Copy everything and build
COPY . .
RUN dotnet publish backend/PulseWatch.API/PulseWatch.API.csproj -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "PulseWatch.API.dll"]
