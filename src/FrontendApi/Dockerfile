FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Shared/Protos/Protos.csproj src/Shared/Protos/
COPY src/FrontendApi/FrontendApi.csproj src/FrontendApi/
RUN dotnet restore src/FrontendApi/FrontendApi.csproj

COPY src/Shared/Protos/ src/Shared/Protos/
COPY src/FrontendApi/ src/FrontendApi/
RUN dotnet publish src/FrontendApi/FrontendApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "FrontendApi.dll"]
