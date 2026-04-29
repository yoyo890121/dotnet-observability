FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Shared/Protos/Protos.csproj src/Shared/Protos/
COPY src/BackendService/BackendService.csproj src/BackendService/
RUN dotnet restore src/BackendService/BackendService.csproj

COPY src/Shared/Protos/ src/Shared/Protos/
COPY src/BackendService/ src/BackendService/
RUN dotnet publish src/BackendService/BackendService.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "BackendService.dll"]
