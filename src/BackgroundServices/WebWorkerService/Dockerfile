#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/BackgroundServices/WebWorkerService/WebWorkerService.csproj", "src/BackgroundServices/WebWorkerService/"]
COPY ["src/Ordering/Ordering.Domain/Ordering.Domain.csproj", "src/Ordering/Ordering.Domain/"]
COPY ["src/Distributing/Distributing.Infrastructure/Distributing.Infrastructure.csproj", "src/Distributing/Distributing.Infrastructure/"]
COPY ["src/Distributing/Distributing.Domain/Distributing.Domain.csproj", "src/Distributing/Distributing.Domain/"]
COPY ["src/Pairing/Pairing.Infrastructure/Pairing.Infrastructure.csproj", "src/Pairing/Pairing.Infrastructure/"]
COPY ["src/Pairing/Pairing.Domain/Pairing.Domain.csproj", "src/Pairing/Pairing.Domain/"]
COPY ["src/Web/WebMVC.Data/WebMVC.Data.csproj", "src/Web/WebMVC.Data/"]
COPY ["src/Web/WebMVC.Model/WebMVC.Model.csproj", "src/Web/WebMVC.Model/"]
COPY ["src/Ordering/Ordering.Infrastructure/Ordering.Infrastructure.csproj", "src/Ordering/Ordering.Infrastructure/"]
RUN dotnet restore "src/BackgroundServices/WebWorkerService/WebWorkerService.csproj"
COPY . .
WORKDIR "/src/src/BackgroundServices/WebWorkerService"
RUN dotnet build "WebWorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebWorkerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebWorkerService.dll"]