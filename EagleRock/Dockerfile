#Build Image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app
EXPOSE 80

# Copy csproj file and restore
COPY *.csproj ./
RUN dotnet restore

# Copy source code and then build
COPY . ./
RUN dotnet publish -c Release -o /app/publish

#Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "EagleRock.dll"]
