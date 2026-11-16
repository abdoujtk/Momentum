# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Notice: No ../ anymore, because context is now the root folder
COPY ["Momentum.Api/Momentum.Api.csproj", "Momentum.Api/"]
COPY ["Momentum.Application/Momentum.Application.csproj", "Momentum.Application/"]
COPY ["Momentum.Infrastructure/Momentum.Infrastructure.csproj", "Momentum.Infrastructure/"]
COPY ["Momentum.Domain/Momentum.Domain.csproj", "Momentum.Domain/"]
COPY ["Momentum.Contracts/Momentum.Contracts.csproj", "Momentum.Contracts/"]

RUN dotnet restore "Momentum.Api/Momentum.Api.csproj"

COPY . .
WORKDIR "/src/Momentum.Api"
RUN dotnet build "Momentum.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "Momentum.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Momentum.Api.dll"]