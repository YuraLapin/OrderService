FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /source
COPY OrderService/. .
RUN dotnet build -o /app
WORKDIR /app
ENTRYPOINT ["dotnet", "/app/OrderServiceMain.dll"]
