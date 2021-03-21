Raw
Permalink
Blame
History
  
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
copy . .
RUN dotnet restore -s https://api.nuget.org/v3/index.json
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENV TZ=America/New_York
ENTRYPOINT ["dotnet", "fmx-cah-host.dll"]
