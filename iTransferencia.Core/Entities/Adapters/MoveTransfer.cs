namespace iTransferencia.Core.Entities.Adapters
{
    public class MoveTransfer
    {
        public string idCliente { get; set; }
        public decimal valor { get; set; }
        public Conta conta { get; set; }
    }

    public class Conta
    {
        public string idOrigem { get; set; }
        public string idDestino { get; set; }
    }
}
