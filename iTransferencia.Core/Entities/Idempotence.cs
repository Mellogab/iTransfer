namespace iTransferencia.Core.Entities
{
    public class Idempotence : Entity<int>
    {
        public Idempotence() { }

        public Idempotence(string idempotenceHash) 
        {
            if (idempotenceHash.Length == 0)
                throw new ArgumentException("Um problema no processamento da chave de idempotemcia aconteceu.");

            this.IdempotenceHash = idempotenceHash;
            this.OperationDate = DateTime.Now;
        }

        public string IdempotenceHash { get; set; }
        public DateTime OperationDate { get; set; }
    }
}