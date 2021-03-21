# Docker file used in personal jenkins automation server
# You need a settings.json file already in the jenkins working dir for the app to work once built

# Using the microsoft dotnet 5.0 sdk image to build
FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build

# Set working dir
WORKDIR /src

# Copy files from local jenkins work directory to build image work dir
COPY . .

# Restore and build the project
RUN dotnet restore -s https://api.nuget.org/v3/index.json
RUN dotnet build -c Release -o /app

## FROM build AS publish
## RUN dotnet publish -c Release -o /app

# Use the microsoft dotnet 5.0 runtime for the actual app
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base

# Set working dir for actual app
WORKDIR /app

FROM base as final

# Copy files from the build img to the final image
COPY --from=build /app .

# Copy settings file
# COPY ./settings.json .

## Set the docker container timezone
## Game notifications will be posted in this timezone
ENV TZ=America/New_York
ENTRYPOINT ["dotnet", "fmx-cah-host.dll"]
