# Estágio base para a aplicação ASP.NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8081
EXPOSE 8082

# Estágio de build para compilar a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos do projeto e restaura as dependências
COPY ["iTransferencia/iTransferencia.csproj", "iTransferencia/"]
COPY ["iTransferencia.Core/iTransferencia.Core.csproj", "iTransferencia.Core/"]
COPY ["iTransferencia.Infrastructure/iTransferencia.Infrastructure.csproj", "iTransferencia.Infrastructure/"]
RUN dotnet restore "iTransferencia/iTransferencia.csproj"

# Copia todos os arquivos para o contêiner e realiza o build
COPY . .
WORKDIR "/src/iTransferencia"
RUN dotnet build "iTransferencia.csproj" -c Release -o /app/build

# Estágio de publicação para publicar a aplicação compilada
FROM build AS publish
RUN dotnet publish "iTransferencia.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio final para configurar SQL Server e banco de dados
FROM mcr.microsoft.com/mssql/server:2019-latest AS sqlserver
WORKDIR /app

# Estágio final para configurar a aplicação
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "iTransferencia.dll"]