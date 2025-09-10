FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
COPY OrderService/. .
RUN dotnet build -o /app
ENTRYPOINT ["dotnet", "/app/OrderServiceMain.dll"]
