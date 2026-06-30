FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ResearchSummarizer.API/ResearchSummarizer.API.csproj", "ResearchSummarizer.API/"]
RUN dotnet restore "ResearchSummarizer.API/ResearchSummarizer.API.csproj"
COPY . .
WORKDIR "/src/ResearchSummarizer.API"
RUN dotnet build "ResearchSummarizer.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ResearchSummarizer.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ResearchSummarizer.API.dll"]
