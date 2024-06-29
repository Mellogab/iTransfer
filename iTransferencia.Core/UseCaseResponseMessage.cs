namespace iTransferencia.Core
{
    public class UseCaseResponseMessage
    {
        public bool Success { get; }
        public string Error { get; }
        public string Message { get; }

        public UseCaseResponseMessage(bool success = false, string? message = null, string? error = null)
        {
            this.Success = success;
            this.Message = message;
            this.Error = error;
        }
    }
}
