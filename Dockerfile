# Use SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and restore dependencies
# The .sln file is in src/ but we'll put it in /app
# The projects are also in src/ but we'll put them in subdirs of /app so paths match the .sln
COPY src/*.sln ./
COPY src/SmartInvestigation.API/*.csproj SmartInvestigation.API/
COPY src/SmartInvestigation.Application/*.csproj SmartInvestigation.Application/
COPY src/SmartInvestigation.Infrastructure/*.csproj SmartInvestigation.Infrastructure/
COPY src/SmartInvestigation.Domain/*.csproj SmartInvestigation.Domain/

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

# Render sets a PORT automatically
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "SmartInvestigation.API.dll"]
