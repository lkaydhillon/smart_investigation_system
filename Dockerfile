# Use SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and project files
COPY src/*.sln ./
COPY src/SmartInvestigation.API/SmartInvestigation.API.csproj SmartInvestigation.API/
COPY src/SmartInvestigation.Application/SmartInvestigation.Application.csproj SmartInvestigation.Application/
COPY src/SmartInvestigation.Infrastructure/SmartInvestigation.Infrastructure.csproj SmartInvestigation.Infrastructure/
COPY src/SmartInvestigation.Domain/SmartInvestigation.Domain.csproj SmartInvestigation.Domain/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY src/ . 

# Build and publish
WORKDIR /app/SmartInvestigation.API
RUN dotnet publish -c Release -o /out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# FIX for Status 139: Disable globalization to avoid segmentation fault
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENV ASPNETCORE_URLS=http://+:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "SmartInvestigation.API.dll"]
