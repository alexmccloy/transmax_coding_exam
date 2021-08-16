FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["EagleRock/EagleRock.csproj", "EagleRock/"]
RUN dotnet restore "EagleRock/EagleRock.csproj"
COPY . .
WORKDIR "/src/EagleRock"
RUN dotnet build "EagleRock.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EagleRock.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EagleRock.dll"]
