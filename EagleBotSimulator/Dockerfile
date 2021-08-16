FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["EagleBotSimulator/EagleBotSimulator.csproj", "EagleBotSimulator/"]
RUN dotnet restore "EagleBotSimulator/EagleBotSimulator.csproj"
COPY . .
WORKDIR "/src/EagleBotSimulator"
RUN dotnet build "EagleBotSimulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EagleBotSimulator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EagleBotSimulator.dll"]
