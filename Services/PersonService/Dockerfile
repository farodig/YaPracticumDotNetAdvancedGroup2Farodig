FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

RUN apt-get update && apt-get install -y \
    libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Shared/SharedContracts/SharedContracts.csproj", "Shared/SharedContracts/"]
COPY ["Shared/TokenService/TokenService.csproj", "Shared/TokenService/"]
COPY ["Services/PersonService/PersonService.Presentation/PersonService.Presentation.csproj", "Services/PersonService/PersonService.Presentation/"]
COPY ["Services/PersonService/PersonService.Application/PersonService.Application.csproj", "Services/PersonService/PersonService.Application/"]
COPY ["Services/PersonService/PersonService.Domain/PersonService.Domain.csproj", "Services/PersonService/PersonService.Domain/"]
COPY ["Services/PersonService/PersonService.Infrastructure/PersonService.Infrastructure.csproj", "Services/PersonService/PersonService.Infrastructure/"]

RUN dotnet restore "Services/PersonService/PersonService.Presentation/PersonService.Presentation.csproj"

COPY . .

WORKDIR "/src/Services/PersonService/PersonService.Presentation"
RUN dotnet build "PersonService.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/Services/PersonService/PersonService.Presentation"
RUN dotnet publish "PersonService.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PersonService.Presentation.dll"]