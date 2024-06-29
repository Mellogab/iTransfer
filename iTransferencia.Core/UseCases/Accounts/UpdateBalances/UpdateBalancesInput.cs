using static iTransferencia.Core.IUseCaseRequest;

namespace iTransferencia.Core.UseCases.Accounts.UpdateBalances
{
    public class UpdateBalancesInput : IUseCaseRequest<UpdateBalancesOutput>
    {
        public UpdateBalancesInput() { }

        public UpdateBalancesInput(string idSourceAccount, string idDestinationAccount, decimal value) 
        { 
            if(idDestinationAccount is null || idDestinationAccount is null || value == 0)
                throw new ArgumentNullException("É necessário informar os dados bancários e saldo para atualização do saldo das contas.");

            this.IdSourceAccount = idSourceAccount;
            this.IdDestinationAccount = idDestinationAccount;
            this.Value = value;
        }

        public string IdSourceAccount { get; set; }
        public string IdDestinationAccount { get; set; }
        public decimal SourceBalance { get; set; }
        public decimal Value { get; set; }
    }
}