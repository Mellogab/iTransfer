# Título: Utilização do Design Pattern Saga e Gestão de Falhas com Polly para Operações Críticas

## Contexto
Estamos desenvolvendo um sistema de transferência bancária que requer a atualização de saldo de contas de origem e destino, além de consultas e cadastros de clientes através de chamadas a uma API externa. A operação de atualização de saldo é composta por quatro etapas principais: consulta de saldo da conta de origem via API externa, consulta de saldo da conta de destino via API externa, atualização de saldo da conta de origem via API externa e atualização de saldo da conta de destino via API externa. É crucial garantir que, no caso de falha em qualquer uma das etapas, os saldos das contas sejam revertidos para seus valores originais antes da operação de saldo começar. Além disso, implementamos políticas de retry para consultas GET e utilizamos circuit breaker combinado com cache para otimizar o desempenho e a confiabilidade das operações.

## Decisão
Optamos por aplicar o Design Pattern Saga para coordenar a operação de atualização de saldo das contas de origem e destino através das chamadas à API externa. O Saga é um padrão de projeto que permite controlar transações distribuídas em que várias operações precisam ser coordenadas e, em caso de falha, as alterações feitas até o momento podem ser desfeitas. Além disso, implementamos políticas de retry utilizando Polly para operações de consulta GET, combinadas com circuit breaker e cache para melhorar a confiabilidade e o desempenho das chamadas à API externa.

### Detalhes da Implementação com Polly:
- **Retry para Consultas GET:** Implementamos políticas de retry utilizando Polly para operações de consulta GET às APIs externas. Isso ajuda a lidar com falhas temporárias de rede ou serviço, tentando novamente a operação após um intervalo configurável de tempo.

- **Circuit Breaker + Cache:** Utilizamos o circuit breaker em conjunto com um mecanismo de cache para otimizar o desempenho das consultas. Quando uma consulta é feita com sucesso, o resultado é armazenado em cache. Se o circuit breaker entrar no estado OPEN devido a falhas recorrentes, as consultas subsequentes podem ser atendidas a partir do cache, evitando chamadas repetidas à API externa.

## Consequências
Ao adotar o padrão Saga para coordenar a atualização de saldo das contas através de chamadas a API externa e implementar políticas de retry com Polly para consultas GET, esperamos uma maior robustez e confiabilidade na operação de transferência bancária e no gerenciamento de clientes. No entanto, isso implica em uma complexidade adicional na implementação do código, especialmente na gestão das operações de compensação para cada etapa do Saga e na gestão de falhas com políticas de retry e circuit breaker.

## Alternativas Consideradas
Consideramos outras abordagens, como transações distribuídas utilizando compensação manual ou o uso de filas de mensagens para coordenação assíncrona de operações. Optamos pelo padrão Saga devido à sua capacidade de manter a consistência dos dados financeiros mesmo em cenários de falha parcial das operações com API externa, combinado com as melhorias de desempenho proporcionadas por Polly.

Este registro de decisão de arquitetura (ADR) serve como uma documentação útil para a equipe de desenvolvimento e stakeholders, fornecendo um entendimento claro das razões por trás das escolhas do padrão Saga, políticas de retry com Polly e circuit breaker + cache, e suas implicações para a operação de transferência bancária e gerenciamento de clientes no sistema em desenvolvimento.