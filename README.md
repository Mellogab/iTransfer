# iTransfer
Este repositório inclui uma API de transferência bancária como parte de solução de um problema.

## Visão Geral

Este projeto envolve o desenvolvimento de uma API .NET Core para gerenciamento de transferências bancárias. A API expõe um endpoint para operações de transferência, Ele segue os princípios de design da API RESTful, Clean Architecture para uma melhor organização do projeto e isolamento do domínio, modelagem de domínios ricos e design patterns adequados ao problema a ser resolvido.

## Tecnologias Utilizadas

- **.NET Core 8**: A versão mais recente do .NET Core, oferecendo desempenho aprimorado e suporte multiplataforma.
- **C#**: Linguagem de programação principal para desenvolvimento de back-end.
- **Entity Framework Core**: Usado para acesso ao banco de dados e mapeamento objeto-relacional.
- **xUnit**: Estrutura de teste de unidade usada para testes automatizados.
- **MOQ/AutoMock**: Utilizado para mockar classes, interfaces no projeto de testes.
- **Polly**: Usado para aplicar padrões de resiliência (retry + circuit breaker) em partes dos endpoints de consultas de APIS externas.
- **Docker**: Usado para empacotar e implantar a API como um contêiner.

## Estrutura do Projeto

O projeto é dividido em quatro projetos internos:

1. **API**: Contém os controladores de API.
2. **Core**: Inclui casos de uso, entidades e lógica de domínio.
3. **Testes**: Abriga os testes de unidade e integração para garantir a qualidade do código.
4. **Infraestrutura**: Fornece a camada de acesso ao banco de dados e configurações de dependência.

## Estrutura de Diretórios


```sh
/iTransfer
  /iTransferencia
    /Controllers
      TranferController.cs
    /Presenters
      DefaultPresenter
    /Configurations
    Program.cs
  /iTransferencia.Core
    /Entities
      /Adapters
        AccountService
        ClientService
        TransferService
      Entity.cs
      Transfer.cs
      Idempotence.cs
    /Enums
      TransferStatus.cs
    /Extensions
      StringHelper.cs
    /Patterns
      ISagaOrquestrator.cs
    /Repository
      IIdempotenceRepository.cs
      IRepository.cs
      IRepositoryBase.cs
      ITransferRepository.cs
    /Services
      IAccountService.cs
      IBacenService.cs
      ICacheService.cs
      IClienteService.cs
      IHttpRequestService.cs
    /UseCases
      /Bacen
        NotifyBacen
          INotifyBacenUseCase.cs
          NotifyBacenUseCaseInput.cs
          NotifyBacenUseCaseOutput.cs
          NotifyBacenUseCase.cs
      /Transfers
        ExecuteTransfer
          IExecuteTransferUseCase.cs
          ExecuteTransferUseCaseInput.cs
          ExecuteTransferUseCaseOutput.cs
          ExecuteTransferUseCase.cs
        UpdateBalances
          IUpdateBalancesUseCase.cs
          UpdateBalancesUseCaseInput.cs
          UpdateBalancesUseCaseOutput.cs
          UpdateBalancesUseCase.cs
    IOutputPort.cs
    IUseCaseRequest.cs
    IUseCaseRequestHandler.cs
    OutputPort.cs
    UseCaseResponseMessage.cs
  /iTransferencia.Infrastructure
    /Automapper
      MappingProfile.cs
    /Configurations
      Retry.cs
      CircuitBreaker.cs
    /DbContext
      ApplicationDbContext
    /EntityFrameworkDataAccess
      /Repositories
        IdempotenceRepository.cs
        Transferrepository.cs
      RepositoryBase.cs
    /Patterns
      /Sagas
        SagaOrquestrator.cs
    /Services
      AccountService.cs
      TransferService.cs
      BacenService.cs
      ClientService.cs
      HttpRequestService.cs
  /iTransferencia.UnitTests
    /Entities
      TransferEntityTests.cs
    /UseCases
      ExecuteUseCaseTests.cs
      NotifyBacenUseCase.cs
      UpdateBalancesUseCaseTests.cs
    /Services
      HttpRequestServiceTests.cs
    /Patterns
      SagaOrquestratorTests.cs
  iTransferencia.sln
  README.md
```

## Casos de Uso

Os casos de uso definem a lógica de negócios e são implementados no projeto Core. Aqui estão alguns exemplos:

- **ExecuteTransferUseCase**: Este caso de uso lida com a criação de uma nova transferência, consulta e atualização de saldos das contas e notificação da transferência para o bacen.
- **UpdateBalancesUseCase**: Este caso de uso gerencia a atualização do saldo das contas de origem e destino.
- **NotifyBacenUseCase**: Este caso de uso gerencia a notificação de uma transferência realizada para bacen.

## Entidades

As entidades representam os objetos de domínio e são definidas no projeto Core. Aqui estão alguns exemplos:

- **Tranfer**: Inclui propriedades como ID, conta de origem, conta de destino, valor e data.
- **Idempotence**: Contém o hash transacional usado para identificar se a transferência já aconteceu para a mesma data. 

## Testes

Os 25 testes iniciais são implementados no projeto Testes e cobrem casos de uso, entidades, classes de serviço e padrões de projeto:

![image](https://github.com/Mellogab/iTransfer/assets/7771245/31da294e-580f-4bfa-90fb-67f817ac11bb)

- **Tranfer**: Inclui propriedades como ID, conta de origem, conta de destino, valor e data.
- **Idempotence**: Contém o hash transacional usado para identificar se a transferência já aconteceu para a mesma data. 

## Infraestrutura

O projeto de infraestrutura gerencia o acesso ao banco de dados e configurações de dependência. Aqui estão alguns pontos-chave:

- **Entity Framework Core**: Usado para mapear entidades para tabelas do banco de dados e gerenciar acesso aos dados.
- **Banco de Dados**: O banco de dados usado é SQLServer para desenvolvimento local e pode ser substituído por um banco de dados relacional para produção. 
- **Injeção de Dependência**: O projeto utiliza injeção de dependência para gerenciar dependências entre classes e módulos.
- **Configurações**: Configurações específicas do ambiente são gerenciadas usando arquivos de configuração.
  
## Instrução de instação
Para instalar e executar o projeto, siga estas etapas:

1. Clone o repositório:
```sh
git clone https://github.com/Mellogab/iTransfer.git
```   
2. Navegue até a pasta do projeto:
```sh
cd iTransfer
```   
3. Restaure as dependências:
```sh
dotnet restore
```
4. Configure a string de conexão no arquivo appsettings.json na pasta API:
```sh
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=NomeDoBanco;User Id=usuario;Password=senha;"
  }
}
```
5. Execute o projeto:
```sh
dotnet run --project iTransferencia
```

## Executando os Testes
Para executar os testes unitários e de integração, use o seguinte comando:

```sh
dotnet test
```

## Endpoints da API

A API expõe os seguintes endpoints:

Transferências

  POST /api/transfers-async: Enviar uma transferência bancária.
