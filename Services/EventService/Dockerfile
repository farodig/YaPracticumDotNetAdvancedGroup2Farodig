FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Shared/SharedContracts/SharedContracts.csproj", "Shared/SharedContracts/"]
COPY ["Shared/TokenService/TokenService.csproj", "Shared/TokenService/"]
COPY ["Components/Broker/BrokerService.Application/BrokerService.Application.csproj", "Components/Broker/BrokerService.Application/"]
COPY ["Components/Broker/BrokerService.Infrastructure/BrokerService.Infrastructure.csproj", "Components/Broker/BrokerService.Infrastructure/"]
COPY ["Components/Cache/CacheService.Application/CacheService.Application.csproj", "Components/Cache/CacheService.Application/"]
COPY ["Components/Cache/CacheService.Infrastructure/CacheService.Infrastructure.csproj", "Components/Cache/CacheService.Infrastructure/"]
COPY ["Services/EventService/EventService.Presentation/EventService.Presentation.csproj", "Services/EventService/EventService.Presentation/"]
COPY ["Services/EventService/EventService.Application/EventService.Application.csproj", "Services/EventService/EventService.Application/"]
COPY ["Services/EventService/EventService.Domain/EventService.Domain.csproj", "Services/EventService/EventService.Domain/"]
COPY ["Services/EventService/EventService.Infrastructure/EventService.Infrastructure.csproj", "Services/EventService/EventService.Infrastructure/"]

RUN dotnet restore "Services/EventService/EventService.Presentation/EventService.Presentation.csproj"

COPY . .

WORKDIR "/src/Services/EventService/EventService.Presentation"
RUN dotnet build "EventService.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/Services/EventService/EventService.Presentation"
RUN dotnet publish "EventService.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventService.Presentation.dll"]