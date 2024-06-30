# Estágio base para a aplicação ASP.NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

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

# Copia o script SQL para dentro do contêiner
#COPY ["Database/TransferDatabase.InitialScripts.sql", "TransferDatabase.InitialScripts.sql"]
#COPY ./Database/TransferDatabase.InitialScripts.sql /app/TransferDatabase.InitialScripts.sql
#COPY Database/TransferDatabase.InitialScripts.sql /docker-entrypoint-initdb.d/
#COPY ["Database/TransferDatabase.InitialScripts.sql", "/docker-entrypoint-initdb.d/TransferDatabase.InitialScripts.sql"]

# Variáveis de ambiente para configurar SQL Server
#ENV ACCEPT_EULA=Y
#ENV SA_PASSWORD=adm123

# Executa o script SQL para criar o banco e as tabelas
#RUN /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P $SA_PASSWORD -i /docker-entrypoint-initdb.d/TransferDatabase.InitialScripts.sql

# Estágio final para configurar a aplicação
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
#COPY --from=sqlserver /docker-entrypoint-initdb.d/TransferDatabase.InitialScripts.sql /docker-entrypoint-initdb.d/
ENTRYPOINT ["dotnet", "iTransferencia.dll"]