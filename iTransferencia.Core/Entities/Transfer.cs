using iTransferencia.Core.Enums;
using iTransferencia.Core.Extensions;

namespace iTransferencia.Core.Entities
{
    public class Transfer : Entity<Guid>
    {
        public Transfer() { }

        public Transfer(string? idSourceAccount, string? idDestinationAccount, decimal value, string? idClient) 
        {
            if (idClient?.Length == 0 || idClient is null)
                throw new ArgumentException("É necessário informar o código do cliente");

            if (idSourceAccount?.Length == 0 || idSourceAccount is null)
                throw new ArgumentException("É necessário informar a conta origem");

            if (idDestinationAccount?.Length == 0 || idDestinationAccount is null)
                throw new ArgumentException("É nccessário informar a conta destino");

            if (value == 0)
                throw new ArgumentException("É nccessário informar o valor");

            this.Id = Guid.NewGuid();
            this.IdClient = idClient;
            this.IdSourceAccount = idSourceAccount;
            this.IdDestinationAccount = idDestinationAccount;
            this.Value = value;
            this.OperationDate = DateTime.Now;
            this.IdempotenceHash = this.GenerateIdempotenceHash();
            this.StatusId = (int)TransferStatuses.PROCESSING;
        }

        public string? IdClient { get; set; }
        public string? IdSourceAccount { get; set; }
        public string? IdDestinationAccount { get; set; }
        public DateTime OperationDate { get; set; }
        public int StatusId { get; set; }
        public decimal Value { get; set; }
        public string IdempotenceHash { get; set; }
        public string ExternalTransactionId { get; set; }

        public string GenerateIdempotenceHash()
        {
            return StringHelper.GenerateIdempotenceHash(this.IdSourceAccount, this.IdDestinationAccount, this.Value);
        }
    }
}