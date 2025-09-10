FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY *.sln .
COPY OrderServiceMain/*.csproj ./OrderServiceMain/
RUN dotnet restore

COPY OrderServiceMain/. ./OrderServiceMain
WORKDIR /source/OrderServiceMain
RUN dotnet publish --no-restore -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "OrderServiceMain.dll"]
