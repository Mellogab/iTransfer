using static iTransferencia.Core.IUseCaseRequest;

namespace iTransferencia.Core.UseCases.Bacen.NotifyBacen
{
    public class NotifyBacenUseCaseInput : IUseCaseRequest<NotifyBacenUseCaseOutput>
    {
        public NotifyBacenUseCaseInput() { }
        public NotifyBacenUseCaseInput(decimal value, string idSourceAccount, string idDestinationAccount) 
        { 
            if(value == 0 || idDestinationAccount is null || idSourceAccount is null)
                throw new ArgumentNullException("E necessário informar os dados bancários e o valor para notifcar o Bacen");

            this.Value = value;
            this.IdSourceAccount = idSourceAccount;
            this.IdDestinationAccount = idDestinationAccount;
        }

        public decimal Value { get; set; }
        public string? IdSourceAccount { get; set; }
        public string? IdDestinationAccount { get; set; }
    }
}