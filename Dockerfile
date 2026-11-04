# Backend - ASP.NET Core Web API (.NET 10)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY RedeRestaurante.API.csproj .
RUN dotnet restore RedeRestaurante.API.csproj

COPY . .
RUN dotnet publish RedeRestaurante.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV PORT=8080
EXPOSE 8080

# A plataforma (Cloud Run, Railway, Render, etc.) define a variável PORT
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} dotnet RedeRestaurante.API.dll"]

