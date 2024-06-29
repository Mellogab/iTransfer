namespace iTransferencia.Core.Entities.Adapters
{
    public class AccountService
    {
        public AccountService() { }
        public AccountService(
            string id,
            decimal saldo,
            bool? ativo,
            decimal limiteDiario
        )
        {
            if (id is null || saldo == 0 || ativo is null || limiteDiario == 0)
                throw new ArgumentException("Dados da conta do cliente não encontradas");

            this.id = id;
            this.saldo = saldo;
            this.ativo = ativo;
            this.limiteDiario = limiteDiario;
        }

        public bool AccountIsActive() 
        {
            if (this.ativo is null)
                return false;

            return (bool)this.ativo;
        }

        public bool InsufficientBalance(decimal value)
        {
            return value > this.saldo;
        }

        public bool DailyLimitExceeded(decimal value)
        {
            return value > this.limiteDiario;
        }

        public string id { get; set; }
        public decimal saldo { get; set; }
        public bool? ativo { get; set; }
        public decimal limiteDiario { get; set; }
    }

    public class UpdateBalances
    {
        public UpdateBalances() { }
        public UpdateBalances(decimal value, string idSourceAccount, string idDestinationAccount) 
        {
            if (value == 0 || idDestinationAccount is null || idSourceAccount is null)
                throw new ArgumentException("Dados para atualização de conta bancária não informados");

            this.valor = value;
            this.conta = new Conta() { idDestino = idDestinationAccount, idOrigem = idSourceAccount };
        }

        public decimal valor { get; set; }
        public Conta conta { get; set; }
    }
}