-- Usar o Banco de Dados 'Transfer'
USE Transfer;
GO

-- Criar Tabela 'Idempotence'
CREATE TABLE Idempotence (
    id INT IDENTITY(1,1) NOT NULL,
    idempotenceHash VARCHAR(250) NULL,
    operationDate DATETIME2(7) NOT NULL CONSTRAINT DF_Idempotence_operationDate DEFAULT (GETDATE()),
    PRIMARY KEY CLUSTERED (id)
);
GO

-- Criar Índice na Coluna idempotenceHash
CREATE INDEX idx_Idempotence_idempotenceHash
ON Idempotence (idempotenceHash);
GO

-- Tabela 'TransferStatuses' (com base nos dados fornecidos)
CREATE TABLE TransferStatuses (
    id INT PRIMARY KEY,
    name NVARCHAR(100) NOT NULL
);

-- Inserir dados na tabela TransferStatuses
INSERT INTO TransferStatuses (id, name) VALUES
(1, 'Em processamento'),
(2, 'Falha na transferência'),
(3, 'Falha na atualização de saldo da conta origem'),
(4, 'Falha na atualização de saldo da conta destino'),
(5, 'Falha na notificação para o bacen'),
(6, 'Transferência enviada com sucesso'),
(7, 'Saldo das contas atualizados'),
(8, 'Falha ao atualizar saldo das contas bancárias'),
(9, 'Erro desconhecido ao processar solicitação');
GO

-- Criar Tabela 'Transfers'
CREATE TABLE Transfers (
    id UNIQUEIDENTIFIER NOT NULL,
    idSourceAccount VARCHAR(100) NOT NULL,
    idDestinationAccount VARCHAR(100) NOT NULL,
    value DECIMAL(18, 2) NOT NULL,
    operationDate DATETIME2(7) NOT NULL CONSTRAINT DF_Transfers_operationDate DEFAULT (GETDATE()),
    externalTransactionId VARCHAR(100) NULL,
    statusId INT NULL,
    idClient VARCHAR(50) NULL,
    PRIMARY KEY CLUSTERED (id),
    CONSTRAINT FK_Transfers_Status FOREIGN KEY (statusId) REFERENCES TransferStatuses(id)
);
GO



