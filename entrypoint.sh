#!/bin/bash
/opt/mssql/bin/sqlservr &

# Espera o SQL Server inicializar
sleep 30s

# Executa o script de inicializa��o
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'numsey#2021' -i /docker-entrypoint-initdb.d/TransferDatabase.InitialScripts.sql

# Mant�m o container rodando
tail -f /dev/null