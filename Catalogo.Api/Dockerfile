FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Catalogo.Api/Catalogo.Api.csproj", "Catalogo.Api/"]
COPY ["Catalogo.Application/Catalogo.Application.csproj", "Catalogo.Application/"]
COPY ["Catalogo.Domain/Catalogo.Domain.csproj", "Catalogo.Domain/"]
COPY ["Catalogo.Infrastructure/Catalogo.Infrastructure.csproj", "Catalogo.Infrastructure/"]
RUN dotnet restore "Catalogo.Api/Catalogo.Api.csproj"
COPY . .
WORKDIR "/src/Catalogo.Api"
RUN dotnet build "./Catalogo.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Catalogo.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalogo.Api.dll"]
