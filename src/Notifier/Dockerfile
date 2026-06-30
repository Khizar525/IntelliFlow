FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["API/Notifier.API.csproj", "API/"]
RUN dotnet restore "API/Notifier.API.csproj"
COPY . .
WORKDIR "/src/API"
RUN dotnet build "Notifier.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notifier.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notifier.API.dll"]