# Сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish ApiGateway.csproj -c Release -o out

# Запуск
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 5171
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
