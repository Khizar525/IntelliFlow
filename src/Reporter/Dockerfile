FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Reporter.API/Reporter.API.csproj", "Reporter.API/"]
RUN dotnet restore "Reporter.API/Reporter.API.csproj"
COPY . .
WORKDIR "/src/Reporter.API"
RUN dotnet build "Reporter.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Reporter.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reporter.API.dll"]
