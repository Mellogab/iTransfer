namespace iTransferencia.Core.Entities.Adapters
{
    public class AccountAPI
    {
        public string baseURL { get; set; }
        public string getAccountById { get; set; }
        public string updateBalance { get; set; }
    }

    public class BacenAPI
    {
        public string baseURL { get; set; }
        public string notify { get; set; }
    }

    public class ClientAPI
    {
        public string baseURL { get; set; }
        public string getClientById { get; set; }
    }

    public class ClientsAPI
    {
        public BacenAPI BacenAPI { get; set; }
        public ClientAPI ClientAPI { get; set; }
        public AccountAPI AccountAPI { get; set; }
        public TransferAPI TransferAPI { get; set; }
    }

    public class Root
    {
        public ClientsAPI Clients { get; set; }
    }

    public class TransferAPI
    {
        public string baseURL { get; set; }
        public string executeTransfer { get; set; }
    }
}
