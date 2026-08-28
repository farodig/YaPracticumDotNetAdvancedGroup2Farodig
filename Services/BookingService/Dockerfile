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
COPY ["Services/BookingService/BookingService.Presentation/BookingService.Presentation.csproj", "Services/BookingService/BookingService.Presentation/"]
COPY ["Services/BookingService/BookingService.Application/BookingService.Application.csproj", "Services/BookingService/BookingService.Application/"]
COPY ["Services/BookingService/BookingService.Domain/BookingService.Domain.csproj", "Services/BookingService/BookingService.Domain/"]
COPY ["Services/BookingService/BookingService.Infrastructure/BookingService.Infrastructure.csproj", "Services/BookingService/BookingService.Infrastructure/"]

RUN dotnet restore "Services/BookingService/BookingService.Presentation/BookingService.Presentation.csproj"

COPY . .

WORKDIR "/src/Services/BookingService/BookingService.Presentation"
RUN dotnet build "BookingService.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/Services/BookingService/BookingService.Presentation"
RUN dotnet publish "BookingService.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookingService.Presentation.dll"]