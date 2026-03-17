# Use SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and restore dependencies
# Note: Adjusting paths relative to the root where this Dockerfile lives
COPY *.sln ./
COPY src/SmartInvestigation.API/*.csproj src/SmartInvestigation.API/
COPY src/SmartInvestigation.Application/*.csproj src/SmartInvestigation.Application/
COPY src/SmartInvestigation.Infrastructure/*.csproj src/SmartInvestigation.Infrastructure/
COPY src/SmartInvestigation.Domain/*.csproj src/SmartInvestigation.Domain/

RUN dotnet restore

# Copy everything and build
COPY . ./
WORKDIR /app/src/SmartInvestigation.API
RUN dotnet publish -c Release -o /out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# Render sets a PORT automatically, but we can default to 10000
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "SmartInvestigation.API.dll"]
